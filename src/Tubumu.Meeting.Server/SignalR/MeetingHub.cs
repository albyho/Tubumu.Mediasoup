using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBS.RtpParameters;
using FBS.WebRtcTransport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tubumu.Mediasoup;

namespace Tubumu.Meeting.Server
{
    [Authorize]
    public partial class MeetingHub : Hub<IHubClient>
    {
        private readonly ILogger<MeetingHub> _logger;

        private readonly IHubContext<MeetingHub, IHubClient> _hubContext;

        private readonly BadDisconnectSocketService _badDisconnectSocketService;

        private readonly Scheduler _scheduler;

        private readonly MeetingServerOptions _meetingServerOptions;

        private string UserId => Context.UserIdentifier!;

        private string ConnectionId => Context.ConnectionId;

        public MeetingHub(ILogger<MeetingHub> logger,
            IHubContext<MeetingHub, IHubClient> hubContext,
            BadDisconnectSocketService badDisconnectSocketService,
            Scheduler scheduler,
            MeetingServerOptions meetingServerOptions)
        {
            _logger = logger;
            _hubContext = hubContext;
            _badDisconnectSocketService = badDisconnectSocketService;
            _scheduler = scheduler;
            _meetingServerOptions = meetingServerOptions;
        }

        public override async Task OnConnectedAsync()
        {
            await LeaveAsync();
            _badDisconnectSocketService.CacheContext(Context);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await LeaveAsync();
            await base.OnDisconnectedAsync(exception);
        }

        #region Private

