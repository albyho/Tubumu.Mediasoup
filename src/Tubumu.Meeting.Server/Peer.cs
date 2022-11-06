using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    public partial class Peer : IEquatable<Peer>
    {
        public string PeerId { get; }

        public string DisplayName { get; }

        public string[] Sources { get; }

        public ConcurrentDictionary<string, object> AppData { get; }

        [JsonIgnore]
        public ConcurrentDictionary<string, object> InternalData { get; }

        #region IEquatable<T>

        public bool Equals(Peer? other)
        {
            if (other is null)
            {
                return false;
            }

            return PeerId == other.PeerId;
        }

        public override bool Equals(object? other)
        {
            return Equals(other as Peer);
        }

        public override int GetHashCode()
        {
            return PeerId.GetHashCode();
        }

        #endregion IEquatable<T>
    }

    public partial class Peer
    {
        /// <summary>
        /// Logger factory for create logger.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<Peer> _logger;

        private bool _joined;

        private readonly AsyncReaderWriterLock _joinedLock = new();

        private readonly WebRtcTransportSettings _webRtcTransportSettings;

        private readonly PlainTransportSettings _plainTransportSettings;

        private readonly RtpCapabilities _rtpCapabilities;

        private readonly SctpCapabilities? _sctpCapabilities;

        private readonly Dictionary<string, Transport> _transports = new();

        private readonly AsyncReaderWriterLock _transportsLock = new();

        private readonly Dictionary<string, Consumer> _consumers = new();

        private readonly AsyncReaderWriterLock _consumersLock = new();

        private readonly Dictionary<string, Producer> _producers = new();

        public async Task<Dictionary<string, Producer>> GetProducersASync()
        {
            using (await _producersLock.ReadLockAsync())
            {
                return _producers;
            }
        }

        private readonly AsyncReaderWriterLock _producersLock = new();

        private readonly Dictionary<string, DataConsumer> _dataConsumers = new();

        private readonly AsyncReaderWriterLock _dataConsumersLock = new();

        private readonly Dictionary<string, DataProducer> _dataProducers = new();

        private readonly AsyncReaderWriterLock __dataProducersLock = new();

        private readonly List<PullPadding> _pullPaddings = new();

        private readonly AsyncAutoResetEvent _pullPaddingsLock = new();

        private Room? _room;

        private readonly AsyncReaderWriterLock _roomLock = new();

        public const string RoleKey = "role";

        [JsonIgnore]
        public string ConnectionId { get; }

        [JsonIgnore]
        public IHubClient? HubClient { get; }

        public Peer(ILoggerFactory loggerFactory,
            WebRtcTransportSettings webRtcTransportSettings,
            PlainTransportSettings plainTransportSettings,
            RtpCapabilities rtpCapabilities,
            SctpCapabilities? sctpCapabilities,
            string peerId,
            string connectionId,
            IHubClient hubClient,
            string displayName,
            string[]? sources,
            Dictionary<string, object>? appData)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Peer>();
            _webRtcTransportSettings = webRtcTransportSettings;
            _plainTransportSettings = plainTransportSettings;
            _rtpCapabilities = rtpCapabilities;
            _sctpCapabilities = sctpCapabilities;
            PeerId = peerId;
            ConnectionId = connectionId;
            HubClient = hubClient;
            DisplayName = displayName.NullOrWhiteSpaceReplace("User:" + peerId.ToString().PadLeft(8, '0'));
            Sources = sources ?? Array.Empty<string>();
            AppData = new ConcurrentDictionary<string, object>(appData ?? new Dictionary<string, object>());
            InternalData = new ConcurrentDictionary<string, object>(appData ?? new Dictionary<string, object>());
            _pullPaddingsLock.Set();
            _joined = true;
        }

        /// <summary>
        /// 创建 WebRtcTransport
        /// </summary>
        /// <param name="createWebRtcTransportRequest"></param>
        /// <returns></returns>
        public async Task<WebRtcTransport> CreateWebRtcTransportAsync(CreateWebRtcTransportRequest createWebRtcTransportRequest, bool isSend)
        {
            var webRtcTransportOptions = new WebRtcTransportOptions
            {
                ListenIps = _webRtcTransportSettings.ListenIps,
                InitialAvailableOutgoingBitrate = _webRtcTransportSettings.InitialAvailableOutgoingBitrate,
                MaxSctpMessageSize = _webRtcTransportSettings.MaxSctpMessageSize,
                EnableSctp = createWebRtcTransportRequest.SctpCapabilities != null,
                NumSctpStreams = createWebRtcTransportRequest.SctpCapabilities?.NumStreams,
                WebRtcServer = null, // TODO: Support WebRtcServer
                AppData = new Dictionary<string, object>
                    {
                        { "Consuming", !isSend },
                        { "Producing", isSend },
                    },
            };

            if (createWebRtcTransportRequest.ForceTcp)
            {
                webRtcTransportOptions.EnableUdp = false;
                webRtcTransportOptions.EnableTcp = true;
            }

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("CreateWebRtcTransportAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("CreateWebRtcTransportAsync()");

                    var transport = await _room!.Router.CreateWebRtcTransportAsync(webRtcTransportOptions);
                    using (await _transportsLock.WriteLockAsync())
                    {
                        if (!isSend && HasConsumingTransport())
                        {
                            throw new Exception("CreateWebRtcTransportAsync() | Consuming transport exists");
                        }

                        if (isSend && HasProducingTransport())
                        {
                            throw new Exception("CreateWebRtcTransportAsync() | Producing transport exists");
                        }

                        // Store the WebRtcTransport into the Peer data Object.
                        _transports[transport.TransportId] = transport;
                    }

                    transport.On("@close", (_, _) =>
                    {
                        // 因为调用 transport.Close() 之前已经使用 _transportsLock 写锁，所以触发该事件的调用从 _transports 移除无需再次加锁。
                        _transports.Remove(transport.TransportId);
                        return Task.CompletedTask;
                    });
                    transport.On("routerclose", async (_, _) =>
                    {
                        using (await _transportsLock.WriteLockAsync())
                        {
                            _transports.Remove(transport.TransportId);
                        }
                    });

                    // If set, apply max incoming bitrate limit.
                    if (_webRtcTransportSettings.MaximumIncomingBitrate.HasValue && _webRtcTransportSettings.MaximumIncomingBitrate.Value > 0)
                    {
                        // Fire and forget
                        transport.SetMaxIncomingBitrateAsync(_webRtcTransportSettings.MaximumIncomingBitrate.Value).ContinueWithOnFaultedHandleLog(_logger);
                    }

                    return transport;
                }
            }
        }

        /// <summary>
        /// 连接 WebRtcTransport
        /// </summary>
        /// <param name="connectWebRtcTransportRequest"></param>
        /// <returns></returns>
        public async Task<bool> ConnectWebRtcTransportAsync(ConnectWebRtcTransportRequest connectWebRtcTransportRequest)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("ConnectWebRtcTransportAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("ConnectWebRtcTransportAsync()");

                    using (await _transportsLock.ReadLockAsync())
                    {
                        if (!_transports.TryGetValue(connectWebRtcTransportRequest.TransportId, out var transport))
                        {
                            throw new Exception($"ConnectWebRtcTransportAsync() | Transport:{connectWebRtcTransportRequest.TransportId} is not exists");
                        }

                        await transport.ConnectAsync(connectWebRtcTransportRequest.DtlsParameters);
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 创建 PlainTransport
        /// </summary>
        /// <param name="createPlainTransportRequest"></param>
        /// <returns></returns>
        public async Task<PlainTransport> CreatePlainTransportAsync(CreatePlainTransportRequest createPlainTransportRequest)
        {
            var plainTransportOptions = new PlainTransportOptions
            {
                ListenIp = _plainTransportSettings.ListenIp,
                MaxSctpMessageSize = _plainTransportSettings.MaxSctpMessageSize,
                RtcpMux = createPlainTransportRequest.RtcpMux, // 一般为 false
                Comedia = createPlainTransportRequest.Comedia, // 一般为 true
                AppData = new Dictionary<string, object>
                    {
                        { "Consuming", createPlainTransportRequest.Consuming },
                        { "Producing", createPlainTransportRequest.Producing },
                    },
            };

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("CreatePlainTransportAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("CreatePlainTransportAsync()");

                    var transport = await _room!.Router.CreatePlainTransportAsync(plainTransportOptions);
                    if (transport == null)
                    {
                        throw new Exception("CreatePlainTransportAsync() | Router.CreatePlainTransport faild");
                    }

                    using (await _transportsLock.WriteLockAsync())
                    {
                        // Store the PlainTransport into the Peer data Object.
                        _transports[transport.TransportId] = transport;
                    }

                    transport.On("@close", (_, _) =>
                    {
                        // 因为调用 transport.CloseAsync() 之前已经使用 _transportsLock 写锁，所以触发该事件的调用从 _transports 移除无需再次加锁。
                        _transports.Remove(transport.TransportId);
                        return Task.CompletedTask;
                    });
                    transport.On("routerclose", async (_, _) =>
                    {
                        using (await _transportsLock.WriteLockAsync())
                        {
                            _transports.Remove(transport.TransportId);
                        }
                    });

                    return transport;
                }
            }
        }

        /// <summary>
        /// 拉取
        /// </summary>
        /// <param name="producerPeer"></param>
        /// <param name="roomId"></param>
        /// <param name="sources"></param>
        /// <returns></returns>
        public async Task<PeerPullResult> PullAsync(Peer producerPeer, IEnumerable<string> sources)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("PullAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("PullAsync()");

                    var consumerRoom = _room!;

                    using (await _consumersLock.ReadLockAsync())
                    {
                        // producerPeer 也有可能是本 Peer
                        if (producerPeer.PeerId == PeerId)
                        {
                            return await PullInternalAsync(producerPeer, sources);
                        }
                        else
                        {
                            using (await producerPeer._joinedLock.ReadLockAsync())
                            {
                                producerPeer.CheckJoined("PullAsync()");

                                using (await producerPeer._roomLock.ReadLockAsync())
                                {
                                    producerPeer.CheckRoom("PullAsync()");

                                    var producerRoom = producerPeer._room!;

                                    if (producerRoom.RoomId != consumerRoom.RoomId)
                                    {
                                        throw new Exception($"PullAsync() | Peer:{producerPeer.PeerId} and Peer:{PeerId} are not in the same room.");
                                    }

                                    if (sources.Except(producerPeer.Sources).Any())
                                    {
                                        throw new Exception($"PullAsync() | Peer:{producerPeer.PeerId} can't produce some sources.");
                                    }

                                    return await PullInternalAsync(producerPeer, sources);
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task<PeerPullResult> PullInternalAsync(Peer producerPeer, IEnumerable<string> sources)
        {
            var consumerActiveRoom = _room!;
            var producerActiveRoom = producerPeer._room!;

            var roomId = consumerActiveRoom.RoomId;

            using (await producerPeer._producersLock.ReadLockAsync())
            {
                var producerProducers = producerPeer._producers.Values.Where(m => sources.Contains(m.Source)).ToArray();

                var existsProducers = new HashSet<Producer>();
                var produceSources = new HashSet<string>();
                foreach (var source in sources)
                {
                    foreach (var existsProducer in producerProducers)
                    {
                        // 忽略重复消费
                        if (_consumers.Values.Any(m => m.ProducerId == existsProducer.ProducerId))
                        {
                            continue;
                        }
                        existsProducers.Add(existsProducer);
                        continue;
                    }

                    await producerPeer._pullPaddingsLock.WaitAsync();
                    // 如果 Source 没有对应的 Producer，通知 otherPeer 生产；生产成功后又要通知本 Peer 去对应的 Room 消费。
                    if (!producerPeer._pullPaddings.Any(m => m.Source == source))
                    {
                        produceSources.Add(source);
                    }
                    if (!producerPeer._pullPaddings.Any(m => m.Source == source && m.RoomId == roomId && m.ConsumerPeerId == PeerId))
                    {
                        producerPeer._pullPaddings.Add(new PullPadding
                        {
                            RoomId = roomId,
                            ConsumerPeerId = PeerId,
                            Source = source,
                        });
                    }
                    producerPeer._pullPaddingsLock.Set();
                }

                return new PeerPullResult
                {
                    ExistsProducers = existsProducers.ToArray(),
                    ProduceSources = produceSources,
                };
            }
        }

        /// <summary>
        /// 生产
        /// </summary>
        /// <param name="produceRequest"></param>
        /// <returns></returns>
        public async Task<PeerProduceResult> ProduceAsync(ProduceRequest produceRequest)
        {
            if (produceRequest.Source.IsNullOrWhiteSpace())
            {
                throw new Exception($"ProduceAsync() | Peer:{PeerId} AppData[\"source\"] is null or white space.");
            }

            if (Sources == null || !Sources.Contains(produceRequest.Source))
            {
                throw new Exception($"ProduceAsync() | Source:\"{produceRequest.Source}\" cannot be produce.");
            }

            // Add peerId into appData to later get the associated Peer during
            // the 'loudest' event of the audioLevelObserver.
            produceRequest.AppData ??= new Dictionary<string, object>();
            produceRequest.AppData["peerId"] = PeerId;

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("ProduceAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("ProduceAsync()");

                    using (await _transportsLock.ReadLockAsync())
                    {
                        var transport = GetProducingTransport();
                        if (transport == null)
                        {
                            throw new Exception($"ProduceAsync() | Transport:Producing is not exists.");
                        }

                        using (await _producersLock.WriteLockAsync())
                        {
                            var producer = _producers.Values.FirstOrDefault(m => m.Source == produceRequest.Source);
                            if (producer != null)
                            {
                                //throw new Exception($"ProduceAsync() | Source:\"{ produceRequest.Source }\" is exists.");
                                _logger.LogWarning($"ProduceAsync() | Source:\"{produceRequest.Source}\" is exists.");
                                return new PeerProduceResult
                                {
                                    Producer = producer,
                                    PullPaddings = Array.Empty<PullPadding>(),
                                };
                            }

                            producer = await transport.ProduceAsync(new ProducerOptions
                            {
                                Kind = produceRequest.Kind,
                                RtpParameters = produceRequest.RtpParameters,
                                AppData = produceRequest.AppData,
                            });

                            // Store producer source
                            producer.Source = produceRequest.Source;

                            producer.On("@close", async (_, _) =>
                            {
                                // 因为调用 producer.Close() 之前已经使用 _producersLock 写锁，所以触发该事件的调用从 _producers 移除无需再次加锁。
                                _producers.Remove(producer.ProducerId);

                                await _pullPaddingsLock.WaitAsync();
                                _pullPaddings.Clear();
                                _pullPaddingsLock.Set();
                            });
                            producer.On("transportclose", async (_, _) =>
                            {
                                using (await _producersLock.WriteLockAsync())
                                {
                                    _producers.Remove(producer.ProducerId);
                                }

                                await _pullPaddingsLock.WaitAsync();
                                _pullPaddings.Clear();
                                _pullPaddingsLock.Set();
                            });

                            await _pullPaddingsLock.WaitAsync();
                            var matchedPullPaddings = _pullPaddings.Where(m => m.Source == producer.Source).ToArray();
                            foreach (var item in matchedPullPaddings)
                            {
                                _pullPaddings.Remove(item);
                            }
                            _pullPaddingsLock.Set();

                            // Store the Producer into the Peer data Object.
                            _producers[producer.ProducerId] = producer;

                            // Add into the audioLevelObserver.
                            if (producer.Data.Kind == MediaKind.Audio)
                            {
                                // Fire and forget
                                _room!.AudioLevelObserver.AddProducerAsync(new RtpObserverAddRemoveProducerOptions
                                {
                                    ProducerId = producer.ProducerId,
                                }).ContinueWithOnFaultedHandleLog(_logger);
                            }

                            return new PeerProduceResult
                            {
                                Producer = producer,
                                PullPaddings = matchedPullPaddings,
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 消费
        /// </summary>
        /// <param name="producer"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async Task<Consumer?> ConsumeAsync(Peer producerPeer, string producerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("ConsumeAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("ConsumeAsync()");

                    using (await _transportsLock.ReadLockAsync())
                    {
                        var transport = GetConsumingTransport();

                        // This should not happen.
                        if (transport == null)
                        {
                            throw new Exception($"ConsumeAsync() | Peer:{PeerId} Transport for consuming not found.");
                        }

                        using (await _consumersLock.WriteLockAsync())
                        {
                            // 已经在消费
                            if (_consumers.Any(m => m.Value.ProducerId == producerId))
                            {
                                return null;
                            }

                            using (await producerPeer._producersLock.ReadLockAsync())
                            {
                                if (!producerPeer._producers.TryGetValue(producerId, out var producer))
                                {
                                    throw new Exception($"ConsumeAsync() | Peer:{PeerId} - ProducerPeer:{producerPeer.PeerId} has no Producer:{producerId}");
                                }

                                if (_rtpCapabilities == null || !await _room!.Router.CanConsumeAsync(producer.ProducerId, _rtpCapabilities))
                                {
                                    throw new Exception($"ConsumeAsync() | Peer:{PeerId} Can not consume.");
                                }

                                // Create the Consumer in paused mode.
                                var consumer = await transport.ConsumeAsync(new ConsumerOptions
                                {
                                    ProducerId = producer.ProducerId,
                                    RtpCapabilities = _rtpCapabilities,
                                    Paused = true // Or: producer.Kind == MediaKind.Video
                                });

                                consumer.Source = producer.Source;

                                consumer.On("@close", async (_, _) =>
                                {
                                    // 因为调用 consumer.CloseAsync() 之前已经使用 _consumersLock 写锁，所以触发该事件的调用从 _consumers 移除无需再次加锁。
                                    _consumers.Remove(consumer.ConsumerId);
                                    await producer.RemoveConsumerAsync(consumer.ConsumerId);
                                });
                                consumer.On("producerclose,transportclose", async (_, _) =>
                                {
                                    using (await _consumersLock.WriteLockAsync())
                                    {
                                        _consumers.Remove(consumer.ConsumerId);
                                    }
                                    await producer.RemoveConsumerAsync(consumer.ConsumerId);
                                });

                                await producer.AddConsumerAsync(consumer);

                                // Store the Consumer into the consumerPeer data Object.
                                _consumers[consumer.ConsumerId] = consumer;

                                return consumer;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 停止生产
        /// </summary>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public async Task<bool> CloseProducerAsync(string producerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("CloseProducerAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("CloseProducerAsync()");

                    // NOTE: 因为 Close 会触发 Emit("@close")，而 @close 的事件处理需要写锁。故使用写锁。
                    using (await _producersLock.WriteLockAsync())
                    {
                        if (!_producers.TryGetValue(producerId, out var producer))
                        {
                            throw new Exception($"CloseProducerAsync() | Peer:{PeerId} has no Producer:{producerId}.");
                        }

                        await producer.CloseAsync();
                        return true;
                    }
                }
            }
        }


        /// <summary>
        /// 停止生产全部
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CloseAllProducersAsync()
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("CloseAllProducersAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("CloseAllProducersAsync()");

                    // NOTE: 因为 Close 会触发 Emit("@close")，而 @close 的事件处理需要写锁。故使用写锁。
                    using (await _producersLock.WriteLockAsync())
                    {
                        var producers = _producers.Values.ToArray();
                        foreach (var producer in producers)
                        {
                           await producer.CloseAsync();
                        }

                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 停止生产指定 Source
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CloseProducerWithSourcesAsync(IEnumerable<string> sources)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("CloseProducerWithSourcesAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("CloseProducerWithSourcesAsync()");

                    // NOTE: 因为 Close 会触发 Emit("@close")，而 @close 的事件处理需要写锁。故使用写锁。
                    using (await _producersLock.WriteLockAsync())
                    {
                        var producers = _producers.Values.Where(m => sources.Contains(m.Source)).ToArray();
                        foreach (var producer in producers)
                        {
                            await producer.CloseAsync();
                        }

                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 暂停生产
        /// </summary>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public async Task<bool> PauseProducerAsync(string producerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("PauseProducerAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("PauseProducerAsync()");

                    using (await _producersLock.ReadLockAsync())
                    {
                        if (!_producers.TryGetValue(producerId, out var producer))
                        {
                            throw new Exception($"PauseProducerAsync() | Peer:{PeerId} has no Producer:{producerId}.");
                        }

                        await producer.PauseAsync();
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 恢复生产
        /// </summary>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public async Task<bool> ResumeProducerAsync(string producerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("ResumeProducerAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("ResumeProducerAsync()");

                    using (await _producersLock.ReadLockAsync())
                    {
                        if (!_producers.TryGetValue(producerId, out var producer))
                        {
                            throw new Exception($"ResumeProducerAsync() | Peer:{PeerId} has no Producer:{producerId}.");
                        }

                        await producer.ResumeAsync();
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 停止消费
        /// </summary>
        /// <param name="consumerId"></param>
        /// <returns></returns>
        public async Task<bool> CloseConsumerAsync(string consumerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("CloseConsumerAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("CloseConsumerAsync()");

                    // NOTE: 因为 Close 会触发 Emit("@close")，而 @close 的事件处理需要写锁。故使用写锁。
                    using (await _consumersLock.WriteLockAsync())
                    {
                        if (!_consumers.TryGetValue(consumerId, out var consumer))
                        {
                            throw new Exception($"CloseConsumerAsync() | Peer:{PeerId} has no Cmonsumer:{consumerId}.");
                        }

                        await consumer.CloseAsync();
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 暂停消费
        /// </summary>
        /// <param name="consumerId"></param>
        /// <returns></returns>
        public async Task<bool> PauseConsumerAsync(string consumerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("PauseConsumerAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("PauseConsumerAsync()");

                    using (await _consumersLock.ReadLockAsync())
                    {
                        if (!_consumers.TryGetValue(consumerId, out var consumer))
                        {
                            throw new Exception($"PauseConsumerAsync() | Peer:{PeerId} has no Consumer:{consumerId}.");
                        }

                        await consumer.PauseAsync();
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 恢复消费
        /// </summary>
        /// <param name="consumerId"></param>
        /// <returns></returns>
        public async Task<Consumer> ResumeConsumerAsync(string consumerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("ResumeConsumerAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("ResumeConsumerAsync()");

                    using (await _consumersLock.ReadLockAsync())
                    {
                        if (!_consumers.TryGetValue(consumerId, out var consumer))
                        {
                            throw new Exception($"ResumeConsumerAsync() | Peer:{PeerId} has no Consumer:{consumerId}.");
                        }

                        await consumer.ResumeAsync();
                        return consumer;
                    }
                }
            }
        }

        /// <summary>
        /// 设置消费建议 Layers
        /// </summary>
        /// <param name="setConsumerPreferedLayersRequest"></param>
        /// <returns></returns>
        public async Task<bool> SetConsumerPreferedLayersAsync(SetConsumerPreferedLayersRequest setConsumerPreferedLayersRequest)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("SetConsumerPreferedLayersAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("SetConsumerPreferedLayersAsync()");

                    using (await _consumersLock.ReadLockAsync())
                    {
                        if (!_consumers.TryGetValue(setConsumerPreferedLayersRequest.ConsumerId, out var consumer))
                        {
                            throw new Exception($"SetConsumerPreferedLayersAsync() | Peer:{PeerId} has no Consumer:{setConsumerPreferedLayersRequest.ConsumerId}.");
                        }

                        await consumer.SetPreferredLayersAsync(setConsumerPreferedLayersRequest);
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 设置消费 Priority
        /// </summary>
        /// <param name="setConsumerPriorityRequest"></param>
        /// <returns></returns>
        public async Task<bool> SetConsumerPriorityAsync(SetConsumerPriorityRequest setConsumerPriorityRequest)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("SetConsumerPriorityAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("SetConsumerPriorityAsync()");

                    using (await _consumersLock.ReadLockAsync())
                    {
                        if (!_consumers.TryGetValue(setConsumerPriorityRequest.ConsumerId, out var consumer))
                        {
                            throw new Exception($"SetConsumerPriorityAsync() | Peer:{PeerId} has no Consumer:{setConsumerPriorityRequest.ConsumerId}.");
                        }

                        await consumer.SetPriorityAsync(setConsumerPriorityRequest.Priority);
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 请求关键帧
        /// </summary>
        /// <param name="consumerId"></param>
        /// <returns></returns>
        public async Task<bool> RequestConsumerKeyFrameAsync(string consumerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("RequestConsumerKeyFrameAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("RequestConsumerKeyFrameAsync()");

                    using (await _consumersLock.ReadLockAsync())
                    {
                        if (!_consumers.TryGetValue(consumerId, out var consumer))
                        {
                            throw new Exception($"RequestConsumerKeyFrameAsync() | Peer:{PeerId} has no Producer:{consumerId}.");
                        }

                        await consumer.RequestKeyFrameAsync();
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 获取 WebRtcTransport 状态
        /// </summary>
        /// <param name="transportId"></param>
        /// <returns></returns>
        public async Task<WebRtcTransportStat> GetWebRtcTransportStatsAsync(string transportId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("GetWebRtcTransportStatsAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("GetWebRtcTransportStatsAsync()");

                    using (await _transportsLock.ReadLockAsync())
                    {
                        if (_transports.TryGetValue(transportId, out var transport))
                        {
                            throw new Exception($"GetWebRtcTransportStatsAsync() | Peer:{PeerId} has no Transport:{transportId}.");
                        }

                        var stats = await transport!.GetStatsAsync();
                        // TODO: (alby) 考虑不进行反序列化
                        // TransportStat 系列包括：WebRtcTransportStat、PlainTransportStat、PipeTransportStat 和 DirectTransportStat。
                        return JsonSerializer.Deserialize<WebRtcTransportStat>(stats, ObjectExtensions.DefaultJsonSerializerOptions)!;
                    }
                }
            }
        }

        /// <summary>
        /// 获取生产者状态
        /// </summary>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public async Task<ProducerStat> GetProducerStatsAsync(string producerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("GetProducerStatsAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("GetProducerStatsAsync()");

                    using (await _producersLock.ReadLockAsync())
                    {
                        if (!_producers.TryGetValue(producerId, out var producer))
                        {
                            throw new Exception($"GetProducerStatsAsync() | Peer:{PeerId} has no Producer:{producerId}.");
                        }

                        var stats = await producer.GetStatsAsync();
                        // TODO: (alby) 考虑不进行反序列化
                        return JsonSerializer.Deserialize<ProducerStat>(stats, ObjectExtensions.DefaultJsonSerializerOptions)!;
                    }
                }
            }
        }

        /// <summary>
        /// 获取消费者状态
        /// </summary>
        /// <param name="consumerId"></param>
        /// <returns></returns>
        public async Task<ConsumerStat> GetConsumerStatsAsync(string consumerId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("GetConsumerStatsAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("GetConsumerStatsAsync()");

                    using (await _consumersLock.ReadLockAsync())
                    {
                        if (!_consumers.TryGetValue(consumerId, out var consumer))
                        {
                            throw new Exception($"GetConsumerStatsAsync() | Peer:{PeerId} has no Consumer:{consumerId}.");
                        }

                        var stats = await consumer.GetStatsAsync();
                        // TODO: (alby) 考虑不进行反序列化
                        return JsonSerializer.Deserialize<ConsumerStat>(stats, ObjectExtensions.DefaultJsonSerializerOptions)!;
                    }
                }
            }
        }

        /// <summary>
        /// 重置 Ice
        /// </summary>
        /// <param name="transportId"></param>
        /// <returns></returns>
        public async Task<IceParameters> RestartIceAsync(string transportId)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("RestartIceAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("RestartIceAsync()");

                    using (await _transportsLock.ReadLockAsync())
                    {
                        if (_transports.TryGetValue(transportId, out var transport))
                        {
                            throw new Exception($"RestartIceAsync() | Peer:{PeerId} has no Transport:{transportId}.");
                        }

                        if (transport is not WebRtcTransport webRtcTransport)
                        {
                            throw new Exception($"RestartIceAsync() | Peer:{PeerId} Transport:{transportId} is not WebRtcTransport.");
                        }

                        var iceParameters = await webRtcTransport.RestartIceAsync();
                        return iceParameters;
                    }
                }
            }
        }

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public async Task<JoinRoomResult> JoinRoomAsync(Room room)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("JoinRoomAsync()");

                using (await _roomLock.WriteLockAsync())
                {
                    if (_room != null)
                    {
                        throw new PeerInRoomException($"JoinRoomAsync()", PeerId, room.RoomId);
                    }

                    _room = room;

                    return await _room.PeerJoinAsync(this);
                }
            }
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="roomId"></param>
        public async Task<LeaveRoomResult> LeaveRoomAsync()
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("LeaveRoomAsync()");

                using (await _roomLock.WriteLockAsync())
                {
                    CheckRoom("LeaveRoomAsync()");

                    // NOTE: 因为 Close 会触发 Emit("@close")，而 @close 的事件处理需要写锁。故使用写锁。
                    using (await _transportsLock.WriteLockAsync())
                    {
                        // Iterate and close all mediasoup Transport associated to this Peer, so all
                        // its Producers and Consumers will also be closed.
                        foreach (var transport in _transports.Values)
                        {
                            await transport.CloseAsync();
                        }
                    }

                    var result = await _room!.PeerLeaveAsync(PeerId);
                    _room = null;
                    return result;
                }
            }
        }

        /// <summary>
        /// 离开
        /// </summary>
        public async Task<LeaveResult> LeaveAsync()
        {
            var leaveResult = new LeaveResult
            {
                SelfPeer = this,
                OtherPeerIds = Array.Empty<string>(),
            };

            if (!_joined)
            {
                return leaveResult;
            }

            using (await _joinedLock.WriteLockAsync())
            {
                if (!_joined)
                {
                    return leaveResult;
                }

                _joined = false;

                using (await _roomLock.WriteLockAsync())
                {
                    // NOTE: 因为 Close 会触发 Emit("@close")，而 @close 的事件处理需要写锁。故使用写锁。
                    using (await _transportsLock.WriteLockAsync())
                    {
                        // Iterate and close all mediasoup Transport associated to this Peer, so all
                        // its Producers and Consumers will also be closed.
                        foreach (var transport in _transports.Values)
                        {
                            await transport.CloseAsync();
                        }
                    }

                    if (_room != null)
                    {
                        var leaveRoomResult = await _room.PeerLeaveAsync(PeerId);
                        leaveResult.OtherPeerIds = leaveRoomResult.OtherPeerIds;
                        _room = null;
                    }

                    return leaveResult;
                }
            }
        }

        /// <summary>
        /// 设置 AppData
        /// </summary>
        /// <param name="setPeerAppDataRequest"></param>
        /// <returns></returns>
        public async Task<PeerAppDataResult> SetPeerAppDataAsync(SetPeerAppDataRequest setPeerAppDataRequest)
        {
            var peerAppDataResult = new PeerAppDataResult
            {
                SelfPeerId = PeerId,
                OtherPeerIds = Array.Empty<string>(),
            };

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("SetPeerAppDataAsync()");

                foreach (var item in setPeerAppDataRequest.AppData)
                {
                    AppData.AddOrUpdate(item.Key, item.Value, (oldKey, oldValue) => item.Value);
                }

                peerAppDataResult.AppData = AppData.ToDictionary(x => x.Key, x => x.Value);

                using (await _roomLock.ReadLockAsync())
                {
                    var allPeerIds = await GetPeerIdsInteralAsync();
                    peerAppDataResult.OtherPeerIds = allPeerIds.Where(m => m != PeerId).ToArray();
                    return peerAppDataResult;
                }
            }
        }

        /// <summary>
        /// 移除 AppData
        /// </summary>
        /// <param name="unsetPeerAppDataRequest"></param>
        /// <returns></returns>
        public async Task<PeerAppDataResult> UnsetPeerAppDataAsync(UnsetPeerAppDataRequest unsetPeerAppDataRequest)
        {
            var peerAppDataResult = new PeerAppDataResult
            {
                SelfPeerId = PeerId,
                OtherPeerIds = Array.Empty<string>(),
            };

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("UnsetPeerAppDataAsync()");

                foreach (var item in unsetPeerAppDataRequest.Keys)
                {
                    AppData.TryRemove(item, out var _);
                }

                peerAppDataResult.AppData = AppData.ToDictionary(x => x.Key, x => x.Value);

                using (await _roomLock.ReadLockAsync())
                {
                    var allPeerIds = await GetPeerIdsInteralAsync();
                    peerAppDataResult.OtherPeerIds = allPeerIds.Where(m => m != PeerId).ToArray();
                    return peerAppDataResult;
                }
            }
        }

        /// <summary>
        /// 清空 AppData
        /// </summary>
        /// <returns></returns>
        public async Task<PeerAppDataResult> ClearPeerAppDataAsync()
        {
            var peerAppDataResult = new PeerAppDataResult
            {
                SelfPeerId = PeerId,
                OtherPeerIds = Array.Empty<string>(),
                AppData = new Dictionary<string, object>(),
            };

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("ClearPeerAppDataAsync()");

                AppData.Clear();

                using (await _roomLock.ReadLockAsync())
                {
                    var allPeerIds = await GetPeerIdsInteralAsync();
                    peerAppDataResult.OtherPeerIds = allPeerIds.Where(m => m != PeerId).ToArray();
                    return peerAppDataResult;
                }
            }
        }

        /// <summary>
        /// 设置 InternalData
        /// </summary>
        /// <param name="setPeerInternalDataRequest"></param>
        /// <returns></returns>
        public async Task<PeerInternalDataResult> SetPeerInternalDataAsync(SetPeerInternalDataRequest setPeerInternalDataRequest)
        {
            var peerInternalDataResult = new PeerInternalDataResult();

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("SetPeerInternalDataAsync()");

                foreach (var item in setPeerInternalDataRequest.InternalData)
                {
                    InternalData.AddOrUpdate(item.Key, item.Value, (oldKey, oldValue) => item.Value);
                }

                peerInternalDataResult.InternalData = InternalData.ToDictionary(x => x.Key, x => x.Value);
                return peerInternalDataResult;
            }
        }

        /// <summary>
        /// 获取 InternalData
        /// </summary>
        /// <param name="setPeerInternalDataRequest"></param>
        /// <returns></returns>
        public async Task<PeerInternalDataResult> GetPeerInternalDataAsync()
        {
            var peerInternalDataResult = new PeerInternalDataResult();

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("GetPeerInternalDataAsync()");

                peerInternalDataResult.InternalData = InternalData.ToDictionary(x => x.Key, x => x.Value);
                return peerInternalDataResult;
            }
        }

        /// <summary>
        /// 移除 InternalData
        /// </summary>
        /// <param name="unsetPeerInternalDataRequest"></param>
        /// <returns></returns>
        public async Task<PeerInternalDataResult> UnsetPeerInternalDataAsync(UnsetPeerInternalDataRequest unsetPeerInternalDataRequest)
        {
            var peerInternalDataResult = new PeerInternalDataResult();

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("UnsetPeerInternalDataAsync()");

                foreach (var item in unsetPeerInternalDataRequest.Keys)
                {
                    AppData.TryRemove(item, out var _);
                }

                peerInternalDataResult.InternalData = InternalData.ToDictionary(x => x.Key, x => x.Value);
                return peerInternalDataResult;
            }
        }

        /// <summary>
        /// 清空 InternalData
        /// </summary>
        /// <returns></returns>
        public async Task<PeerInternalDataResult> ClearPeerInternalDataAsync()
        {
            var peerInternalDataResult = new PeerInternalDataResult
            {
                InternalData = new Dictionary<string, object>()
            };

            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("ClearPeerInternalDataAsync()");

                InternalData.Clear();

                return peerInternalDataResult;
            }
        }

        /// <summary>
        /// 获取 Room 内其他 Peer 的 Id
        /// </summary>
        public async Task<string[]> GetOtherPeerIdsAsync(UserRole? role = null)
        {
            var peers = await GetOtherPeersAsync(role);
            return peers.Select(m => m.PeerId).ToArray();
        }

        /// <summary>
        /// 获取 Room 内其他 Peer
        /// </summary>
        public async Task<Peer[]> GetOtherPeersAsync(UserRole? role = null)
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("GetOtherPeersAsync()");

                using (await _roomLock.ReadLockAsync())
                {
                    CheckRoom("GetOtherPeersAsync()");

                    var allPeers = await GetPeersInteralAsync();
                    var query = allPeers.Where(m => m.PeerId != PeerId);
                    if (role.HasValue)
                    {
                        query = query.Where(m => m.InternalData.TryGetValue(RoleKey, out var r) && r.GetType() == typeof(UserRole) && (UserRole)r == role.Value);
                    }
                    return query.ToArray();
                }
            }
        }

        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <returns></returns>
        public async Task<UserRole> GetRoleAsync()
        {
            using (await _joinedLock.ReadLockAsync())
            {
                CheckJoined("GetRoleAsync()");

                return InternalData.TryGetValue(RoleKey, out var role) && role.GetType() == typeof(UserRole) ? (UserRole)role : UserRole.Normal;
            }
        }

        #region Private Methods

        private Transport? GetProducingTransport()
        {
            return _transports.Values.FirstOrDefault(m => m.AppData != null && m.AppData.TryGetValue("Producing", out var value) && (bool)value);
        }

        private Transport? GetConsumingTransport()
        {
            return _transports.Values.FirstOrDefault(m => m.AppData != null && m.AppData.TryGetValue("Consuming", out var value) && (bool)value);
        }

        private bool HasProducingTransport()
        {
            return _transports.Values.Any(m => m.AppData != null && m.AppData.TryGetValue("Producing", out var value) && (bool)value);
        }

        private bool HasConsumingTransport()
        {
            return _transports.Values.Any(m => m.AppData != null && m.AppData.TryGetValue("Consuming", out var value) && (bool)value);
        }

        private void CheckJoined(string tag)
        {
            if (!_joined)
            {
                throw new PeerNotJoinedException(tag, PeerId);
            }
        }

        private void CheckRoom(string tag)
        {
            if (_room == null)
            {
                throw new PeerNotInRoomException(tag, PeerId);
            }
        }

        private async Task<string[]> GetPeerIdsInteralAsync()
        {
            return _room != null ? await _room.GetPeerIdsAsync() : Array.Empty<string>();
        }

        private async Task<Peer[]> GetPeersInteralAsync()
        {
            return _room != null ? await _room.GetPeersAsync() : Array.Empty<Peer>();
        }

        #endregion Private Methods
    }
}
