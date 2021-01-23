using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tubumu.Core.Extensions;
using TubumuMeeting.Mediasoup.Extensions;

namespace TubumuMeeting.Mediasoup
{
    public class Router : EventEmitter, IEquatable<Router>
    {
        /// <summary>
        /// Logger factory for create logger.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<Router> _logger;

        // TODO: (alby) _closed 的使用及线程安全。
        /// <summary>
        /// Whether the Router is closed.
        /// </summary>
        private bool _closed;

        #region Internal data.

        public string RouterId { get; }

        private readonly object _internal;

        #endregion Internal data.

        #region Router data.

        public RtpCapabilities RtpCapabilities { get; }

        #endregion Router data.

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly Channel _channel;

        /// <summary>
        /// PayloadChannel instance.
        /// </summary>
        private readonly PayloadChannel _payloadChannel;

        /// <summary>
        /// Transports map.
        /// </summary>
        private readonly ConcurrentDictionary<string, Transport> _transports = new ConcurrentDictionary<string, Transport>();

        /// <summary>
        /// Producers map.
        /// </summary>
        private readonly ConcurrentDictionary<string, Producer> _producers = new ConcurrentDictionary<string, Producer>();

        /// <summary>
        /// RtpObservers map.
        /// </summary>
        private readonly ConcurrentDictionary<string, RtpObserver> _rtpObservers = new ConcurrentDictionary<string, RtpObserver>();

        /// <summary>
        /// DataProducers map.
        /// </summary>
        private readonly ConcurrentDictionary<string, DataProducer> _dataProducers = new ConcurrentDictionary<string, DataProducer>();

        /// <summary>
        /// Router to PipeTransport map.
        /// </summary>
        private readonly ConcurrentDictionary<Router, PipeTransport[]> _mapRouterPipeTransports = new ConcurrentDictionary<Router, PipeTransport[]>();

