using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    public partial class Room : IEquatable<Room>
    {
        public string RoomId { get; }

        public string Name { get; }

        #region IEquatable<T>

        public bool Equals(Room? other)
        {
            if(other is null)
            {
                return false;
            }

            return RoomId == other.RoomId;
        }

        public override bool Equals(object? other)
        {
            return Equals(other as Room);
        }

        public override int GetHashCode()
        {
            return RoomId.GetHashCode();
        }

        #endregion IEquatable<T>
    }

    public partial class Room
    {
        /// <summary>
        /// Logger factory for create logger.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<Room> _logger;

        /// <summary>
        /// Whether the Room is closed.
        /// </summary>
        private bool _closed;

        private readonly AsyncReaderWriterLock _closeLock = new();

        private readonly Dictionary<string, Peer> _peers = new();

        private readonly AsyncReaderWriterLock _peersLock = new();

        public Router Router { get; }

        public AudioLevelObserver AudioLevelObserver { get; }

        //public PassthroughObserver PassthroughObserver { get; }

        public Room(ILoggerFactory loggerFactory,
            Router router,
            AudioLevelObserver audioLevelObserver,
            //PassthroughObserver passthroughObserver,
            string roomId, string name)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<Room>();
            Router = router;
            AudioLevelObserver = audioLevelObserver;
            //PassthroughObserver = passthroughObserver;
            RoomId = roomId;
            Name = name.NullOrWhiteSpaceReplace("Default");
            _closed = false;

            HandleAudioLevelObserver();
        }

        public async Task<JoinRoomResult> PeerJoinAsync(Peer peer)
        {
            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new Exception($"PeerJoinAsync() | RoomId:{RoomId} was closed.");
                }

                using(await _peersLock.WriteLockAsync())
                {
                    if(_peers.ContainsKey(peer.PeerId))
                    {
                        throw new Exception($"PeerJoinAsync() | Peer:{peer.PeerId} was in RoomId:{RoomId} already.");
                    }

                    _peers[peer.PeerId] = peer;

                    return new JoinRoomResult
                    {
                        SelfPeer = peer,
                        Peers = _peers.Values.ToArray(),
                    };
                }
            }
        }

        public async Task<LeaveRoomResult> PeerLeaveAsync(string peerId)
        {
            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new Exception($"PeerLeaveAsync() | RoomId:{RoomId} was closed.");
                }

                using(await _peersLock.WriteLockAsync())
                {
                    if(!_peers.TryGetValue(peerId, out var peer))
                    {
                        throw new Exception($"PeerLeaveAsync() | Peer:{peerId} is not in RoomId:{RoomId}.");
                    }

                    _peers.Remove(peerId);

                    return new LeaveRoomResult
                    {
                        SelfPeer = peer,
                        OtherPeerIds = _peers.Keys.ToArray()
                    };
                }
            }
        }

        public async Task<string[]> GetPeerIdsAsync()
        {
            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new Exception($"GetPeerIdsAsync() | RoomId:{RoomId} was closed.");
                }

                using(await _peersLock.ReadLockAsync())
                {
                    return _peers.Keys.ToArray();
                }
            }
        }

        public async Task<Peer[]> GetPeersAsync()
        {
            using(await _closeLock.ReadLockAsync())
            {
                if(_closed)
                {
                    throw new Exception($"GetPeersAsync() | RoomId:{RoomId} was closed.");
                }

                using(await _peersLock.ReadLockAsync())
                {
                    return _peers.Values.ToArray();
                }
            }
        }

        public async Task CloseAsync()
        {
            using(await _closeLock.WriteLockAsync())
            {
                if(_closed)
                {
                    return;
                }

                _logger.LogDebug("CloseAsync() | RoomId:{RoomId}", RoomId);

                _closed = true;

                await Router.CloseAsync();
            }
        }

        private void HandleAudioLevelObserver()
        {
            AudioLevelObserver.On("volumes", async (_, volumes) =>
            {
                using(await _closeLock.ReadLockAsync())
                {
                    if(_closed)
                    {
                        return;
                    }

                    using(await _peersLock.ReadLockAsync())
                    {
                        foreach(var peer in _peers.Values)
                        {
                            peer.HubClient?.Notify(new MeetingNotification
                            {
                                Type = "activeSpeaker",
                                // TODO: (alby)Strongly typed
                                Data = (volumes as List<AudioLevelObserverVolume>)!.Select(m => new { PeerId = m.Producer.AppData!["peerId"], m.Producer.ProducerId, m.Volume }),
                            }).ContinueWithOnFaultedHandleLog(_logger);
                        }
                    }
                }
            });

            AudioLevelObserver.On("silence", async (_, _) =>
            {
                using(await _closeLock.ReadLockAsync())
                {
                    if(_closed)
                    {
                        return;
                    }

                    using(await _peersLock.ReadLockAsync())
                    {
                        foreach(var peer in _peers.Values)
                        {
                            peer.HubClient?.Notify(new MeetingNotification
                            {
                                Type = "activeSpeaker",
                            }).ContinueWithOnFaultedHandleLog(_logger);
                        }
                    }
                }
            });
        }
    }
}
