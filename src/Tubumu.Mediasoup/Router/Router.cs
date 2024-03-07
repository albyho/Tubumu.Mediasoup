using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBS.Request;
using FBS.Router;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public sealed class Router : EventEmitter, IEquatable<Router>
    {
        /// <summary>
        /// Logger factory for create logger.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<Router> _logger;

        /// <summary>
        /// Whether the Router is closed.
        /// </summary>
        private bool _closed;

        /// <summary>
        /// Close locker.
        /// </summary>
        private readonly AsyncReaderWriterLock _closeLock = new();

        #region Internal data.

        private readonly RouterInternal _internal;

        public string RouterId => _internal.RouterId;

        #endregion Internal data.

        #region Router data.

        public RouterData Data { get; }

        #endregion Router data.

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly IChannel _channel;

        /// <summary>
        /// Transports map.
        /// </summary>
        private readonly Dictionary<string, Transport> _transports = new();
        private readonly AsyncReaderWriterLock _transportsLock = new();

        /// <summary>
        /// Producers map.
        /// </summary>
        private readonly Dictionary<string, Producer> _producers = new();
        private readonly AsyncReaderWriterLock _producersLock = new();

        /// <summary>
        /// RtpObservers map.
        /// </summary>
        private readonly Dictionary<string, RtpObserver> _rtpObservers = new();
        private readonly AsyncReaderWriterLock _rtpObserversLock = new();

        /// <summary>
        /// DataProducers map.
        /// </summary>
        private readonly Dictionary<string, DataProducer> _dataProducers = new();
        private readonly AsyncReaderWriterLock _dataProducersLock = new();

        /// <summary>
        /// Router to PipeTransport map.
        /// </summary>
        private readonly Dictionary<Router, PipeTransport[]> _mapRouterPipeTransports = new();
        private readonly AsyncReaderWriterLock _mapRouterPipeTransportsLock = new();

        /// <summary>
        /// App custom data.
        /// </summary>
        public Dictionary<string, object> AppData { get; }

        /// <summary>
        /// Observer instance.
        /// </summary>
        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits workerclose</para>
        /// <para>@emits @close</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits newtransport - (transport: Transport)</para>
        /// <para>@emits newrtpobserver - (rtpObserver: RtpObserver)</para>
        /// </summary>
        public Router(ILoggerFactory loggerFactory,
            RouterInternal internal_,
            RouterData data,
            IChannel channel,
            Dictionary<string, object>? appData
            )
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Router>();

            _internal = internal_;
            Data = data;
            _channel = channel;
            AppData = appData ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Close the Router.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug("CloseAsync() | Router:{RouterId}", RouterId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var closeRouterRequest = new FBS.Worker.CloseRouterRequestT
                {
                    RouterId = _internal.RouterId,
                };

                var closeRouterRequestOffset = FBS.Worker.CloseRouterRequest.Pack(bufferBuilder, closeRouterRequest);

                // Fire and forget
                _channel.RequestAsync(bufferBuilder, Method.WORKER_CLOSE_ROUTER,
                    Body.Worker_CloseRouterRequest,
                    closeRouterRequestOffset.Value
                    ).ContinueWithOnFaultedHandleLog(_logger);

                await CloseInternalAsync();

                Emit("@close");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Worker was closed.
        /// </summary>
        public async Task WorkerClosedAsync()
        {
            _logger.LogDebug("WorkerClosedAsync() | Router:{RouterId}", RouterId);

            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _closed = true;

                await CloseInternalAsync();

                Emit("workerclose");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Dump Router.
        /// </summary>
        public async Task<FBS.Router.DumpResponseT> DumpAsync()
        {
            _logger.LogDebug("DumpAsync() | Router:{RouterId}", RouterId);

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var response = await _channel.RequestAsync(bufferBuilder, Method.ROUTER_DUMP, null, null, _internal.RouterId);
                var data = response.Value.BodyAsRouter_DumpResponse().UnPack();

                return data;
            }
        }

        /// <summary>
        /// Create a WebRtcTransport.
        /// </summary>
        public async Task<WebRtcTransport> CreateWebRtcTransportAsync(WebRtcTransportOptions webRtcTransportOptions)
        {
            _logger.LogDebug("CreateWebRtcTransportAsync()");

            if(webRtcTransportOptions.WebRtcServer == null && webRtcTransportOptions.ListenInfos.IsNullOrEmpty())
            {
                throw new ArgumentException("missing webRtcServer and listenIps (one of them is mandatory)");
            }
            /*
            else if(webRtcTransportOptions.WebRtcServer != null && !webRtcTransportOptions.ListenInfos.IsNullOrEmpty())
            {
                throw new ArgumentException("only one of webRtcServer, listenInfos and listenIps must be given");
            }
            */

            var webRtcServer = webRtcTransportOptions.WebRtcServer;

            // If webRtcServer is given, then do not force default values for enableUdp
            // and enableTcp. Otherwise set them if unset.
            if(webRtcServer != null)
            {
                webRtcTransportOptions.EnableUdp ??= true;
                webRtcTransportOptions.EnableTcp ??= true;
            }
            else
            {
                webRtcTransportOptions.EnableUdp ??= true;
                webRtcTransportOptions.EnableTcp ??= false;
            }

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                /* Build Request. */
                FBS.WebRtcTransport.ListenServerT? webRtcTransportListenServer = null;
                FBS.WebRtcTransport.ListenIndividualT? webRtcTransportListenIndividual = null;
                if(webRtcServer != null)
                {
                    webRtcTransportListenServer = new FBS.WebRtcTransport.ListenServerT
                    {
                        WebRtcServerId = webRtcServer.WebRtcServerId
                    };
                }
                else
                {
                    var fbsListenInfos = webRtcTransportOptions.ListenInfos!.Select(m => new FBS.Transport.ListenInfoT
                    {
                        Protocol = m.Protocol,
                        Ip = m.Ip,
                        AnnouncedAddress = m.AnnouncedAddress,
                        Port = m.Port,
                        Flags = m.Flags,
                        SendBufferSize = m.SendBufferSize,
                        RecvBufferSize = m.RecvBufferSize,
                    }).ToList();

                    webRtcTransportListenIndividual =
                        new FBS.WebRtcTransport.ListenIndividualT
                        {
                            ListenInfos = fbsListenInfos,
                        };
                }

                var baseTransportOptions = new FBS.Transport.OptionsT
                {
                    Direct = false,
                    MaxMessageSize = null,
                    InitialAvailableOutgoingBitrate = webRtcTransportOptions.InitialAvailableOutgoingBitrate,
                    EnableSctp = webRtcTransportOptions.EnableSctp,
                    NumSctpStreams = webRtcTransportOptions.NumSctpStreams,
                    MaxSctpMessageSize = webRtcTransportOptions.MaxSctpMessageSize,
                    SctpSendBufferSize = webRtcTransportOptions.SctpSendBufferSize,
                    IsDataChannel = true
                };

                var webRtcTransportOptionsForCreate = new FBS.WebRtcTransport.WebRtcTransportOptionsT
                {
                    Base = baseTransportOptions,
                    EnableUdp = webRtcTransportOptions.EnableUdp!.Value,
                    EnableTcp = webRtcTransportOptions.EnableTcp!.Value,
                    PreferUdp = webRtcTransportOptions.PreferUdp,
                    PreferTcp = webRtcTransportOptions.PreferTcp,
                    IceConsentTimeout = webRtcTransportOptions.IceConsentTimeout,
                    Listen = new FBS.WebRtcTransport.ListenUnion
                    {
                        Type = webRtcServer != null ? FBS.WebRtcTransport.Listen.ListenServer : FBS.WebRtcTransport.Listen.ListenIndividual,
                        Value = webRtcServer != null ? webRtcTransportListenServer : webRtcTransportListenIndividual
                    }
                };

                var transportId = Guid.NewGuid().ToString();

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var createWebRtcTransportRequest = new CreateWebRtcTransportRequestT
                {
                    TransportId = transportId,
                    Options = webRtcTransportOptionsForCreate
                };

                var createWebRtcTransportRequestOffset = FBS.Router.CreateWebRtcTransportRequest.Pack(bufferBuilder, createWebRtcTransportRequest);

                var response = await _channel.RequestAsync(bufferBuilder, webRtcServer != null
                        ? Method.ROUTER_CREATE_WEBRTCTRANSPORT_WITH_SERVER
                        : Method.ROUTER_CREATE_WEBRTCTRANSPORT,
                        Body.Router_CreateWebRtcTransportRequest,
                        createWebRtcTransportRequestOffset.Value,
                        _internal.RouterId);

                /* Decode Response. */
                var data = response.Value.BodyAsWebRtcTransport_DumpResponse().UnPack();

                var transport = new WebRtcTransport(_loggerFactory,
                                    new TransportInternal(_internal.RouterId, transportId),
                                    data, // 直接使用返回值
                                    _channel,
                                    webRtcTransportOptions.AppData,
                                    () => Data.RtpCapabilities,
                                    async m =>
                                    {
                                        using(await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    },
                                    async m =>
                                    {
                                        using(await _dataProducersLock.ReadLockAsync())
                                        {
                                            return _dataProducers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    }
                                    );

                await ConfigureTransportAsync(transport, webRtcServer);

                return transport;
            }
        }

        /// <summary>
        /// Create a PlainTransport.
        /// </summary>
        public async Task<PlainTransport> CreatePlainTransportAsync(PlainTransportOptions plainTransportOptions)
        {
            _logger.LogDebug("CreatePlainTransportAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                if(plainTransportOptions.ListenInfo?.Ip.IsNullOrWhiteSpace() != false)
                {
                    throw new ArgumentException("Missing ListenInfo");
                }

                // If rtcpMux is enabled, ignore rtcpListenInfo.
                if(plainTransportOptions.RtcpMux && plainTransportOptions.RtcpListenInfo != null)
                {
                    _logger.LogWarning("createPlainTransport() | ignoring rtcpMux since rtcpListenInfo is given");
                    plainTransportOptions.RtcpMux = false;
                }

                var baseTransportOptions = new FBS.Transport.OptionsT
                {
                    Direct = false,
                    MaxMessageSize = null,
                    InitialAvailableOutgoingBitrate = null,
                    EnableSctp = plainTransportOptions.EnableSctp,
                    NumSctpStreams = plainTransportOptions.NumSctpStreams,
                    MaxSctpMessageSize = plainTransportOptions.MaxSctpMessageSize,
                    SctpSendBufferSize = plainTransportOptions.SctpSendBufferSize,
                    IsDataChannel = false
                };

                var plainTransportOptionsForCreate = new FBS.PlainTransport.PlainTransportOptionsT
                {
                    Base = baseTransportOptions,
                    ListenInfo = plainTransportOptions.ListenInfo,
                    RtcpListenInfo = plainTransportOptions.RtcpListenInfo,
                    RtcpMux = plainTransportOptions.RtcpMux,
                    Comedia = plainTransportOptions.Comedia,
                };

                var transportId = Guid.NewGuid().ToString();

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var createPlainTransportRequest = new CreatePlainTransportRequestT
                {
                    TransportId = transportId,
                    Options = plainTransportOptionsForCreate
                };

                var createPlainTransportRequestOffset = FBS.Router.CreatePlainTransportRequest.Pack(bufferBuilder, createPlainTransportRequest);

                var response = await _channel.RequestAsync(bufferBuilder, Method.ROUTER_CREATE_PLAINTRANSPORT,
                        Body.Router_CreatePlainTransportRequest,
                        createPlainTransportRequestOffset.Value,
                        _internal.RouterId);

                /* Decode Response. */
                var data = response.Value.BodyAsPlainTransport_DumpResponse().UnPack();

                var transport = new PlainTransport(_loggerFactory,
                                    new TransportInternal(_internal.RouterId, transportId),
                                    data, // 直接使用返回值
                                    _channel,
                                    plainTransportOptions.AppData,
                                    () => Data.RtpCapabilities,
                                    async m =>
                                    {
                                        using(await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    },
                                    async m =>
                                    {
                                        using(await _dataProducersLock.ReadLockAsync())
                                        {
                                            return _dataProducers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    }
                                );

                await ConfigureTransportAsync(transport);

                return transport;
            }
        }

        /// <summary>
        /// Create a PipeTransport.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidStateException"></exception>
        public async Task<PipeTransport> CreatePipeTransportAsync(PipeTransportOptions pipeTransportOptions)
        {
            _logger.LogDebug("CreatePipeTransportAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                if(pipeTransportOptions.ListenInfo?.Ip.IsNullOrWhiteSpace() != false)
                {
                    throw new ArgumentException("Missing ListenInfo");
                }

                var baseTransportOptions = new FBS.Transport.OptionsT
                {
                    Direct = false,
                    MaxMessageSize = null,
                    InitialAvailableOutgoingBitrate = null,
                    EnableSctp = pipeTransportOptions.EnableSctp,
                    NumSctpStreams = pipeTransportOptions.NumSctpStreams,
                    MaxSctpMessageSize = pipeTransportOptions.MaxSctpMessageSize,
                    SctpSendBufferSize = pipeTransportOptions.SctpSendBufferSize,
                    IsDataChannel = false
                };

                var listenInfo = pipeTransportOptions.ListenInfo;

                var pipeTransportOptionsForCreate = new FBS.PipeTransport.PipeTransportOptionsT
                {
                    Base = baseTransportOptions,
                    ListenInfo = pipeTransportOptions.ListenInfo,
                    EnableRtx = pipeTransportOptions.EnableRtx,
                    EnableSrtp = pipeTransportOptions.EnableSrtp,
                };

                var transportId = Guid.NewGuid().ToString();

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var createPipeTransportRequest = new CreatePipeTransportRequestT
                {
                    TransportId = transportId,
                    Options = pipeTransportOptionsForCreate
                };

                var createPipeTransportRequestOffset = CreatePipeTransportRequest.Pack(bufferBuilder, createPipeTransportRequest);

                var response = await _channel.RequestAsync(bufferBuilder, Method.ROUTER_CREATE_PIPETRANSPORT,
                        Body.Router_CreatePipeTransportRequest,
                        createPipeTransportRequestOffset.Value,
                        _internal.RouterId);

                /* Decode Response. */
                var data = response.Value.BodyAsPipeTransport_DumpResponse().UnPack();

                var transport = new PipeTransport(_loggerFactory,
                                    new TransportInternal(_internal.RouterId, transportId),
                                    data, // 直接使用返回值
                                    _channel,
                                    pipeTransportOptions.AppData,
                                    () => Data.RtpCapabilities,
                                    async m =>
                                    {
                                        using(await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    },
                                    async m =>
                                    {
                                        using(await _dataProducersLock.ReadLockAsync())
                                        {
                                            return _dataProducers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    });

                await ConfigureTransportAsync(transport);

                return transport;
            }
        }

        /// <summary>
        /// Create a DirectTransport.
        /// </summary>
        public async Task<DirectTransport> CreateDirectTransportAsync(DirectTransportOptions directTransportOptions)
        {
            _logger.LogDebug("CreateDirectTransportAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                var baseTransportOptions = new FBS.Transport.OptionsT
                {
                    Direct = true,
                    MaxMessageSize = directTransportOptions.MaxMessageSize,
                };

                var reqData = new
                {
                    TransportId = Guid.NewGuid().ToString(),
                    Direct = true,
                    directTransportOptions.MaxMessageSize,
                };

                var directTransportOptionsForCreate = new FBS.DirectTransport.DirectTransportOptionsT
                {
                    Base = baseTransportOptions,
                };

                var transportId = Guid.NewGuid().ToString();

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var createDirectTransportRequest = new CreateDirectTransportRequestT
                {
                    TransportId = transportId,
                    Options = directTransportOptionsForCreate
                };

                var createDirectTransportRequestOffset = CreateDirectTransportRequest.Pack(bufferBuilder, createDirectTransportRequest);

                var response = await _channel.RequestAsync(bufferBuilder, Method.ROUTER_CREATE_DIRECTTRANSPORT,
                        Body.Router_CreateDirectTransportRequest,
                        createDirectTransportRequestOffset.Value,
                        _internal.RouterId);

                /* Decode Response. */
                var data = response.Value.BodyAsDirectTransport_DumpResponse().UnPack();

                var transport = new DirectTransport(_loggerFactory,
                                    new TransportInternal(RouterId, transportId),
                                    data, // 直接使用返回值
                                    _channel,
                                    directTransportOptions.AppData,
                                    () => Data.RtpCapabilities,
                                    async m =>
                                    {
                                        using(await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    },
                                    async m =>
                                    {
                                        using(await _dataProducersLock.ReadLockAsync())
                                        {
                                            return _dataProducers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    }
                                    );

                await ConfigureTransportAsync(transport);

                return transport;
            }
        }

        private async Task ConfigureTransportAsync(Transport transport, WebRtcServer? webRtcServer = null)
        {
            using(await _transportsLock.WriteLockAsync())
            {
                _transports[transport.TransportId] = transport;
            }

            transport.On("@close", async (_, _) =>
            {
                using(await _transportsLock.WriteLockAsync())
                {
                    _transports.Remove(transport.TransportId);
                }
            });
            transport.On("@listenserverclose", async (_, _) =>
            {
                using(await _transportsLock.WriteLockAsync())
                {
                    _transports.Remove(transport.TransportId);
                }
            });
            transport.On("@newproducer", async (_, obj) =>
            {
                var producer = (Producer)obj!;
                using(await _producersLock.WriteLockAsync())
                {
                    _producers[producer.ProducerId] = producer;
                }
            });
            transport.On("@producerclose", async (_, obj) =>
            {
                var producer = (Producer)obj!;
                using(await _producersLock.WriteLockAsync())
                {
                    _producers.Remove(producer.ProducerId);
                }
            });
            transport.On("@newdataproducer", async (_, obj) =>
            {
                var dataProducer = (DataProducer)obj!;
                using(await _dataProducersLock.WriteLockAsync())
                {
                    _dataProducers[dataProducer.DataProducerId] = dataProducer;
                }
            });
            transport.On("@dataproducerclose", async (_, obj) =>
            {
                var dataProducer = (DataProducer)obj!;
                using(await _dataProducersLock.WriteLockAsync())
                {
                    _dataProducers.Remove(dataProducer.DataProducerId);
                }
            });

            // Emit observer event.
            Observer.Emit("newtransport", transport);

            if(webRtcServer != null && transport is WebRtcTransport webRtcTransport)
            {
                await webRtcServer.HandleWebRtcTransportAsync(webRtcTransport);
            }
        }

        /// <summary>
        /// Pipes the given Producer or DataProducer into another Router in same host.
        /// </summary>
        /// <param name="pipeToRouterOptions">ListenIp 传入 127.0.0.1, EnableSrtp 传入 true 。</param>
        ///
        public async Task<PipeToRouterResult> PipeToRouteAsync(PipeToRouterOptions pipeToRouterOptions)
        {
            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                if(pipeToRouterOptions.ListenInfo == null)
                {
                    throw new ArgumentNullException(nameof(pipeToRouterOptions), "Missing listenInfo");
                }

                if(pipeToRouterOptions.ProducerId.IsNullOrWhiteSpace() && pipeToRouterOptions.DataProducerId.IsNullOrWhiteSpace())
                {
                    throw new ArgumentException("Missing producerId or dataProducerId");
                }

                if(!pipeToRouterOptions.ProducerId.IsNullOrWhiteSpace() && !pipeToRouterOptions.DataProducerId.IsNullOrWhiteSpace())
                {
                    throw new ArgumentException("Just producerId or dataProducerId can be given");
                }

                if(pipeToRouterOptions.Router == null)
                {
                    throw new ArgumentNullException(nameof(pipeToRouterOptions), "Router not found");
                }

                if(pipeToRouterOptions.Router == this)
                {
                    throw new ArgumentException("Cannot use this Router as destination");
                }

                Producer? producer = null;
                DataProducer? dataProducer = null;

                if(!pipeToRouterOptions.ProducerId.IsNullOrWhiteSpace())
                {
                    using(await _producersLock.ReadLockAsync())
                    {
                        if(!_producers.TryGetValue(pipeToRouterOptions.ProducerId!, out producer))
                        {
                            throw new Exception("Producer not found");
                        }
                    }
                }
                else if(!pipeToRouterOptions.DataProducerId.IsNullOrWhiteSpace())
                {
                    using(await _dataProducersLock.ReadLockAsync())
                    {
                        if(!_dataProducers.TryGetValue(pipeToRouterOptions.DataProducerId!, out dataProducer))
                        {
                            throw new Exception("DataProducer not found");
                        }
                    }
                }

                // Here we may have to create a new PipeTransport pair to connect source and
                // destination Routers. We just want to keep a PipeTransport pair for each
                // pair of Routers. Since this operation is async, it may happen that two
                // simultaneous calls to router1.pipeToRouter({ producerId: xxx, router: router2 })
                // would end up generating two pairs of PipeTranports. To prevent that, let's
                // use an async queue.

                PipeTransport? localPipeTransport = null;
                PipeTransport? remotePipeTransport = null;

                // 因为有可能新增，所以用写锁。
                using(await _mapRouterPipeTransportsLock.WriteLockAsync())
                {
                    if(_mapRouterPipeTransports.TryGetValue(pipeToRouterOptions.Router, out var pipeTransportPair))
                    {
                        localPipeTransport = pipeTransportPair[0];
                        remotePipeTransport = pipeTransportPair[1];
                    }
                    else
                    {
                        try
                        {
                            var pipeTransports = await Task.WhenAll(CreatePipeTransportAsync(new PipeTransportOptions
                            {
                                ListenInfo = pipeToRouterOptions.ListenInfo,
                                EnableSctp = pipeToRouterOptions.EnableSctp,
                                NumSctpStreams = pipeToRouterOptions.NumSctpStreams,
                                EnableRtx = pipeToRouterOptions.EnableRtx,
                                EnableSrtp = pipeToRouterOptions.EnableSrtp
                            }),
                            pipeToRouterOptions.Router.CreatePipeTransportAsync(new PipeTransportOptions
                            {
                                ListenInfo = pipeToRouterOptions.ListenInfo,
                                EnableSctp = pipeToRouterOptions.EnableSctp,
                                NumSctpStreams = pipeToRouterOptions.NumSctpStreams,
                                EnableRtx = pipeToRouterOptions.EnableRtx,
                                EnableSrtp = pipeToRouterOptions.EnableSrtp
                            })
                            );

                            localPipeTransport = pipeTransports[0];
                            remotePipeTransport = pipeTransports[1];

                            await Task.WhenAll(localPipeTransport.ConnectAsync(new FBS.PipeTransport.ConnectRequestT
                            {
                                Ip = remotePipeTransport.Data.Tuple.LocalAddress,
                                Port = remotePipeTransport.Data.Tuple.LocalPort,
                                SrtpParameters = remotePipeTransport.Data.SrtpParameters,
                            }),
                            remotePipeTransport.ConnectAsync(new FBS.PipeTransport.ConnectRequestT
                            {
                                Ip = localPipeTransport.Data.Tuple.LocalAddress,
                                Port = localPipeTransport.Data.Tuple.LocalPort,
                                SrtpParameters = localPipeTransport.Data.SrtpParameters,
                            })
                            );

                            localPipeTransport.Observer.On("close", async (_, _) =>
                            {
                                await remotePipeTransport.CloseAsync();
                                using(await _mapRouterPipeTransportsLock.WriteLockAsync())
                                {
                                    _mapRouterPipeTransports.Remove(pipeToRouterOptions.Router);
                                }
                            });

                            remotePipeTransport.Observer.On("close", async (_, _) =>
                            {
                                await localPipeTransport.CloseAsync();
                                using(await _mapRouterPipeTransportsLock.WriteLockAsync())
                                {
                                    _mapRouterPipeTransports.Remove(pipeToRouterOptions.Router);
                                }
                            });

                            _mapRouterPipeTransports[pipeToRouterOptions.Router] = new[] { localPipeTransport, remotePipeTransport };
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError(ex, "PipeToRouterAsync() | Create PipeTransport pair failed.");

                            if(localPipeTransport != null)
                            {
                                await localPipeTransport.CloseAsync();
                            }

                            if(remotePipeTransport != null)
                            {
                                await remotePipeTransport.CloseAsync();
                            }

                            throw;
                        }
                    }
                }

                if(producer != null)
                {
                    Consumer? pipeConsumer = null;
                    Producer? pipeProducer = null;

                    try
                    {
                        pipeConsumer = await localPipeTransport.ConsumeAsync(new ConsumerOptions
                        {
                            ProducerId = pipeToRouterOptions.ProducerId!
                        });

                        pipeProducer = await remotePipeTransport.ProduceAsync(new ProducerOptions
                        {
                            Id = producer.ProducerId,
                            Kind = pipeConsumer.Data.Kind,
                            RtpParameters = pipeConsumer.Data.RtpParameters,
                            Paused = pipeConsumer.ProducerPaused,
                            AppData = producer.AppData,
                        });

                        // Ensure that the producer has not been closed in the meanwhile.
                        if(producer.Closed)
                            throw new InvalidStateException("original Producer closed");

                        // Ensure that producer.paused has not changed in the meanwhile and, if
                        // so, sync the pipeProducer.
                        if(pipeProducer.Paused != producer.Paused)
                        {
                            if(producer.Paused)
                                await pipeProducer.PauseAsync();
                            else
                                await pipeProducer.ResumeAsync();
                        }

                        // Pipe events from the pipe Consumer to the pipe Producer.
                        pipeConsumer.Observer.On("close", async (_, _) => await pipeProducer.CloseAsync());
                        pipeConsumer.Observer.On("pause", async (_, _) => await pipeProducer.PauseAsync());
                        pipeConsumer.Observer.On("resume", async (_, _) => await pipeProducer.ResumeAsync());

                        // Pipe events from the pipe Producer to the pipe Consumer.
                        pipeProducer.Observer.On("close", async (_, _) => await pipeConsumer.CloseAsync());

                        return new PipeToRouterResult { PipeConsumer = pipeConsumer, PipeProducer = pipeProducer };
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "PipeToRouterAsync() | Create pipe Consumer/Producer pair failed");

                        if(pipeConsumer != null)
                        {
                            await pipeConsumer.CloseAsync();
                        }

                        if(pipeProducer != null)
                        {
                            await pipeProducer.CloseAsync();
                        }

                        throw;
                    }
                }
                else if(dataProducer != null)
                {
                    DataConsumer? pipeDataConsumer = null;
                    DataProducer? pipeDataProducer = null;

                    try
                    {
                        pipeDataConsumer = await localPipeTransport.ConsumeDataAsync(new DataConsumerOptions
                        {
                            DataProducerId = pipeToRouterOptions.DataProducerId!
                        });

                        pipeDataProducer = await remotePipeTransport.ProduceDataAsync(new DataProducerOptions
                        {
                            Id = dataProducer.DataProducerId,
                            SctpStreamParameters = pipeDataConsumer.Data.SctpStreamParameters,
                            Label = pipeDataConsumer.Data.Label,
                            Protocol = pipeDataConsumer.Data.Protocol,
                            AppData = dataProducer.AppData,
                        });

                        // Pipe events from the pipe DataConsumer to the pipe DataProducer.
                        pipeDataConsumer.Observer.On("close", async (_, _) => await pipeDataProducer.CloseAsync());

                        // Pipe events from the pipe DataProducer to the pipe DataConsumer.
                        pipeDataProducer.Observer.On("close", async (_, _) => await pipeDataConsumer.CloseAsync());

                        return new PipeToRouterResult { PipeDataConsumer = pipeDataConsumer, PipeDataProducer = pipeDataProducer };
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "PipeToRouterAsync() | Create pipe DataConsumer/DataProducer pair failed.");

                        if(pipeDataConsumer != null)
                        {
                            await pipeDataConsumer.CloseAsync();
                        }

                        if(pipeDataProducer != null)
                        {
                            await pipeDataProducer.CloseAsync();
                        }

                        throw;
                    }
                }
                else
                {
                    throw new Exception("Internal error");
                }
            }
        }

        /// <summary>
        /// Create an ActiveSpeakerObserver
        /// </summary>
        public async Task<ActiveSpeakerObserver> CreateActiveSpeakerObserverAsync(ActiveSpeakerObserverOptions activeSpeakerObserverOptions)
        {
            _logger.LogDebug("CreateActiveSpeakerObserverAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                var rtpObserverId = Guid.NewGuid().ToString();

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var createActiveSpeakerObserverRequest = new CreateActiveSpeakerObserverRequestT
                {
                    RtpObserverId = rtpObserverId,
                    Options = new FBS.ActiveSpeakerObserver.ActiveSpeakerObserverOptionsT
                    {
                        Interval = activeSpeakerObserverOptions.Interval,
                    }
                };

                var createActiveSpeakerObserverRequestOffset = CreateActiveSpeakerObserverRequest.Pack(bufferBuilder, createActiveSpeakerObserverRequest);

                // Fire and forget
                _channel.RequestAsync(bufferBuilder, Method.ROUTER_CREATE_ACTIVESPEAKEROBSERVER,
                    Body.Router_CreateActiveSpeakerObserverRequest,
                    createActiveSpeakerObserverRequestOffset.Value,
                    _internal.RouterId
                    ).ContinueWithOnFaultedHandleLog(_logger);

                var activeSpeakerObserver = new ActiveSpeakerObserver(_loggerFactory,
                                    new RtpObserverInternal(_internal.RouterId, rtpObserverId),
                                    _channel,
                                    activeSpeakerObserverOptions.AppData,
                                    async m =>
                                    {
                                        using(await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    });

                await ConfigureRtpObserverAsync(activeSpeakerObserver);

                return activeSpeakerObserver;
            }
        }

        /// <summary>
        /// Create an AudioLevelObserver.
        /// </summary>
        public async Task<AudioLevelObserver> CreateAudioLevelObserverAsync(AudioLevelObserverOptions audioLevelObserverOptions)
        {
            _logger.LogDebug("CreateAudioLevelObserverAsync()");

            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                var rtpObserverId = Guid.NewGuid().ToString();

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var createAudioLevelObserverRequest = new CreateAudioLevelObserverRequestT
                {
                    RtpObserverId = rtpObserverId,
                    Options = new FBS.AudioLevelObserver.AudioLevelObserverOptionsT
                    {
                        MaxEntries = audioLevelObserverOptions.MaxEntries,
                        Threshold = audioLevelObserverOptions.Threshold,
                        Interval = audioLevelObserverOptions.Interval,
                    }
                };

                var createAudioLevelObserverRequestOffset = CreateAudioLevelObserverRequest.Pack(bufferBuilder, createAudioLevelObserverRequest);

                // Fire and forget
                _channel.RequestAsync(bufferBuilder, Method.ROUTER_CREATE_AUDIOLEVELOBSERVER,
                    Body.Router_CreateAudioLevelObserverRequest,
                    createAudioLevelObserverRequestOffset.Value,
                    _internal.RouterId
                    ).ContinueWithOnFaultedHandleLog(_logger);

                var audioLevelObserver = new AudioLevelObserver(_loggerFactory,
                                    new RtpObserverInternal(_internal.RouterId, rtpObserverId),
                                    _channel,
                                    audioLevelObserverOptions.AppData,
                                    async m =>
                                    {
                                        using(await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    });

                await ConfigureRtpObserverAsync(audioLevelObserver);

                return audioLevelObserver;
            }
        }

        /// <summary>
        /// Check whether the given RTP capabilities can consume the given Producer.
        /// </summary>
        public async Task<bool> CanConsumeAsync(string producerId, RtpCapabilities rtpCapabilities)
        {
            using(await _producersLock.ReadLockAsync())
            {
                if(!_producers.TryGetValue(producerId, out var producer))
                {
                    _logger.LogError("CanConsume() | Producer with id {producerId} not found", producerId);
                    return false;
                }

                try
                {
                    return ORTC.CanConsume(producer.Data.ConsumableRtpParameters, rtpCapabilities);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "CanConsume() | Unexpected error");
                    return false;
                }
            }
        }

        #region IEquatable<T>

        public bool Equals(Router? other)
        {
            if(other is null)
            {
                return false;
            }

            return RouterId == other.RouterId;
        }

        public override bool Equals(object? other)
        {
            return Equals(other as Router);
        }

        public override int GetHashCode()
        {
            return RouterId.GetHashCode();
        }

        #endregion IEquatable<T>

        private async Task CloseInternalAsync()
        {
            using(await _transportsLock.WriteLockAsync())
            {
                // Close every Transport.
                foreach(var transport in _transports.Values)
                {
                    await transport.RouterClosedAsync();
                }

                _transports.Clear();
            }

            using(await _producersLock.WriteLockAsync())
            {
                // Clear the Producers map.
                _producers.Clear();
            }

            using(await _rtpObserversLock.WriteLockAsync())
            {
                // Close every RtpObserver.
                foreach(var rtpObserver in _rtpObservers.Values)
                {
                    await rtpObserver.RouterClosedAsync();
                }

                _rtpObservers.Clear();
            }

            using(await _dataProducersLock.WriteLockAsync())
            {
                // Clear the DataProducers map.
                _dataProducers.Clear();
            }

            using(await _mapRouterPipeTransportsLock.WriteLockAsync())
            {
                // Clear map of Router/PipeTransports.
                _mapRouterPipeTransports.Clear();
            }
        }

        private Task ConfigureRtpObserverAsync(RtpObserver rtpObserver)
        {
            _rtpObservers[rtpObserver.Internal.RtpObserverId] = rtpObserver;
            rtpObserver.On("@close", async (_, _) =>
            {
                using(await _rtpObserversLock.WriteLockAsync())
                {
                    _rtpObservers.Remove(rtpObserver.Internal.RtpObserverId);
                }
            });

            // Emit observer event.
            Observer.Emit("newrtpobserver", rtpObserver);

            return Task.CompletedTask;
        }
    }
}