        /// <summary>
        /// App custom data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; private set; }

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
        /// <param name="routerId"></param>
        /// <param name="rtpCapabilities"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        public Router(ILoggerFactory loggerFactory,
            string routerId,
            RtpCapabilities rtpCapabilities,
            Channel channel,
            PayloadChannel payloadChannel,
            Dictionary<string, object>? appData
            )
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Router>();
            RouterId = routerId;
            _internal = new
            {
                RouterId,
            };
            RtpCapabilities = rtpCapabilities;
            _channel = channel;
            _payloadChannel = payloadChannel;
            AppData = appData;
        }

        /// <summary>
        /// Close the Router.
        /// </summary>
        public async Task CloseAsync()
        {
            if (_closed)
            {
                return;
            }

            _logger.LogDebug("CloseAsync() | Router");

            _closed = true;

            // Fire and forget
            _channel.RequestAsync(MethodId.ROUTER_CLOSE, _internal).ContinueWithOnFaultedHandleLog(_logger);

            await CloseInternalAsync();

            Emit("@close");

            // Emit observer event.
            Observer.Emit("close");
        }

        /// <summary>
        /// Worker was closed.
        /// </summary>
        public async Task WorkerClosedAsync()
        {
            if (_closed)
            {
                return;
            }

            _logger.LogDebug($"WorkerClosedAsync() | Router:{RouterId}");

            _closed = true;

            await CloseInternalAsync();

            Emit("workerclose");

            // Emit observer event.
            Observer.Emit("close");
        }

        private async Task CloseInternalAsync()
        {
            // Close every Transport.
            foreach (var transport in _transports.Values)
            {
                await transport.RouterClosedAsync();
            }

            _transports.Clear();

            // Clear the Producers map.
            _producers.Clear();

            // Close every RtpObserver.
            foreach (var rtpObserver in _rtpObservers.Values)
            {
                rtpObserver.RouterClosed();
            }
            _rtpObservers.Clear();

            // Clear the DataProducers map.
            _dataProducers.Clear();

            // Clear map of Router/PipeTransports.
            _mapRouterPipeTransports.Clear();
        }

        /// <summary>
        /// Dump Router.
        /// </summary>
        public Task<string?> DumpAsync()
        {
            _logger.LogDebug("DumpAsync()");

            return _channel.RequestAsync(MethodId.ROUTER_DUMP, _internal);
        }

        /// <summary>
        /// Create a WebRtcTransport.
        /// </summary>
        public async Task<WebRtcTransport> CreateWebRtcTransportAsync(WebRtcTransportOptions webRtcTransportOptions)
        {
            _logger.LogDebug("CreateWebRtcTransportAsync()");

            var @internal = new
            {
                RouterId,
                TransportId = Guid.NewGuid().ToString(),
            };
            var reqData = new
            {
                webRtcTransportOptions.ListenIps,
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

            var status = await _channel.RequestAsync(MethodId.ROUTER_CREATE_WEBRTC_TRANSPORT, @internal, reqData);
            var responseData = JsonConvert.DeserializeObject<RouterCreateWebRtcTransportResponseData>(status!);

            var transport = new WebRtcTransport(_loggerFactory,
                new TransportInternalData(@internal.RouterId, @internal.TransportId),
                sctpParameters: null,
                sctpState: null,
                _channel,
                _payloadChannel,
                webRtcTransportOptions.AppData,
                () => RtpCapabilities,
                m => _producers.TryGetValue(m, out var p) ? p : null,
                m => _dataProducers.TryGetValue(m, out var p) ? p : null,
                responseData.IceRole,
                responseData.IceParameters,
                responseData.IceCandidates,
                responseData.IceState,
                responseData.IceSelectedTuple,
                responseData.DtlsParameters,
                responseData.DtlsState,
                responseData.DtlsRemoteCert
                );
            _transports[transport.TransportId] = transport;

            transport.On("@close", _ =>
            {
                _transports.TryRemove(transport.TransportId, out var _);
                return Task.CompletedTask;
            });
            transport.On("@newproducer", obj =>
            {
                var producer = (Producer)obj!;
                _producers[producer.ProducerId] = producer;
                return Task.CompletedTask;
            });
            transport.On("@producerclose", obj =>
            {
                var producer = (Producer)obj!;
                _producers.TryRemove(producer.ProducerId, out var _);
                return Task.CompletedTask;
            });
            transport.On("@newdataproducer", obj =>
            {
                var dataProducer = (DataProducer)obj!;
                _dataProducers[dataProducer.DataProducerId] = dataProducer;
                return Task.CompletedTask;
            });
            transport.On("@dataproducerclose", obj =>
            {
                var dataProducer = (DataProducer)obj!;
                _dataProducers.TryRemove(dataProducer.DataProducerId, out var _);
                return Task.CompletedTask;
            });

            // Emit observer event.
            Observer.Emit("newtransport", transport);

            return transport;
        }

        /// <summary>
        /// Create a PlainTransport.
        /// </summary>
        public async Task<PlainTransport> CreatePlainTransportAsync(PlainTransportOptions plainTransportOptions)
        {
            _logger.LogDebug("CreatePlainTransportAsync()");

            if (plainTransportOptions.ListenIp == null || plainTransportOptions.ListenIp.Ip.IsNullOrWhiteSpace())
            {
                throw new Exception("Missing listenIp");
            }

            var @internal = new
            {
                RouterId,
                TransportId = Guid.NewGuid().ToString(),
            };

            var reqData = new
            {
                plainTransportOptions.ListenIp,
                plainTransportOptions.Comedia,
                plainTransportOptions.EnableSctp,
                plainTransportOptions.NumSctpStreams,
                plainTransportOptions.MaxSctpMessageSize,
                plainTransportOptions.SctpSendBufferSize,
                IsDataChannel = false,
                plainTransportOptions.EnableSrtp,
                plainTransportOptions.SrtpCryptoSuite
            };

            var status = await _channel.RequestAsync(MethodId.ROUTER_CREATE_PLAIN_TRANSPORT, @internal, reqData);
            var responseData = JsonConvert.DeserializeObject<RouterCreatePlainTransportResponseData>(status!);

            var transport = new PlainTransport(_loggerFactory,
                                new TransportInternalData(@internal.RouterId, @internal.TransportId),
                                sctpParameters: null,
                                sctpState: null,
                                _channel,
                                _payloadChannel,
                                plainTransportOptions.AppData,
                                () => RtpCapabilities,
                                m => _producers.TryGetValue(m, out var p) ? p : null,
                                m => _dataProducers.TryGetValue(m, out var p) ? p : null,
                                responseData.RtcpMux,
                                responseData.Comedia,
                                responseData.Tuple,
                                responseData.RtcpTuple,
                                responseData.SrtpParameters
                            );
            _transports[transport.TransportId] = transport;

            transport.On("@close", _ =>
            {
                _transports.TryRemove(transport.TransportId, out var _);
                return Task.CompletedTask;
            });
            transport.On("@newproducer", obj =>
            {
                var producer = (Producer)obj!;
                _producers[producer.ProducerId] = producer;
                return Task.CompletedTask;
            });
            transport.On("@producerclose", obj =>
            {
                var producer = (Producer)obj!;
                _producers.TryRemove(producer.ProducerId, out var _);
                return Task.CompletedTask;
            });
            transport.On("@newdataproducer", obj =>
            {
                var dataProducer = (DataProducer)obj!;
                _dataProducers[dataProducer.DataProducerId] = dataProducer;
                return Task.CompletedTask;
            });
            transport.On("@dataproducerclose", obj =>
            {
                var dataProducer = (DataProducer)obj!;
                _dataProducers.TryRemove(dataProducer.DataProducerId, out var _);
                return Task.CompletedTask;
            });

            // Emit observer event.
            Observer.Emit("newtransport", transport);

            return transport;
        }

        /// <summary>
        /// Create a PipeTransport.
        /// </summary>
        public async Task<PipeTransport> CreatePipeTransportAsync(PipeTransportOptions pipeTransportOptions)
        {
            _logger.LogDebug("CreatePipeTransportAsync()");

            if (pipeTransportOptions.ListenIp == null)
            {
                throw new NullReferenceException("Missing listenIp");
            }

            var @internal = new
            {
                RouterId,
                TransportId = Guid.NewGuid().ToString(),
            };

            var reqData = new
            {
                pipeTransportOptions.ListenIp,
                pipeTransportOptions.EnableSctp,
                pipeTransportOptions.NumSctpStreams,
                pipeTransportOptions.MaxSctpMessageSize,
                pipeTransportOptions.SctpSendBufferSize,
                IsDataChannel = false,
                pipeTransportOptions.EnableRtx,
                pipeTransportOptions.EnableSrtp,
            };

            var status = await _channel.RequestAsync(MethodId.ROUTER_CREATE_PIPE_TRANSPORT, @internal, reqData);
            var responseData = JsonConvert.DeserializeObject<RouterCreatePipeTransportResponseData>(status!);

            var transport = new PipeTransport(_loggerFactory,
                                new TransportInternalData(@internal.RouterId, @internal.TransportId),
                                sctpParameters: null,
                                sctpState: null,
                                _channel,
                                _payloadChannel,
                                pipeTransportOptions.AppData,
                                () => RtpCapabilities,
                                m => _producers.TryGetValue(m, out var p) ? p : null,
                                m => _dataProducers.TryGetValue(m, out var p) ? p : null,
                                responseData.Tuple,
                                responseData.Rtx,
                                responseData.SrtpParameters
                            );

            _transports[transport.TransportId] = transport;

            transport.On("@close", _ =>
            {
                _transports.TryRemove(transport.TransportId, out var _);
                return Task.CompletedTask;
            });
            transport.On("@newproducer", obj =>
            {
                var producer = (Producer)obj!;
                _producers[producer.ProducerId] = producer;
                return Task.CompletedTask;
            });
            transport.On("@producerclose", obj =>
            {
                var producer = (Producer)obj!;
                _producers.TryRemove(producer.ProducerId, out var _);
                return Task.CompletedTask;
            });
            transport.On("@newdataproducer", obj =>
            {
                var dataProducer = (DataProducer)obj!;
                _dataProducers[dataProducer.DataProducerId] = dataProducer;
                return Task.CompletedTask;
            });
            transport.On("@dataproducerclose", obj =>
            {
                var dataProducer = (DataProducer)obj!;
                _dataProducers.TryRemove(dataProducer.DataProducerId, out var _);
                return Task.CompletedTask;
            });

            // Emit observer event.
            Observer.Emit("newtransport", transport);

            return transport;
        }

        /// <summary>
        /// Create a DirectTransport.
        /// </summary>
        /// <param name="directTransportOptions"></param>
        /// <returns></returns>
        public async Task<DirectTransport> CreateDirectTransportAsync(DirectTransportOptions directTransportOptions)
        {
            _logger.LogDebug("CreateDirectTransportAsync()");

            var @internal = new
            {
                RouterId,
                TransportId = Guid.NewGuid().ToString(),
            };

            var reqData = new
            {
                Direct = true,
                directTransportOptions.MaxMessageSize,
            };

            var status = await _channel.RequestAsync(MethodId.ROUTER_CREATE_DIRECT_TRANSPORT, @internal, reqData);
            var responseData = JsonConvert.DeserializeObject<RouterCreatePlainTransportResponseData>(status!);

            var transport = new DirectTransport(_loggerFactory,
                new TransportInternalData(@internal.RouterId, @internal.TransportId),
                sctpParameters: null,
                sctpState: null,
                _channel,
                _payloadChannel,
                directTransportOptions.AppData,
                () => RtpCapabilities,
                m => _producers.TryGetValue(m, out var p) ? p : null,
                m => _dataProducers.TryGetValue(m, out var p) ? p : null
                );

            _transports[transport.TransportId] = transport;

            transport.On("@close", _ =>
            {
                _transports.TryRemove(transport.TransportId, out var _);
                return Task.CompletedTask;
            });
            transport.On("@newproducer", obj =>
            {
                var producer = (Producer)obj!;
                _producers[producer.ProducerId] = producer;
                return Task.CompletedTask;
            });
            transport.On("@producerclose", obj =>
            {
                var producer = (Producer)obj!;
                _producers.TryRemove(producer.ProducerId, out var _);
                return Task.CompletedTask;
            });
            transport.On("@newdataproducer", obj =>
            {
                var dataProducer = (DataProducer)obj!;
                _dataProducers[dataProducer.DataProducerId] = dataProducer;
                return Task.CompletedTask;
            });
            transport.On("@dataproducerclose", obj =>
            {
                var dataProducer = (DataProducer)obj!;
                _dataProducers.TryRemove(dataProducer.DataProducerId, out var _);
                return Task.CompletedTask;
            });

            // Emit observer event.
            Observer.Emit("newtransport", transport);

            return transport;
        }

        /// <summary>
        /// Pipes the given Producer or DataProducer into another Router in same host.
        /// </summary>
        /// <param name="pipeToRouterOptions">ListenIp 传入 127.0.0.1, EnableSrtp 传入 true 。</param>
        /// <returns></returns>
        public async Task<PipeToRouterResult> PipeToRouterAsync(PipeToRouterOptions pipeToRouterOptions)
        {
            if (pipeToRouterOptions.ListenIp == null)
            {
                throw new NullReferenceException("Missing listenIp");
            }

            if (pipeToRouterOptions.ProducerId.IsNullOrWhiteSpace() && pipeToRouterOptions.DataProducerId.IsNullOrWhiteSpace())
            {
                throw new Exception("Missing producerId or dataProducerId");
            }

            if (!pipeToRouterOptions.ProducerId.IsNullOrWhiteSpace() && !pipeToRouterOptions.DataProducerId.IsNullOrWhiteSpace())
            {
                throw new Exception("Just producerId or dataProducerId can be given");
            }

            if (pipeToRouterOptions.Router == null)
            {
                throw new Exception("Router not found");
            }

            if (pipeToRouterOptions.Router == this)
            {
                throw new Exception("Cannot use this Router as destination");
            }

            Producer? producer = null;
            DataProducer? dataProducer = null;

            if (!pipeToRouterOptions.ProducerId.IsNullOrWhiteSpace())
            {
                if (!_producers.TryGetValue(pipeToRouterOptions.ProducerId!, out producer))
                {
                    throw new Exception("Producer not found");
                }
            }
            else if (!pipeToRouterOptions.DataProducerId.IsNullOrWhiteSpace())
            {
                if (!_dataProducers.TryGetValue(pipeToRouterOptions.DataProducerId!, out dataProducer))
                {
                    throw new Exception("DataProducer not found");
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
                        Ip = remotePipeTransport.Tuple.LocalIp,
                        Port = remotePipeTransport.Tuple.LocalPort,
                        SrtpParameters = remotePipeTransport.SrtpParameters,
                    }),
                    remotePipeTransport.ConnectAsync(new PipeTransportConnectParameters
                    {
                        Ip = localPipeTransport.Tuple.LocalIp,
                        Port = localPipeTransport.Tuple.LocalPort,
                        SrtpParameters = localPipeTransport.SrtpParameters,
                    })
                    );

                    localPipeTransport.Observer.On("close", async _ =>
                    {
                        await remotePipeTransport.CloseAsync();
                        _mapRouterPipeTransports.TryRemove(pipeToRouterOptions.Router, out var _);
                    });

                    remotePipeTransport.Observer.On("close", async _ =>
                    {
                        await localPipeTransport.CloseAsync();
                        _mapRouterPipeTransports.TryRemove(pipeToRouterOptions.Router, out var _);
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
                        Kind = pipeConsumer.Kind,
                        RtpParameters = pipeConsumer.RtpParameters,
                        Paused = pipeConsumer.ProducerPaused,
                        AppData = producer.AppData,
                    });

                    // Pipe events from the pipe Consumer to the pipe Producer.
                    pipeConsumer.Observer.On("close", _ =>
                    {
                        pipeProducer.Close();
                        return Task.CompletedTask;
                    });
                    pipeConsumer.Observer.On("pause", async _ => await pipeProducer.PauseAsync());
                    pipeConsumer.Observer.On("resume", async _ => await pipeProducer.ResumeAsync());

                    // Pipe events from the pipe Producer to the pipe Consumer.
                    pipeProducer.Observer.On("close", async _ =>
                    {
                        await pipeConsumer.CloseAsync();
                    });

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
                        pipeProducer.Close();
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
                        SctpStreamParameters = pipeDataConsumer.SctpStreamParameters,
                        Label = pipeDataConsumer.Label,
                        Protocol = pipeDataConsumer.Protocol,
                        AppData = dataProducer.AppData,
                    });

                    // Pipe events from the pipe DataConsumer to the pipe DataProducer.
                    pipeDataConsumer.Observer.On("close", _ =>
                    {
                        pipeDataProducer.Close();
                        return Task.CompletedTask;
                    });

                    // Pipe events from the pipe DataProducer to the pipe DataConsumer.
                    pipeDataProducer.Observer.On("close", _ =>
                    {
                        pipeDataConsumer.Close();
                        return Task.CompletedTask;
                    });

                    return new PipeToRouterResult { PipeDataConsumer = pipeDataConsumer, PipeDataProducer = pipeDataProducer };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"PipeToRouterAsync() | Create pipe DataConsumer/DataProducer pair failed.");

                    if (pipeDataConsumer != null)
                    {
                        pipeDataConsumer.Close();
                    }

                    if (pipeDataProducer != null)
                    {
                        pipeDataProducer.Close();
                    }

                    throw;
                }
            }
            else
            {
                throw new Exception("Internal error");
            }
        }

        /// <summary>
        /// Create an AudioLevelObserver.
        /// </summary>
        public async Task<AudioLevelObserver> CreateAudioLevelObserverAsync(AudioLevelObserverOptions audioLevelObserverOptions)
        {
            _logger.LogDebug("createAudioLevelObserver()");

            var @internal = new
            {
                RouterId,
                RtpObserverId = Guid.NewGuid().ToString(),
            };

            var reqData = new
            {
                audioLevelObserverOptions.MaxEntries,
                audioLevelObserverOptions.Threshold,
                audioLevelObserverOptions.Interval
            };

            await _channel.RequestAsync(MethodId.ROUTER_CREATE_AUDIO_LEVEL_OBSERVER, @internal, reqData);

            var audioLevelObserver = new AudioLevelObserver(_loggerFactory,
                new RtpObserverInternalData(@internal.RouterId, @internal.RtpObserverId),
                _channel,
                _payloadChannel,
                AppData,
                m => _producers.TryGetValue(m, out var p) ? p : null);

            _rtpObservers[audioLevelObserver.Internal.RtpObserverId] = audioLevelObserver;
            audioLevelObserver.On("@close", _ =>
            {
                _rtpObservers.TryRemove(audioLevelObserver.Internal.RtpObserverId, out var _);
                return Task.CompletedTask;
            });

            // Emit observer event.
            Observer.Emit("newrtpobserver", audioLevelObserver);

            return audioLevelObserver;
        }

        /// <summary>
        /// Check whether the given RTP capabilities can consume the given Producer.
        /// </summary>
        public bool CanConsume(string producerId, RtpCapabilities rtpCapabilities)
        {
            if (!_producers.TryGetValue(producerId, out var producer))
            {
                _logger.LogError($"CanConsume() | Producer with id {producerId} not found");
                return false;
            }

            try
            {
                return ORTC.CanConsume(producer.ConsumableRtpParameters, rtpCapabilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CanConsume() | Unexpected error");
                return false;
            }
        }

        #region IEquatable<T>

        public bool Equals(Router other)
        {
            return RouterId == other.RouterId;
        }

        public override int GetHashCode()
        {
            return RouterId.GetHashCode();
        }

        #endregion IEquatable<T>
    }
}
