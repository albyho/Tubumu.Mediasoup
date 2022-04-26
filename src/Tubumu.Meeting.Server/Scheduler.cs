using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    public class Scheduler
    {
        #region Private Fields

        /// <summary>
        /// Logger factory for create logger.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<Scheduler> _logger;

        private readonly MediasoupOptions _mediasoupOptions;

        private readonly MediasoupServer _mediasoupServer;

        private readonly Dictionary<string, Peer> _peers = new Dictionary<string, Peer>();

        private readonly AsyncReaderWriterLock _peersLock = new AsyncReaderWriterLock();

        private readonly Dictionary<string, Room> _rooms = new Dictionary<string, Room>();

        private readonly AsyncAutoResetEvent _roomsLock = new AsyncAutoResetEvent();

        #endregion Private Fields

        public RtpCapabilities DefaultRtpCapabilities { get; private set; }

        public Scheduler(ILoggerFactory loggerFactory,
            MediasoupOptions mediasoupOptions,
            MediasoupServer mediasoupServer)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<Scheduler>();
            _mediasoupOptions = mediasoupOptions;
            _mediasoupServer = mediasoupServer;

            // 按创建 Route 时一样方式创建 RtpCodecCapabilities
            var rtpCodecCapabilities = mediasoupOptions.MediasoupSettings.RouterSettings.RtpCodecCapabilities;
            // This may throw.
            DefaultRtpCapabilities = ORTC.GenerateRouterRtpCapabilities(rtpCodecCapabilities);

            _roomsLock.Set();
        }

        public async Task<Peer> JoinAsync(string peerId, string connectionId, JoinRequest joinRequest)
        {
            using (await _peersLock.WriteLockAsync())
            {
                if (_peers.TryGetValue(peerId, out var peer))
                {
                    // 客户端多次调用 `Join`
                    if (peer.ConnectionId == connectionId)
                    {
                        throw new PeerJoinedException("PeerJoinAsync()", peerId);
                    }
                }

                peer = new Peer(_loggerFactory,
                    _mediasoupOptions.MediasoupSettings.WebRtcTransportSettings,
                    _mediasoupOptions.MediasoupSettings.PlainTransportSettings,
                    joinRequest.RtpCapabilities,
                    joinRequest.SctpCapabilities,
                    peerId,
                    connectionId,
                    joinRequest.DisplayName,
                    joinRequest.Sources,
                    joinRequest.AppData
                    );

                _peers[peerId] = peer;

                return peer;
            }
        }

        public async Task<LeaveResult?> LeaveAsync(string peerId)
        {
            using (await _peersLock.WriteLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    return null;
                }

                _peers.Remove(peerId);

                return await peer.LeaveAsync();
            }
        }

        public async Task<JoinRoomResult> JoinRoomAsync(string peerId, string connectionId, JoinRoomRequest joinRoomRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("JoinRoomAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                await _roomsLock.WaitAsync();
                try
                {
                    // Room 如果不存在则创建
                    if (!_rooms.TryGetValue(joinRoomRequest.RoomId, out var room))
                    {
                        // Router media codecs.
                        var mediaCodecs = _mediasoupOptions.MediasoupSettings.RouterSettings.RtpCodecCapabilities;

                        // Create a mediasoup Router.
                        var worker = _mediasoupServer.GetWorker();
                        var router = await worker.CreateRouterAsync(new RouterOptions
                        {
                            MediaCodecs = mediaCodecs
                        });
                        if (router == null)
                        {
                            throw new Exception($"PeerJoinAsync() | Worker maybe closed.");
                        }

                        room = new Room(_loggerFactory, router, joinRoomRequest.RoomId, "Default");
                        _rooms[room.RoomId] = room;
                    }

                    var result = await peer.JoinRoomAsync(room);

                    await peer.SetPeerInternalDataAsync(new SetPeerInternalDataRequest
                    {
                        InternalData = new Dictionary<string, object>
                        {
                            { Peer.RoleKey, joinRoomRequest.Role }
                        }
                    });

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "JoinRoomAsync()");
                    throw;
                }
                finally
                {
                    _roomsLock.Set();
                }
            }
        }

        public async Task<LeaveRoomResult> LeaveRoomAsync(string peerId, string connectionId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("LeaveRoomAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.LeaveRoomAsync();
            }
        }

        public async Task<PeerAppDataResult> SetPeerAppDataAsync(string peerId, string connectionId, SetPeerAppDataRequest setPeerAppDataRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("SetPeerAppDataAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.SetPeerAppDataAsync(setPeerAppDataRequest);
            }
        }

        public async Task<PeerAppDataResult> UnsetPeerAppDataAsync(string peerId, string connectionId, UnsetPeerAppDataRequest unsetPeerAppDataRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("UnsetPeerAppDataAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.UnsetPeerAppDataAsync(unsetPeerAppDataRequest);
            }
        }

        public async Task<PeerAppDataResult> ClearPeerAppDataAsync(string peerId, string connectionId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("ClearPeerAppDataAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.ClearPeerAppDataAsync();
            }
        }

        public async Task<PeerInternalDataResult> SetPeerInternalDataAsync(SetPeerInternalDataRequest setPeerInternalDataRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(setPeerInternalDataRequest.PeerId, out var peer))
                {
                    throw new PeerNotExistsException("SetPeerInternalDataAsync()", setPeerInternalDataRequest.PeerId);
                }

                return await peer.SetPeerInternalDataAsync(setPeerInternalDataRequest);
            }
        }

        public async Task<PeerInternalDataResult> GetPeerInternalDataAsync(string peerId, string connectionId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("GetPeerInternalDataAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.GetPeerInternalDataAsync();
            }
        }

        public async Task<PeerInternalDataResult> UnsetPeerInternalDataAsync(UnsetPeerInternalDataRequest unsetPeerInternalDataRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(unsetPeerInternalDataRequest.PeerId, out var peer))
                {
                    throw new PeerNotExistsException("UnsetPeerInternalDataAsync()", unsetPeerInternalDataRequest.PeerId);
                }

                return await peer.UnsetPeerInternalDataAsync(unsetPeerInternalDataRequest);
            }
        }

        public async Task<PeerInternalDataResult> ClearPeerInternalDataAsync(string peerId, string connectionId, string targetPeerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("ClearPeerInternalDataAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                Peer? targetPeer;
                if(peerId == targetPeerId)
                {
                    targetPeer = peer;
                }
                else if (!_peers.TryGetValue(targetPeerId, out targetPeer))
                {
                    throw new PeerNotExistsException("ClearPeerInternalDataAsync()", targetPeerId);
                }

                return await targetPeer.ClearPeerInternalDataAsync();
            }
        }

        public async Task<WebRtcTransport> CreateWebRtcTransportAsync(string peerId, string connectionId, CreateWebRtcTransportRequest createWebRtcTransportRequest, bool isSend)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("CreateWebRtcTransport()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.CreateWebRtcTransportAsync(createWebRtcTransportRequest, isSend);
            }
        }

        public async Task<bool> ConnectWebRtcTransportAsync(string peerId, string connectionId, ConnectWebRtcTransportRequest connectWebRtcTransportRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("ConnectWebRtcTransportAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.ConnectWebRtcTransportAsync(connectWebRtcTransportRequest);
            }
        }

        public async Task<PlainTransport> CreatePlainTransportAsync(string peerId, string connectionId, CreatePlainTransportRequest createPlainTransportRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("CreateWebRtcTransport()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.CreatePlainTransportAsync(createPlainTransportRequest);
            }
        }

        public async Task<PullResult> PullAsync(string peerId, string connectionId, PullRequest pullRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("PullAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                if (!_peers.TryGetValue(pullRequest.PeerId, out var producePeer))
                {
                    throw new PeerNotExistsException("PullAsync()", pullRequest.PeerId);
                }

                var pullResult = await peer.PullAsync(producePeer, pullRequest.Sources);

                return new PullResult
                {
                    ConsumePeer = peer,
                    ProducePeer = producePeer,
                    ExistsProducers = pullResult.ExistsProducers,
                    Sources = pullResult.ProduceSources,
                };
            }
        }

        public async Task<ProduceResult> ProduceAsync(string peerId, string connectionId, ProduceRequest produceRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("ProduceAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                var peerProduceResult = await peer.ProduceAsync(produceRequest);
                if (peerProduceResult == null)
                {
                    throw new Exception($"ProduceAsync() | Peer:{peerId} produce faild.");
                }

                // NOTE: 这里假设了 Room 存在
                var pullPaddingConsumerPeers = new List<Peer>();
                foreach (var item in peerProduceResult.PullPaddings)
                {
                    // 其他 Peer 消费本 Peer
                    if (_peers.TryGetValue(item.ConsumerPeerId, out var consumerPeer))
                    {
                        pullPaddingConsumerPeers.Add(consumerPeer);
                    }
                }

                var produceResult = new ProduceResult
                {
                    ProducerPeer = peer,
                    Producer = peerProduceResult.Producer,
                    PullPaddingConsumerPeers = pullPaddingConsumerPeers.ToArray(),
                };

                return produceResult;
            }
        }

        public async Task<Consumer?> ConsumeAsync(string producerPeerId, string cosumerPeerId, string producerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(producerPeerId, out var producerPeer))
                {
                    throw new PeerNotExistsException("ConsumeAsync()", producerPeerId);
                }
                if (!_peers.TryGetValue(cosumerPeerId, out var cosumerPeer))
                {
                    throw new PeerNotExistsException("ConsumeAsync()", cosumerPeerId);
                }

                // NOTE: 这里假设了 Room 存在
                return await cosumerPeer.ConsumeAsync(producerPeer, producerId);
            }
        }

        public async Task<bool> CloseProducerAsync(string peerId, string connectionId, string producerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("CloseProducerAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.CloseProducerAsync(producerId);
            }
        }

        public async Task<bool> CloseAllProducersAsync(string peerId, string connectionId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("CloseAllProducersAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.CloseAllProducersAsync();
            }
        }

        public async Task<bool> CloseProducerWithSourcesAsync(string peerId, string connectionId, string targetPeerId, IEnumerable<string> sources)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("CloseProducerWithSourcesAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                Peer? targetPeer;
                if (peerId == targetPeerId)
                {
                    targetPeer = peer;
                }
                else if (!_peers.TryGetValue(targetPeerId, out targetPeer))
                {
                    throw new PeerNotExistsException("CloseProducerWithSourcesAsync()", targetPeerId);
                }

                return await targetPeer.CloseProducerWithSourcesAsync(sources);
            }
        }

        public async Task<bool> PauseProducerAsync(string peerId, string connectionId, string producerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("PauseProducerAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.PauseProducerAsync(producerId);
            }
        }

        public async Task<bool> ResumeProducerAsync(string peerId, string connectionId, string producerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("ResumeProducerAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.ResumeProducerAsync(producerId);
            }
        }

        public async Task<bool> CloseConsumerAsync(string peerId, string connectionId, string consumerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("CloseConsumerAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.CloseConsumerAsync(consumerId);
            }
        }

        public async Task<bool> PauseConsumerAsync(string peerId, string connectionId, string consumerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("PauseConsumerAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.PauseConsumerAsync(consumerId);
            }
        }

        public async Task<Consumer> ResumeConsumerAsync(string peerId, string connectionId, string consumerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("ResumeConsumerAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.ResumeConsumerAsync(consumerId);
            }
        }

        public async Task<bool> SetConsumerPreferedLayersAsync(string peerId, string connectionId, SetConsumerPreferedLayersRequest setConsumerPreferedLayersRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("SetConsumerPreferedLayersAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.SetConsumerPreferedLayersAsync(setConsumerPreferedLayersRequest);
            }
        }

        public async Task<bool> SetConsumerPriorityAsync(string peerId, string connectionId, SetConsumerPriorityRequest setConsumerPriorityRequest)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("SetConsumerPriorityAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.SetConsumerPriorityAsync(setConsumerPriorityRequest);
            }
        }

        public async Task<bool> RequestConsumerKeyFrameAsync(string peerId, string connectionId, string consumerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("RequestConsumerKeyFrameAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.RequestConsumerKeyFrameAsync(consumerId);
            }
        }

        public async Task<WebRtcTransportStat?> GetWebRtcTransportStatsAsync(string peerId, string connectionId, string transportId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("GetWebRtcTransportStatsAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.GetWebRtcTransportStatsAsync(transportId);
            }
        }

        public async Task<ProducerStat?> GetProducerStatsAsync(string peerId, string connectionId, string producerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("GetProducerStatsAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.GetProducerStatsAsync(producerId);
            }
        }

        public async Task<ConsumerStat?> GetConsumerStatsAsync(string peerId, string connectionId, string consumerId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("GetConsumerStatsAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.GetConsumerStatsAsync(consumerId);
            }
        }

        public async Task<IceParameters> RestartIceAsync(string peerId, string connectionId, string transportId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("RestartIceAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.RestartIceAsync(transportId);
            }
        }

        public async Task<string[]> GetOtherPeerIdsAsync(string peerId, string connectionId, UserRole? role = null)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("GetOtherPeerIdsAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.GetOtherPeerIdsAsync(role);
            }
        }

        public async Task<Peer[]> GetOtherPeersAsync(string peerId, string connectionId, UserRole? role = null)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("GetOtherPeersAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.GetOtherPeersAsync(role);
            }
        }

        public async Task<UserRole> GetPeerRoleAsync(string peerId, string connectionId)
        {
            using (await _peersLock.ReadLockAsync())
            {
                if (!_peers.TryGetValue(peerId, out var peer))
                {
                    throw new PeerNotExistsException("GetPeerRoleAsync()", peerId);
                }

                CheckConnection(peer, connectionId);

                return await peer.GetRoleAsync();
            }
        }

        private void CheckConnection(Peer peer, string connectionId)
        {
            if (peer.ConnectionId != connectionId)
            {
                throw new DisconnectedException($"New: {connectionId} Old:{peer.ConnectionId}");
            }
        }
    }
}