        private async Task LeaveAsync()
        {
            try
            {
                var leaveResult = await _scheduler.LeaveAsync(UserId);
                if(leaveResult != null)
                {
                    // Notification: peerLeaveRoom
                    SendNotification(leaveResult.OtherPeerIds, "peerLeaveRoom", new PeerLeaveRoomNotification
                    {
                        PeerId = leaveResult.SelfPeer.PeerId
                    });
                    _badDisconnectSocketService.DisconnectClient(leaveResult.SelfPeer.ConnectionId);
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("LeaveAsync 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "LeaveAsync 调用失败.");
            }
        }

        #endregion Private
    }

    public partial class MeetingHub
    {
        public MeetingMessage<object> GetServeMode()
        {
            return MeetingMessage<object>.Success(new { _meetingServerOptions.ServeMode }, "GetServeMode 成功");
        }

        #region Room

        /// <summary>
        /// Get RTP capabilities of router.
        /// </summary>
        ///
        public MeetingMessage<RtpCapabilities> GetRouterRtpCapabilities()
        {
            var rtpCapabilities = _scheduler.DefaultRtpCapabilities;
            return MeetingMessage<RtpCapabilities>.Success(rtpCapabilities, "GetRouterRtpCapabilities 成功");
        }

        /// <summary>
        /// Join meeting.
        /// </summary>
        public async Task<MeetingMessage> Join(JoinRequest joinRequest)
        {
            try
            {
                var client = _hubContext.Clients.User(UserId);
                await _scheduler.JoinAsync(UserId, ConnectionId, client, joinRequest);
                return MeetingMessage.Success("Join 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("Join 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Join 调用失败.");
            }

            return MeetingMessage.Failure("Join 失败");
        }

        /// <summary>
        /// Join room.
        /// </summary>
        public async Task<MeetingMessage<JoinRoomResponse>> JoinRoom(JoinRoomRequest joinRoomRequest)
        {
            try
            {
                // FIXME: (alby) 明文告知用户进入房间的 Role 存在安全问题, 特别是 Invite 模式下。
                var joinRoomResult = await _scheduler.JoinRoomAsync(UserId, ConnectionId, joinRoomRequest);

                // 将自身的信息告知给房间内的其他人
                var otherPeerIds = joinRoomResult.Peers.Select(m => m.PeerId).Where(m => m != joinRoomResult.SelfPeer.PeerId).ToArray();
                // Notification: peerJoinRoom
                SendNotification(otherPeerIds, "peerJoinRoom", new PeerJoinRoomNotification
                {
                    Peer = joinRoomResult.SelfPeer
                });

                // 返回包括自身的房间内的所有人的信息
                var data = new JoinRoomResponse
                {
                    Peers = joinRoomResult.Peers,
                };
                return MeetingMessage<JoinRoomResponse>.Success(data, "JoinRoom 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("JoinRoom 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "JoinRoom 调用失败.");
            }

            return MeetingMessage<JoinRoomResponse>.Failure("JoinRoom 失败");
        }

        /// <summary>
        /// Leave room.
        /// </summary>
        ///
        public async Task<MeetingMessage> LeaveRoom()
        {
            try
            {
                // FIXME: (alby) 在 Invite 模式下，清除尚未处理的邀请。避免在会议室A受邀请后，离开会议室A进入会议室B，误受邀请。
                var leaveRoomResult = await _scheduler.LeaveRoomAsync(UserId, ConnectionId);

                // Notification: peerLeaveRoom
                SendNotification(leaveRoomResult.OtherPeerIds, "peerLeaveRoom", new PeerLeaveRoomNotification
                {
                    PeerId = UserId
                });

                return MeetingMessage.Success("LeaveRoom 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("LeaveRoom 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "LeaveRoom 调用失败.");
            }

            return MeetingMessage.Failure("LeaveRoom 失败");
        }

        #endregion

        #region Transport

        /// <summary>
        /// Create send WebRTC transport.
        /// </summary>
        public Task<MeetingMessage<CreateWebRtcTransportResult>> CreateSendWebRtcTransport(CreateWebRtcTransportRequest createWebRtcTransportRequest)
        {
            return CreateWebRtcTransportAsync(createWebRtcTransportRequest, true);
        }

        /// <summary>
        /// Create recv WebRTC transport.
        /// </summary>
        public Task<MeetingMessage<CreateWebRtcTransportResult>> CreateRecvWebRtcTransport(CreateWebRtcTransportRequest createWebRtcTransportRequest)
        {
            return CreateWebRtcTransportAsync(createWebRtcTransportRequest, false);
        }

        /// <summary>
        /// Create WebRTC transport.
        /// </summary>
        private async Task<MeetingMessage<CreateWebRtcTransportResult>> CreateWebRtcTransportAsync(CreateWebRtcTransportRequest createWebRtcTransportRequest, bool isSend)
        {
            try
            {
                var transport = await _scheduler.CreateWebRtcTransportAsync(UserId, ConnectionId, createWebRtcTransportRequest, isSend);
                transport.On("sctpstatechange", (_, obj) =>
                {
                    _logger.LogDebug("WebRtcTransport \"sctpstatechange\" event [sctpState:{SctpState}]", obj);
                    return Task.CompletedTask;
                });

                transport.On("dtlsstatechange", (_, obj) =>
                {
                    var dtlsState = (DtlsState)obj!;
                    if(dtlsState is DtlsState.FAILED or DtlsState.CLOSED)
                    {
                        _logger.LogWarning("WebRtcTransport dtlsstatechange event [dtlsState:{SctpState}]", obj);
                    }

                    return Task.CompletedTask;
                });

                // NOTE: For testing.
                //await transport.EnableTraceEventAsync(new[] { TransportTraceEventType.Probation, TransportTraceEventType.BWE });
                //await transport.EnableTraceEventAsync(new[] { TransportTraceEventType.BWE });

                var peerId = UserId;
                transport.On("trace", (_, obj) =>
                {
                    // TODO: Fix this
                    // var traceData = (TransportTraceEventData)obj!;
                    // _logger.LogDebug($"transport \"trace\" event [transportId:{transport.TransportId}, trace:{traceData.Type.GetEnumMemberValue()}]");

                    // if(traceData.Type == TransportTraceEventType.BWE && traceData.Direction == TraceEventDirection.Out)
                    // {
                    //     // Notification: downlinkBwe
                    //     SendNotification(peerId, "downlinkBwe", new
                    //     {
                    //         DesiredBitrate = traceData.Info["desiredBitrate"],
                    //         EffectiveDesiredBitrate = traceData.Info["effectiveDesiredBitrate"],
                    //         AvailableBitrate = traceData.Info["availableBitrate"]
                    //     });
                    // }
                    return Task.CompletedTask;
                });

                return MeetingMessage<CreateWebRtcTransportResult>.Success(new CreateWebRtcTransportResult
                {
                    TransportId = transport.TransportId,
                    IceParameters = transport.Data.IceParameters,
                    IceCandidates = transport.Data.IceCandidates,
                    DtlsParameters = transport.Data.DtlsParameters,
                    SctpParameters = transport.Data.Base.SctpParameters,
                },
                "CreateWebRtcTransport 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("CreateWebRtcTransportAsync 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "CreateWebRtcTransportAsync 调用失败.");
            }

            return MeetingMessage<CreateWebRtcTransportResult>.Failure("CreateWebRtcTransportAsync 失败");
        }

        /// <summary>
        /// Connect WebRTC transport.
        /// </summary>
        public async Task<MeetingMessage> ConnectWebRtcTransport(ConnectWebRtcTransportRequest connectWebRtcTransportRequest)
        {
            try
            {
                if(await _scheduler.ConnectWebRtcTransportAsync(UserId, ConnectionId, connectWebRtcTransportRequest))
                {
                    return MeetingMessage.Success("ConnectWebRtcTransport 成功");
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("ConnectWebRtcTransport 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ConnectWebRtcTransport 调用失败.");
            }

            return MeetingMessage.Failure($"ConnectWebRtcTransport 失败: TransportId: {connectWebRtcTransportRequest.TransportId}");
        }

        public async Task<MeetingMessage> Ready()
        {
            if(_meetingServerOptions.ServeMode == ServeMode.Pull)
            {
                return MeetingMessage.Failure("Ready 失败(无需调用)");
            }

            try
            {
                var otherPeers = await _scheduler.GetOtherPeersAsync(UserId, ConnectionId);
                foreach(var producerPeer in otherPeers.Where(m => m.PeerId != UserId))
                {
                    var producers = await producerPeer.GetProducersASync();
                    foreach(var producer in producers.Values)
                    {
                        // 本 Peer 消费其他 Peer
                        CreateConsumer(UserId, producerPeer.PeerId, producer).ContinueWithOnFaultedHandleLog(_logger);
                    }
                }

                return MeetingMessage.Success("Ready 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("Ready 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ready 调用失败.");
            }

            return MeetingMessage.Failure("Ready 失败");
        }

        #endregion

        #region Pull mode

        /// <summary>
        /// Pull medias.
        /// </summary>
        public async Task<MeetingMessage> Pull(PullRequest pullRequest)
        {
            if(_meetingServerOptions.ServeMode != ServeMode.Pull)
            {
                throw new NotSupportedException($"Not supported on \"{_meetingServerOptions.ServeMode}\" mode. Needs \"{ServeMode.Pull}\" mode.");
            }

            try
            {
                var pullResult = await _scheduler.PullAsync(UserId, ConnectionId, pullRequest);
                var consumerPeer = pullResult.ConsumePeer;
                var producerPeer = pullResult.ProducePeer;

                foreach(var producer in pullResult.ExistsProducers)
                {
                    // 本 Peer 消费其他 Peer
                    CreateConsumer(consumerPeer.PeerId, producerPeer.PeerId, producer).ContinueWithOnFaultedHandleLog(_logger);
                }

                if(!pullResult.Sources.IsNullOrEmpty())
                {
                    // Notification: produceSources
                    SendNotification(pullRequest.PeerId, "produceSources", new ProduceSourcesNotification
                    {
                        Sources = pullResult.Sources
                    });
                }

                return MeetingMessage.Success("Pull 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("Pull 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Pull 调用失败.");
            }

            return MeetingMessage.Failure("Pull 失败");
        }

        #endregion

        #region Invite mode

        /// <summary>
        /// Invite medias.
        /// </summary>
        public async Task<MeetingMessage> Invite(InviteRequest inviteRequest)
        {
            if(_meetingServerOptions.ServeMode != ServeMode.Invite)
            {
                throw new NotSupportedException($"Not supported on \"{_meetingServerOptions.ServeMode}\" mode. Needs \"{ServeMode.Invite}\" mode.");
            }

            try
            {
                // 仅会议室管理员可以邀请。
                if(await _scheduler.GetPeerRoleAsync(UserId, ConnectionId) != UserRole.Admin)
                {
                    return MeetingMessage.Failure("仅管理员可发起邀请。");
                }

                // 管理员无需邀请自己。
                if(inviteRequest.PeerId == UserId)
                {
                    return MeetingMessage.Failure("管理员请勿邀请自己。");
                }

                if(inviteRequest.Sources.IsNullOrEmpty() || inviteRequest.Sources.Any(m => m.IsNullOrWhiteSpace()))
                {
                    return MeetingMessage.Failure("Sources 参数缺失或非法。");
                }

                // NOTE: 暂未校验被邀请方是否有对应的 Source 。不过就算接收邀请也无法生产。

                var setPeerInternalDataRequest = new SetPeerInternalDataRequest
                {
                    PeerId = inviteRequest.PeerId,
                    InternalData = new Dictionary<string, object>()
                };
                foreach(var source in inviteRequest.Sources)
                {
                    setPeerInternalDataRequest.InternalData[$"Invite:{source}"] = true;
                }

                await _scheduler.SetPeerInternalDataAsync(setPeerInternalDataRequest);

                // NOTE：如果对应 Source 已经在生产中，是否允许重复邀请？
                // Notification: produceSources
                SendNotification(inviteRequest.PeerId, "produceSources", new ProduceSourcesNotification
                {
                    Sources = inviteRequest.Sources
                });

                return MeetingMessage.Success("Invite 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("Invite 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Invite 调用失败.");
            }

            return MeetingMessage.Failure("Invite 失败");
        }

        /// <summary>
        /// Deinvite medias.
        /// </summary>
        public async Task<MeetingMessage> Deinvite(DeinviteRequest deinviteRequest)
        {
            if(_meetingServerOptions.ServeMode != ServeMode.Invite)
            {
                throw new NotSupportedException($"Not supported on \"{_meetingServerOptions.ServeMode}\" mode. Needs \"{ServeMode.Invite}\" mode.");
            }

            try
            {
                // 仅会议室管理员可以取消邀请。
                if(await _scheduler.GetPeerRoleAsync(UserId, ConnectionId) != UserRole.Admin)
                {
                    return MeetingMessage.Failure("仅管理员可取消邀请。");
                }

                // 管理员无需取消邀请自己。
                if(deinviteRequest.PeerId == UserId)
                {
                    return MeetingMessage.Failure("管理员请勿取消邀请自己。");
                }

                if(deinviteRequest.Sources.IsNullOrEmpty() || deinviteRequest.Sources.Any(m => m.IsNullOrWhiteSpace()))
                {
                    return MeetingMessage.Failure("Sources 参数缺失或非法。");
                }

                // NOTE: 暂未校验被邀请方是否有对应的 Source 。也未校验对应 Source 是否收到邀请。

                var unSetPeerInternalDataRequest = new UnsetPeerInternalDataRequest
                {
                    PeerId = deinviteRequest.PeerId,
                };

                var keys = new List<string>();
                foreach(var source in deinviteRequest.Sources)
                {
                    keys.Add($"Invite:{source}");
                }

                unSetPeerInternalDataRequest.Keys = keys.ToArray();

                await _scheduler.UnsetPeerInternalDataAsync(unSetPeerInternalDataRequest);

                await _scheduler.CloseProducerWithSourcesAsync(UserId, ConnectionId, deinviteRequest.PeerId, deinviteRequest.Sources);

                // Notification: closeSources
                SendNotification(deinviteRequest.PeerId, "closeSources", new CloseSourcesNotification
                {
                    Sources = deinviteRequest.Sources
                });

                return MeetingMessage.Success("Deinvite 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("Deinvite 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Deinvite 调用失败.");
            }

            return MeetingMessage.Failure("Deinvite 失败");
        }

        /// <summary>
        /// Request produce medias.
        /// </summary>
        public async Task<MeetingMessage> RequestProduce(RequestProduceRequest requestProduceRequest)
        {
            if(_meetingServerOptions.ServeMode != ServeMode.Invite)
            {
                throw new NotSupportedException($"Not supported on \"{_meetingServerOptions.ServeMode}\" mode. Needs \"{ServeMode.Invite}\" mode.");
            }

            // 管理员无需发出申请。
            if(await _scheduler.GetPeerRoleAsync(UserId, ConnectionId) == UserRole.Admin)
            {
                return MeetingMessage.Failure("管理员无需发出申请。");
            }

            try
            {
                if(requestProduceRequest.Sources.IsNullOrEmpty() || requestProduceRequest.Sources.Any(m => m.IsNullOrWhiteSpace()))
                {
                    return MeetingMessage.Failure("RequestProduce 失败: Sources 参数缺失或非法。");
                }

                // NOTE: 暂未校验被邀请方是否有对应的 Source 。不过就算接收邀请也无法生产。

                var adminIds = await _scheduler.GetOtherPeerIdsAsync(UserId, ConnectionId, UserRole.Admin);

                // Notification: requestProduce
                SendNotification(adminIds, "requestProduce", new RequestProduceNotification
                {
                    PeerId = UserId,
                    Sources = requestProduceRequest.Sources
                });

                return MeetingMessage.Success("RequestProduce 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("RequestProduce 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "RequestProduce 调用失败.");
            }

            return MeetingMessage.Failure("RequestProduce 失败");
        }

        #endregion

        #region Producer

        /// <summary>
        /// Produce media.
        /// </summary>
        public async Task<MeetingMessage<ProduceRespose>> Produce(ProduceRequest produceRequest)
        {
            // HACK: (alby) Android 传入 RtpParameters 有误的临时处理方案。See: https://mediasoup.discourse.group/t/audio-codec-channel-not-supported/1877
            if(produceRequest.Kind == MediaKind.AUDIO && produceRequest.RtpParameters.Codecs[0].MimeType == "audio/opus")
            {
                produceRequest.RtpParameters.Codecs[0].Channels = 2;
            }

            try
            {
                var peerId = UserId;
                // 在 Invite 模式下如果不是管理员需校验 Source 是否被邀请。
                if(_meetingServerOptions.ServeMode == ServeMode.Invite && await _scheduler.GetPeerRoleAsync(UserId, ConnectionId) != UserRole.Admin)
                {
                    var internalData = await _scheduler.GetPeerInternalDataAsync(UserId, ConnectionId);
                    var inviteKey = $"Invite:{produceRequest.Source}";
                    if(!internalData.InternalData.TryGetValue(inviteKey, out var inviteValue))
                    {
                        return MeetingMessage<ProduceRespose>.Failure("未受邀请的生产。");
                    }

                    // 清除邀请状态
                    await _scheduler.UnsetPeerInternalDataAsync(new UnsetPeerInternalDataRequest
                    {
                        PeerId = UserId,
                        Keys = new[] { inviteKey }
                    });
                }

                var produceResult = await _scheduler.ProduceAsync(peerId, ConnectionId, produceRequest);

                var producerPeer = produceResult.ProducerPeer;
                var producer = produceResult.Producer;
                var otherPeers = _meetingServerOptions.ServeMode == ServeMode.Pull ?
                    produceResult.PullPaddingConsumerPeers : await producerPeer.GetOtherPeersAsync();

                foreach(var consumerPeer in otherPeers)
                {
                    // 其他 Peer 消费本 Peer
                    CreateConsumer(consumerPeer.PeerId, producerPeer.PeerId, producer).ContinueWithOnFaultedHandleLog(_logger);
                }

                // NOTE: For Testing
                //CreateConsumer(producerPeer, producerPeer, producer, "1").ContinueWithOnFaultedHandleLog(_logger);

                // Set Producer events.
                producer.On("score", (_, obj) =>
                {
                    // Notification: producerScore
                    SendNotification(peerId, "producerScore", new ProducerScoreNotification
                    {
                        ProducerId = producer.ProducerId,
                        Score = obj
                    });
                    return Task.CompletedTask;
                });

                producer.On("videoorientationchange", (_, obj) =>
                {
                    // For Testing
                    //var videoOrientation= (ProducerVideoOrientation?)data;

                    // Notification: producerVideoOrientationChanged
                    SendNotification(peerId, "producerVideoOrientationChanged", new ProducerVideoOrientationChangedNotification
                    {
                        ProducerId = producer.ProducerId,
                        VideoOrientation = obj
                    });
                    return Task.CompletedTask;
                });

                // NOTE: For testing.
                // await producer.enableTraceEvent([ 'rtp', 'keyframe', 'nack', 'pli', 'fir' ]);
                // await producer.enableTraceEvent([ 'pli', 'fir' ]);
                // await producer.enableTraceEvent([ 'keyframe' ]);

                producer.On("trace", (_, obj) =>
                {
                    _logger.LogDebug("producer \"trace\" event [producerId:{ProducerId}, trace:{Trace}]", producer.ProducerId, obj);
                    return Task.CompletedTask;
                });

                producer.Observer.On("close", (_, _) =>
                {
                    // Notification: producerClosed
                    SendNotification(peerId, "producerClosed", new ProducerClosedNotification
                    {
                        ProducerId = producer.ProducerId
                    });
                    return Task.CompletedTask;
                });

                return MeetingMessage<ProduceRespose>.Success(new ProduceRespose
                {
                    Id = producer.ProducerId,
                    Source = produceRequest.Source
                },
                    "Produce 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("Produce 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Produce 调用失败.");
            }

            return MeetingMessage<ProduceRespose>.Failure("Produce 失败");
        }

        /// <summary>
        /// Close producer.
        /// </summary>
        public async Task<MeetingMessage> CloseProducer(string producerId)
        {
            try
            {
                if(await _scheduler.CloseProducerAsync(UserId, ConnectionId, producerId))
                {
                    return MeetingMessage.Success("CloseProducer 成功");
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("CloseProducer 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "CloseProducer 调用失败.");
            }

            return MeetingMessage.Failure("CloseProducer 失败");
        }

        /// <summary>
        /// Pause producer.
        /// </summary>
        public async Task<MeetingMessage> PauseProducer(string producerId)
        {
            try
            {
                if(await _scheduler.PauseProducerAsync(UserId, ConnectionId, producerId))
                {
                    return MeetingMessage.Success("PauseProducer 成功");
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("PauseProducer 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "PauseProducer 调用失败.");
            }

            return MeetingMessage.Failure("CloseProducer 失败");
        }

        /// <summary>
        /// Resume producer.
        /// </summary>
        public async Task<MeetingMessage> ResumeProducer(string producerId)
        {
            try
            {
                if(await _scheduler.ResumeProducerAsync(UserId, ConnectionId, producerId))
                {
                    return MeetingMessage.Success("ResumeProducer 成功");
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("CloseProducer 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ResumeProducer 调用失败.");
            }

            return MeetingMessage.Failure("CloseProducer 失败");
        }

        #endregion

        #region Consumer

        /// <summary>
        /// Close consumer.
        /// </summary>
        public async Task<MeetingMessage> CloseConsumer(string consumerId)
        {
            try
            {
                if(await _scheduler.CloseConsumerAsync(UserId, ConnectionId, consumerId))
                {
                    return MeetingMessage.Success("CloseConsumer 成功");
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("CloseConsumer 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "CloseConsumer 调用失败.");
            }

            return MeetingMessage.Failure("CloseConsumer 失败");
        }

        /// <summary>
        /// Pause consumer.
        /// </summary>
        public async Task<MeetingMessage> PauseConsumer(string consumerId)
        {
            try
            {
                if(await _scheduler.PauseConsumerAsync(UserId, ConnectionId, consumerId))
                {
                    return MeetingMessage.Success("PauseConsumer 成功");
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("PauseConsumer 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "PauseConsumer 调用失败.");
            }

            return MeetingMessage.Failure("PauseConsumer 失败");
        }

        /// <summary>
        /// Resume consumer.
        /// </summary>
        public async Task<MeetingMessage> ResumeConsumer(string consumerId)
        {
            try
            {
                var consumer = await _scheduler.ResumeConsumerAsync(UserId, ConnectionId, consumerId);
                if(consumer != null)
                {
                    // Notification: consumerScore
                    SendNotification(UserId, "consumerScore", new ConsumerScoreNotification
                    {
                        ConsumerId = consumer.ConsumerId,
                        Score = consumer.Score
                    });
                }

                return MeetingMessage.Success("ResumeConsumer 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("ResumeConsumer 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ResumeConsumer 调用失败.");
            }

            return MeetingMessage.Failure("ResumeConsumer 失败");
        }

        /// <summary>
        /// Set consumer's preferedLayers.
        /// </summary>
        public async Task<MeetingMessage> SetConsumerPreferedLayers(SetConsumerPreferedLayersRequest setConsumerPreferedLayersRequest)
        {
            try
            {
                if(await _scheduler.SetConsumerPreferedLayersAsync(UserId, ConnectionId, setConsumerPreferedLayersRequest))
                {
                    return MeetingMessage.Success("SetConsumerPreferedLayers 成功");
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("SetConsumerPreferedLayers 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "SetConsumerPreferedLayers 调用失败.");
            }

            return MeetingMessage.Failure("SetConsumerPreferedLayers 失败");
        }

        /// <summary>
        /// Set consumer's priority.
        /// </summary>
        public async Task<MeetingMessage> SetConsumerPriority(SetConsumerPriorityRequest setConsumerPriorityRequest)
        {
            try
            {
                if(await _scheduler.SetConsumerPriorityAsync(UserId, ConnectionId, setConsumerPriorityRequest))
                {
                    return MeetingMessage.Success("SetConsumerPriority 成功");
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("SetConsumerPriority 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "SetConsumerPriority 调用失败.");
            }

            return MeetingMessage.Failure("SetConsumerPreferedLayers 失败");
        }

        /// <summary>
        /// Request key-frame.
        /// </summary>
        public async Task<MeetingMessage> RequestConsumerKeyFrame(string consumerId)
        {
            try
            {
                if(await _scheduler.RequestConsumerKeyFrameAsync(UserId, ConnectionId, consumerId))
                {
                    return MeetingMessage.Success("RequestConsumerKeyFrame 成功");
                }
            }
            catch(MeetingException ex)
            {
                _logger.LogError("RequestConsumerKeyFrame 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "RequestConsumerKeyFrame 调用失败.");
            }

            return MeetingMessage.Failure("RequestConsumerKeyFrame 失败");
        }

        #endregion

        #region Stats

        /// <summary>
        /// Get transport's state.
        /// </summary>
        public async Task<MeetingMessage<object[]>> GetWebRtcTransportStats(string transportId)
        {
            try
            {
                var data = await _scheduler.GetWebRtcTransportStatsAsync(UserId, ConnectionId, transportId);
                return data == null
                    ? MeetingMessage<object[]>.Failure("GetWebRtcTransportStats 失败")
                    : MeetingMessage<object[]>.Success(data, "GetWebRtcTransportStats 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("GetWebRtcTransportStats 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "GetWebRtcTransportStats 调用失败.");
            }

            return MeetingMessage<object[]>.Failure("GetWebRtcTransportStats 失败");
        }

        /// <summary>
        /// Get producer's state.
        /// </summary>
        public async Task<MeetingMessage<object[]>> GetProducerStats(string producerId)
        {
            try
            {
                var data = await _scheduler.GetProducerStatsAsync(UserId, ConnectionId, producerId);
                return data == null
                    ? MeetingMessage<object[]>.Failure("GetProducerStats 失败")
                    : MeetingMessage<object[]>.Success(data, "GetProducerStats 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("GetProducerStats 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "GetProducerStats 调用失败.");
            }

            return MeetingMessage<object[]>.Failure("GetProducerStats 失败");
        }

        /// <summary>
        /// Get consumer's state.
        /// </summary>
        public async Task<MeetingMessage<object[]>> GetConsumerStats(string consumerId)
        {
            try
            {
                var data = await _scheduler.GetConsumerStatsAsync(UserId, ConnectionId, consumerId);
                return data == null
                    ? MeetingMessage<object[]>.Failure("GetConsumerStats 失败")
                    : MeetingMessage<object[]>.Success(data, "GetConsumerStats 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("GetConsumerStats 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "GetConsumerStats 调用失败.");
            }

            return MeetingMessage<object[]>.Failure("GetConsumerStats 失败");
        }

        /// <summary>
        /// Restart ICE.
        /// </summary>
        public async Task<MeetingMessage<IceParametersT>> RestartIce(string transportId)
        {
            try
            {
                var iceParameters = await _scheduler.RestartIceAsync(UserId, ConnectionId, transportId);
                return MeetingMessage<IceParametersT>.Success(iceParameters, "RestartIce 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("RestartIce 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "RestartIce 调用失败.");
            }

            return MeetingMessage<IceParametersT>.Failure("RestartIce 失败");
        }

        #endregion

        #region Message

        /// <summary>
        /// Send message to other peers in rooms.
        /// </summary>
        public async Task<MeetingMessage> SendMessage(SendMessageRequest sendMessageRequest)
        {
            try
            {
                var otherPeerIds = await _scheduler.GetOtherPeerIdsAsync(UserId, ConnectionId);

                // Notification: newMessage
                SendNotification(otherPeerIds, "newMessage", new NewMessageNotification
                {
                    Message = sendMessageRequest.Message,
                });

                return MeetingMessage.Success("SendMessage 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("SendMessage 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "SendMessage 调用失败.");
            }

            return MeetingMessage.Failure("SendMessage 失败");
        }

        #endregion

        #region PeerAppData

        /// <summary>
        /// Set peer's appData. Then notify other peer, if in a room.
        /// </summary>
        public async Task<MeetingMessage> SetPeerAppData(SetPeerAppDataRequest setPeerAppDataRequest)
        {
            try
            {
                var peerPeerAppDataResult = await _scheduler.SetPeerAppDataAsync(UserId, ConnectionId, setPeerAppDataRequest);

                // Notification: peerAppDataChanged
                SendNotification(peerPeerAppDataResult.OtherPeerIds, "peerAppDataChanged", new PeerAppDataChangedNotification
                {
                    PeerId = UserId,
                    AppData = peerPeerAppDataResult.AppData,
                });

                return MeetingMessage.Success("SetPeerAppData 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("SetPeerAppData 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "SetPeerAppData 调用失败.");
            }

            return MeetingMessage.Failure("SetPeerAppData 失败");
        }

        /// <summary>
        /// Unset peer'ss appData. Then notify other peer, if in a room.
        /// </summary>
        public async Task<MeetingMessage> UnsetPeerAppData(UnsetPeerAppDataRequest unsetPeerAppDataRequest)
        {
            try
            {
                var peerPeerAppDataResult = await _scheduler.UnsetPeerAppDataAsync(UserId, ConnectionId, unsetPeerAppDataRequest);

                // Notification: peerAppDataChanged
                SendNotification(peerPeerAppDataResult.OtherPeerIds, "peerAppDataChanged", new PeerAppDataChangedNotification
                {
                    PeerId = UserId,
                    AppData = peerPeerAppDataResult.AppData,
                });

                return MeetingMessage.Success("UnsetPeerAppData 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("UnsetPeerAppData 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "UnsetPeerAppData 调用失败.");
            }

            return MeetingMessage.Failure("UnsetPeerAppData 失败");
        }

        /// <summary>
        /// Clear peer's appData. Then notify other peer, if in a room.
        /// </summary>
        ///
        public async Task<MeetingMessage> ClearPeerAppData()
        {
            try
            {
                var peerPeerAppDataResult = await _scheduler.ClearPeerAppDataAsync(UserId, ConnectionId);

                // Notification: peerAppDataChanged
                SendNotification(peerPeerAppDataResult.OtherPeerIds, "peerAppDataChanged", new PeerAppDataChangedNotification
                {
                    PeerId = UserId,
                    AppData = peerPeerAppDataResult.AppData,
                });

                return MeetingMessage.Success("ClearPeerAppData 成功");
            }
            catch(MeetingException ex)
            {
                _logger.LogError("ClearPeerAppData 调用失败: {Message}", ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ClearPeerAppData 调用失败.");
            }

            return MeetingMessage.Failure("ClearPeerAppData 失败");
        }

        #endregion

        #region Private Methods

        private async Task CreateConsumer(string consumerPeerId, string producerPeerId, Producer producer)
        {
            _logger.LogDebug(
                "CreateConsumer() | [ConsumerPeerId:\"{ConsumerPeerId}\", ProducerPeerId:\"{ProducerPeerId}\", ProducerId:\"{ProducerId}\"]",
                consumerPeerId,
                producerPeerId,
                producer.ProducerId
                );

            // Create the Consumer in paused mode.
            Consumer? consumer;

            try
            {
                consumer = await _scheduler.ConsumeAsync(producerPeerId, consumerPeerId, producer.ProducerId);
                if(consumer == null)
                {
                    return;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "CreateConsumer()");
                return;
            }

            // Set Consumer events.
            consumer.On("score", (_, obj) =>
            {
                // For Testing
                //var score = (ConsumerScore?)obj;

                // Notification: consumerScore
                SendNotification(consumerPeerId, "consumerScore", new ConsumerScoreNotification
                {
                    ProducerPeerId = producerPeerId,
                    Kind = producer.Data.Kind,
                    ConsumerId = consumer.ConsumerId,
                    Score = obj
                });
                return Task.CompletedTask;
            });

            // consumer.On("@close", (_, _) => ...);
            // consumer.On("@producerclose", (_, _) => ...);
            // consumer.On("producerclose", (_, _) => ...);
            // consumer.On("transportclose", (_, _) => ...);
            consumer.Observer.On("close", (_, _) =>
            {
                // Notification: consumerClosed
                SendNotification(consumerPeerId, "consumerClosed", new ConsumerClosedNotification
                {
                    ProducerPeerId = producerPeerId,
                    Kind = producer.Data.Kind,
                    ConsumerId = consumer.ConsumerId
                });
                return Task.CompletedTask;
            });

            consumer.On("producerpause", (_, _) =>
            {
                // Notification: consumerPaused
                SendNotification(consumerPeerId, "consumerPaused", new ConsumerPausedNotification
                {
                    ProducerPeerId = producerPeerId,
                    Kind = producer.Data.Kind,
                    ConsumerId = consumer.ConsumerId
                });
                return Task.CompletedTask;
            });

            consumer.On("producerresume", (_, _) =>
            {
                // Notification: consumerResumed
                SendNotification(consumerPeerId, "consumerResumed", new ConsumerResumedNotification
                {
                    ProducerPeerId = producerPeerId,
                    Kind = producer.Data.Kind,
                    ConsumerId = consumer.ConsumerId
                });
                return Task.CompletedTask;
            });

            consumer.On("layerschange", (_, obj) =>
            {
                // For Testing
                //var layers = (ConsumerLayers?)obj;

                // Notification: consumerLayersChanged
                SendNotification(consumerPeerId, "consumerLayersChanged", new ConsumerLayersChangedNotification
                {
                    ProducerPeerId = producerPeerId,
                    Kind = producer.Data.Kind,
                    ConsumerId = consumer.ConsumerId,
                    Layers = obj
                });
                return Task.CompletedTask;
            });

            // NOTE: For testing.
            // await consumer.enableTraceEvent([ 'rtp', 'keyframe', 'nack', 'pli', 'fir' ]);
            // await consumer.enableTraceEvent([ 'pli', 'fir' ]);
            // await consumer.enableTraceEvent([ 'keyframe' ]);

            consumer.On("trace", (_, obj) =>
            {
                _logger.LogDebug("consumer \"trace\" event [consumerId:{ConsumerId}, trace:{Trace}]", consumer.ConsumerId, obj);
                return Task.CompletedTask;
            });

            // Send a request to the remote Peer with Consumer parameters.
            // Notification: newConsumer
            SendNotification(consumerPeerId, "newConsumer", new NewConsumerNotification
            {
                ProducerPeerId = producerPeerId,
                Kind = consumer.Data.Kind,
                ProducerId = producer.ProducerId,
                ConsumerId = consumer.ConsumerId,
                RtpParameters = consumer.Data.RtpParameters,
                Type = consumer.Data.Type,
                ProducerAppData = producer.AppData,
                ProducerPaused = consumer.ProducerPaused,
            });
        }

        private void SendNotification(string peerId, string type, object data)
        {
            // For Testing
            if(type is "consumerLayersChanged" or "consumerScore" or "producerScore")
            {
                return;
            }

            var client = _hubContext.Clients.User(peerId);
            client.Notify(new MeetingNotification
            {
                Type = type,
                Data = data
            }).ContinueWithOnFaultedHandleLog(_logger);
        }

        private void SendNotification(IReadOnlyList<string> peerIds, string type, object data)
        {
            if(peerIds.IsNullOrEmpty())
            {
                return;
            }

            var client = _hubContext.Clients.Users(peerIds);
            client.Notify(new MeetingNotification
            {
                Type = type,
                Data = data
            }).ContinueWithOnFaultedHandleLog(_logger);
        }

        #endregion Private Methods
    }
}
