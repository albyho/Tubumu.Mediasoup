using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
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

        public RouterData _data { get; }

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
        /// <param name="logger"></param>
        /// <param name="@internal"></param>
        /// <param name="rtpCapabilities"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        public Router(ILoggerFactory loggerFactory,
            RouterInternal @internal,
            RouterData data,
            IChannel channel,
            Dictionary<string, object>? appData
            )
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Router>();

            _internal = @internal;
            _data = data;
            _channel = channel;
            AppData = appData ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Close the Router.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug($"CloseAsync() | Router:{RouterId}");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;

                // Fire and forget
                var reqData = _internal;
                _channel.RequestAsync(MethodId.WORKER_CLOSE_ROUTER, null, reqData).ContinueWithOnFaultedHandleLog(_logger);

                await CloseInternalAsync();

                Emit("@close");

                // Emit observer event.
                Observer.Emit("close");
            };
        }

        /// <summary>
        /// Worker was closed.
        /// </summary>
        public async Task WorkerClosedAsync()
        {
            _logger.LogDebug($"WorkerClosedAsync() | Router:{RouterId}");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
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

        private async Task CloseInternalAsync()
        {
            using (await _transportsLock.WriteLockAsync())
            {
                // Close every Transport.
                foreach (var transport in _transports.Values)
                {
                    await transport.RouterClosedAsync();
                }

                _transports.Clear();
            }

            using (await _producersLock.WriteLockAsync())
            {
                // Clear the Producers map.
                _producers.Clear();
            }

            using (await _rtpObserversLock.WriteLockAsync())
            {
                // Close every RtpObserver.
                foreach (var rtpObserver in _rtpObservers.Values)
                {
                    await rtpObserver.RouterClosedAsync();
                }
                _rtpObservers.Clear();
            }

            using (await _dataProducersLock.WriteLockAsync())
            {
                // Clear the DataProducers map.
                _dataProducers.Clear();
            }

            using (await _mapRouterPipeTransportsLock.WriteLockAsync())
            {
                // Clear map of Router/PipeTransports.
                _mapRouterPipeTransports.Clear();
            }
        }

        /// <summary>
        /// Dump Router.
        /// </summary>
        public async Task<string> DumpAsync()
        {
            _logger.LogDebug($"DumpAsync() | Router:{RouterId}");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                return (await _channel.RequestAsync(MethodId.ROUTER_DUMP, _internal.RouterId))!;
            }
        }

        /// <summary>
        /// Create a WebRtcTransport.
        /// </summary>
        public async Task<WebRtcTransport> CreateWebRtcTransportAsync(WebRtcTransportOptions webRtcTransportOptions)
        {
            _logger.LogDebug("CreateWebRtcTransportAsync()");

            if (webRtcTransportOptions.WebRtcServer == null && webRtcTransportOptions.ListenIps.IsNullOrEmpty())
            {
                throw new ArgumentException("missing webRtcServer and listenIps (one of them is mandatory)");
            }

            var webRtcServer = webRtcTransportOptions.WebRtcServer;
            if (webRtcServer != null)
            {
                webRtcTransportOptions.ListenIps = null;
                webRtcTransportOptions.Port = null;
            }

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                var reqData = new
                {
                    TransportId = Guid.NewGuid().ToString(),
                    webRtcServer?.WebRtcServerId,
                    webRtcTransportOptions.ListenIps,
                    webRtcTransportOptions.Port,
                    webRtcTransportOptions.EnableUdp,
                    webRtcTransportOptions.EnableTcp,
                    webRtcTransportOptions.PreferUdp,
                    webRtcTransportOptions.PreferTcp,
                    webRtcTransportOptions.InitialAvailableOutgoingBitrate,
                    webRtcTransportOptions.EnableSctp,
                    webRtcTransportOptions.NumSctpStreams,
                    webRtcTransportOptions.MaxSctpMessageSize,
                    webRtcTransportOptions.MaxSctpSendBufferSize,
                    IsDataChannel = true
                };

                var resData = await _channel.RequestAsync(webRtcTransportOptions.WebRtcServer != null ?
                    MethodId.ROUTER_CREATE_WEBRTC_TRANSPORT_WITH_SERVER : MethodId.ROUTER_CREATE_WEBRTC_TRANSPORT,
                    _internal.RouterId,
                    reqData);
                var responseData = JsonSerializer.Deserialize<RouterCreateWebRtcTransportResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                var transport = new WebRtcTransport(_loggerFactory,
                                    new TransportInternal(_internal.RouterId, reqData.TransportId),
                                    responseData, // 直接使用返回值
                                    _channel,
                                    webRtcTransportOptions.AppData,
                                    () => _data.RtpCapabilities,
                                    async m =>
                                    {
                                        using (await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    },
                                    async m =>
                                    {
                                        using (await _dataProducersLock.ReadLockAsync())
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

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                if (plainTransportOptions.ListenIp == null || plainTransportOptions.ListenIp.Ip.IsNullOrWhiteSpace())
                {
                    throw new ArgumentException("Missing listenIp");
                }

                var reqData = new
                {
                    TransportId = Guid.NewGuid().ToString(),
                    plainTransportOptions.ListenIp,
                    plainTransportOptions.Port,
                    plainTransportOptions.Comedia,
                    plainTransportOptions.EnableSctp,
                    plainTransportOptions.NumSctpStreams,
                    plainTransportOptions.MaxSctpMessageSize,
                    plainTransportOptions.SctpSendBufferSize,
                    IsDataChannel = false,
                    plainTransportOptions.EnableSrtp,
                    plainTransportOptions.SrtpCryptoSuite
                };

                var resData = await _channel.RequestAsync(MethodId.ROUTER_CREATE_PLAIN_TRANSPORT, _internal.RouterId, reqData);
                var responseData = JsonSerializer.Deserialize<RouterCreatePlainTransportResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                var transport = new PlainTransport(_loggerFactory,
                                    new TransportInternal(_internal.RouterId, reqData.TransportId),
                                    responseData, // 直接使用返回值
                                    _channel,
                                    plainTransportOptions.AppData,
                                    () => _data.RtpCapabilities,
                                    async m =>
                                    {
                                        using (await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    },
                                    async m =>
                                    {
                                        using (await _dataProducersLock.ReadLockAsync())
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
        public async Task<PipeTransport> CreatePipeTransportAsync(PipeTransportOptions pipeTransportOptions)
        {
            _logger.LogDebug("CreatePipeTransportAsync()");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                if (pipeTransportOptions.ListenIp == null)
                {
                    throw new ArgumentNullException(nameof(pipeTransportOptions.ListenIp), "Missing listenIp");
                }

                var reqData = new
                {
                    TransportId = Guid.NewGuid().ToString(),
                    pipeTransportOptions.ListenIp,
                    pipeTransportOptions.Port,
                    pipeTransportOptions.EnableSctp,
                    pipeTransportOptions.NumSctpStreams,
                    pipeTransportOptions.MaxSctpMessageSize,
                    pipeTransportOptions.SctpSendBufferSize,
                    IsDataChannel = false,
                    pipeTransportOptions.EnableRtx,
                    pipeTransportOptions.EnableSrtp,
                };

                var resData = await _channel.RequestAsync(MethodId.ROUTER_CREATE_PIPE_TRANSPORT, _internal.RouterId, reqData);
                var responseData = JsonSerializer.Deserialize<RouterCreatePipeTransportResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                var transport = new PipeTransport(_loggerFactory,
                                    new TransportInternal(_internal.RouterId, reqData.TransportId),
                                    responseData, // 直接使用返回值
                                    _channel,
                                    pipeTransportOptions.AppData,
                                    () => _data.RtpCapabilities,
                                    async m =>
                                    {
                                        using (await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    },
                                    async m =>
                                    {
                                        using (await _dataProducersLock.ReadLockAsync())
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
        /// <param name="directTransportOptions"></param>
        /// <returns></returns>
        public async Task<DirectTransport> CreateDirectTransportAsync(DirectTransportOptions directTransportOptions)
        {
            _logger.LogDebug("CreateDirectTransportAsync()");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                var reqData = new
                {
                    TransportId = Guid.NewGuid().ToString(),
                    Direct = true,
                    directTransportOptions.MaxMessageSize,
                };

                var resData = await _channel.RequestAsync(MethodId.ROUTER_CREATE_DIRECT_TRANSPORT, _internal.RouterId, reqData);
                var responseData = JsonSerializer.Deserialize<RouterCreateDirectTransportResponseData>(resData!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                var transport = new DirectTransport(_loggerFactory,
                                    new TransportInternal(RouterId, reqData.TransportId),
                                    responseData, // 直接使用返回值
                                    _channel,
                                    directTransportOptions.AppData,
                                    () => _data.RtpCapabilities,
                                    async m =>
                                    {
                                        using (await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    },
                                    async m =>
                                    {
                                        using (await _dataProducersLock.ReadLockAsync())
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
            using (await _transportsLock.WriteLockAsync())
            {
                _transports[transport.TransportId] = transport;
            }

            transport.On("@close", async (_, _) =>
            {
                using (await _transportsLock.WriteLockAsync())
                {
                    _transports.Remove(transport.TransportId);
                }
            });
            transport.On("@listenserverclose", async (_, _) =>
            {
                using (await _transportsLock.WriteLockAsync())
                {
                    _transports.Remove(transport.TransportId);
                }
            });
            transport.On("@newproducer", async (_, obj) =>
            {
                var producer = (Producer)obj!;
                using (await _producersLock.WriteLockAsync())
                {
                    _producers[producer.ProducerId] = producer;
                }
            });
            transport.On("@producerclose", async (_, obj) =>
            {
                var producer = (Producer)obj!;
                using (await _producersLock.WriteLockAsync())
                {
                    _producers.Remove(producer.ProducerId);
                }
            });
            transport.On("@newdataproducer", async (_, obj) =>
            {
                var dataProducer = (DataProducer)obj!;
                using (await _dataProducersLock.WriteLockAsync())
                {
                    _dataProducers[dataProducer.DataProducerId] = dataProducer;
                }
            });
            transport.On("@dataproducerclose", async (_, obj) =>
            {
                var dataProducer = (DataProducer)obj!;
                using (await _dataProducersLock.WriteLockAsync())
                {
                    _dataProducers.Remove(dataProducer.DataProducerId);
                }
            });

            // Emit observer event.
            Observer.Emit("newtransport", transport);

            if (webRtcServer != null && transport is WebRtcTransport webRtcTransport)
            {
                await webRtcServer.HandleWebRtcTransportAsync(webRtcTransport);
            }
        }

        /// <summary>
        /// Pipes the given Producer or DataProducer into another Router in same host.
        /// </summary>
        /// <param name="pipeToRouterOptions">ListenIp 传入 127.0.0.1, EnableSrtp 传入 true 。</param>
        /// <returns></returns>
        public async Task<PipeToRouterResult> PipeToRouteAsync(PipeToRouterOptions pipeToRouterOptions)
        {
            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                if (pipeToRouterOptions.ListenIp == null)
                {
                    throw new ArgumentNullException(nameof(pipeToRouterOptions.ListenIp), "Missing listenIp");
                }

                if (pipeToRouterOptions.ProducerId.IsNullOrWhiteSpace() && pipeToRouterOptions.DataProducerId.IsNullOrWhiteSpace())
                {
                    throw new ArgumentException("Missing producerId or dataProducerId");
                }

                if (!pipeToRouterOptions.ProducerId.IsNullOrWhiteSpace() && !pipeToRouterOptions.DataProducerId.IsNullOrWhiteSpace())
                {
                    throw new ArgumentException("Just producerId or dataProducerId can be given");
                }

                if (pipeToRouterOptions.Router == null)
                {
                    throw new ArgumentNullException(nameof(pipeToRouterOptions.Router), "Router not found");
                }

                if (pipeToRouterOptions.Router == this)
                {
                    throw new ArgumentException("Cannot use this Router as destination");
                }

                Producer? producer = null;
                DataProducer? dataProducer = null;

                if (!pipeToRouterOptions.ProducerId.IsNullOrWhiteSpace())
                {
                    using (await _producersLock.ReadLockAsync())
                    {
                        if (!_producers.TryGetValue(pipeToRouterOptions.ProducerId!, out producer))
                        {
                            throw new Exception("Producer not found");
                        }
                    }
                }
                else if (!pipeToRouterOptions.DataProducerId.IsNullOrWhiteSpace())
                {
                    using (await _dataProducersLock.ReadLockAsync())
                    {
                        if (!_dataProducers.TryGetValue(pipeToRouterOptions.DataProducerId!, out dataProducer))
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
                using (await _mapRouterPipeTransportsLock.WriteLockAsync())
                {
                    if (_mapRouterPipeTransports.TryGetValue(pipeToRouterOptions.Router, out var pipeTransportPair))
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
                                ListenIp = pipeToRouterOptions.ListenIp,
                                EnableSctp = pipeToRouterOptions.EnableSctp,
                                NumSctpStreams = pipeToRouterOptions.NumSctpStreams,
                                EnableRtx = pipeToRouterOptions.EnableRtx,
                                EnableSrtp = pipeToRouterOptions.EnableSrtp
                            }),
                            pipeToRouterOptions.Router.CreatePipeTransportAsync(new PipeTransportOptions
                            {
                                ListenIp = pipeToRouterOptions.ListenIp,
                                EnableSctp = pipeToRouterOptions.EnableSctp,
                                NumSctpStreams = pipeToRouterOptions.NumSctpStreams,
                                EnableRtx = pipeToRouterOptions.EnableRtx,
                                EnableSrtp = pipeToRouterOptions.EnableSrtp
                            })
                            );

                            localPipeTransport = pipeTransports[0];
                            remotePipeTransport = pipeTransports[1];

                            await Task.WhenAll(localPipeTransport.ConnectAsync(new PipeTransportConnectParameters
                            {
                                Ip = remotePipeTransport.Data.Tuple.LocalIp,
                                Port = remotePipeTransport.Data.Tuple.LocalPort,
                                SrtpParameters = remotePipeTransport.Data.SrtpParameters,
                            }),
                            remotePipeTransport.ConnectAsync(new PipeTransportConnectParameters
                            {
                                Ip = localPipeTransport.Data.Tuple.LocalIp,
                                Port = localPipeTransport.Data.Tuple.LocalPort,
                                SrtpParameters = localPipeTransport.Data.SrtpParameters,
                            })
                            );

                            localPipeTransport.Observer.On("close", async (_, _) =>
                            {
                                await remotePipeTransport.CloseAsync();
                                using (await _mapRouterPipeTransportsLock.WriteLockAsync())
                                {
                                    _mapRouterPipeTransports.Remove(pipeToRouterOptions.Router);
                                }
                            });

                            remotePipeTransport.Observer.On("close", async (_, _) =>
                            {
                                await localPipeTransport.CloseAsync();
                                using (await _mapRouterPipeTransportsLock.WriteLockAsync())
                                {
                                    _mapRouterPipeTransports.Remove(pipeToRouterOptions.Router);
                                }
                            });

                            _mapRouterPipeTransports[pipeToRouterOptions.Router] = new[] { localPipeTransport, remotePipeTransport };
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"PipeToRouterAsync() | Create PipeTransport pair failed.");

                            if (localPipeTransport != null)
                            {
                                await localPipeTransport.CloseAsync();
                            }

                            if (remotePipeTransport != null)
                            {
                                await remotePipeTransport.CloseAsync();
                            }

                            throw;
                        }
                    }
                }

                if (producer != null)
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
                        if (producer.Closed)
                            throw new InvalidStateException("original Producer closed");

                        // Ensure that producer.paused has not changed in the meanwhile and, if
                        // so, sync the pipeProducer.
                        if (pipeProducer.Paused != producer.Paused)
                        {
                            if (producer.Paused)
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
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"PipeToRouterAsync() | Create pipe Consumer/Producer pair failed");

                        if (pipeConsumer != null)
                        {
                            await pipeConsumer.CloseAsync();
                        }

                        if (pipeProducer != null)
                        {
                            await pipeProducer.CloseAsync();
                        }

                        throw;
                    }
                }
                else if (dataProducer != null)
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
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"PipeToRouterAsync() | Create pipe DataConsumer/DataProducer pair failed.");

                        if (pipeDataConsumer != null)
                        {
                            await pipeDataConsumer.CloseAsync();
                        }

                        if (pipeDataProducer != null)
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

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                var reqData = new
                {
                    RtpObserverId = Guid.NewGuid().ToString(),
                    activeSpeakerObserverOptions.Interval
                };

                // Fire and forget
                _channel.RequestAsync(MethodId.ROUTER_CREATE_ACTIVE_SPEAKER_OBSERVER, _internal.RouterId, reqData).ContinueWithOnFaultedHandleLog(_logger);

                var activeSpeakerObserver = new ActiveSpeakerObserver(_loggerFactory,
                                    new RtpObserverInternal(_internal.RouterId, reqData.RtpObserverId),
                                    _channel,
                                    activeSpeakerObserverOptions.AppData,
                                    async m =>
                                    {
                                        using (await _producersLock.ReadLockAsync())
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

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Router closed");
                }

                var reqData = new
                {
                    RtpObserverId = Guid.NewGuid().ToString(),
                    audioLevelObserverOptions.MaxEntries,
                    audioLevelObserverOptions.Threshold,
                    audioLevelObserverOptions.Interval
                };

                // Fire and forget
                _channel.RequestAsync(MethodId.ROUTER_CREATE_AUDIO_LEVEL_OBSERVER, _internal.RouterId, reqData).ContinueWithOnFaultedHandleLog(_logger);

                var audioLevelObserver = new AudioLevelObserver(_loggerFactory,
                                    new RtpObserverInternal(_internal.RouterId, reqData.RtpObserverId),
                                    _channel,
                                    audioLevelObserverOptions.AppData,
                                    async m =>
                                    {
                                        using (await _producersLock.ReadLockAsync())
                                        {
                                            return _producers.TryGetValue(m, out var p) ? p : null;
                                        }
                                    });
                await ConfigureRtpObserverAsync(audioLevelObserver);

                return audioLevelObserver;
            }
        }

        private Task ConfigureRtpObserverAsync(RtpObserver rtpObserver)
        {
            _rtpObservers[rtpObserver.Internal.RtpObserverId] = rtpObserver;
            rtpObserver.On("@close", async (_, _) =>
            {
                using (await _rtpObserversLock.WriteLockAsync())
                {
                    _rtpObservers.Remove(rtpObserver.Internal.RtpObserverId);
                }
            });

            // Emit observer event.
            Observer.Emit("newrtpobserver", rtpObserver);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Check whether the given RTP capabilities can consume the given Producer.
        /// </summary>
        public async Task<bool> CanConsumeAsync(string producerId, RtpCapabilities rtpCapabilities)
        {
            using (await _producersLock.ReadLockAsync())
            {
                if (!_producers.TryGetValue(producerId, out var producer))
                {
                    _logger.LogError($"CanConsume() | Producer with id {producerId} not found");
                    return false;
                }
                try
                {
                    return ORTC.CanConsume(producer.Data.ConsumableRtpParameters, rtpCapabilities);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "CanConsume() | Unexpected error");
                    return false;
                }
            }
        }

        #region IEquatable<T>

        public bool Equals(Router? other)
        {
            if (other is null)
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
    }
}
