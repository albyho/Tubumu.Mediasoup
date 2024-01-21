using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FBS.DirectTransport;
using FBS.Request;
using FBS.Transport;
using FBS.Notification;

namespace Tubumu.Mediasoup
{
    public class DirectTransport : Transport
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<DirectTransport> _logger;

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits rtcp - (packet: Buffer)</para>
        /// <para>@emits trace - (trace: TransportTraceEventData)</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits newdataproducer - (dataProducer: DataProducer)</para>
        /// <para>@emits newdataconsumer - (dataProducer: DataProducer)</para>
        /// <para>@emits trace - (trace: TransportTraceEventData)</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="internal_"></param>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="getRouterRtpCapabilities"></param>
        /// <param name="getProducerById"></param>
        /// <param name="getDataProducerById"></param>
        public DirectTransport(
            ILoggerFactory loggerFactory,
            TransportInternal internal_,
            DumpResponseT data,
            IChannel channel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Task<Producer?>> getProducerById,
            Func<string, Task<DataProducer?>> getDataProducerById
        )
            : base(
                loggerFactory,
                internal_,
                data.Base,
                channel,
                appData,
                getRouterRtpCapabilities,
                getProducerById,
                getDataProducerById
            )
        {
            _logger = loggerFactory.CreateLogger<DirectTransport>();

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the DirectTransport.
        /// </summary>
        /// <returns></returns>
        protected override Task OnCloseAsync()
        {
            // Do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// Router was closed.
        /// </summary>
        protected override Task OnRouterClosedAsync()
        {
            // Do nothing
            return Task.CompletedTask;
        }

        /// <summary>
        /// Dump Transport.
        /// </summary>
        protected override async Task<object> OnDumpAsync()
        {
            var response = await Channel.RequestAsync(Method.TRANSPORT_DUMP, null, null, Internal.TransportId);
            var data = response.Value.BodyAsDirectTransport_DumpResponse().UnPack();
            return data;
        }

        /// <summary>
        /// Get Transport stats.
        /// </summary>
        protected override async Task<object[]> OnGetStatsAsync()
        {
            var response = await Channel.RequestAsync(Method.TRANSPORT_GET_STATS, null, null, Internal.TransportId);
            var data = response.Value.BodyAsDirectTransport_GetStatsResponse().UnPack();
            return new[] { data };
        }

        /// <summary>
        /// NO-OP method in DirectTransport.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected override Task OnConnectAsync(object parameters)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set maximum incoming bitrate for receiving media.
        /// </summary>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        public override Task<string> SetMaxIncomingBitrateAsync(uint bitrate)
        {
            _logger.LogError("SetMaxIncomingBitrateAsync() | DiectTransport:{TransportId} Bitrate:{bitrate}", TransportId, TransportId);
            throw new NotImplementedException("SetMaxIncomingBitrateAsync() not implemented in DirectTransport");
        }

        /// <summary>
        /// Set maximum outgoing bitrate for sending media.
        /// </summary>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        public override Task<string> SetMaxOutgoingBitrateAsync(uint bitrate)
        {
            _logger.LogError("SetMaxOutgoingBitrateAsync() | DiectTransport:{TransportId} Bitrate:{bitrate}", TransportId, TransportId);
            throw new NotImplementedException("SetMaxOutgoingBitrateAsync is not implemented in DirectTransport");
        }

        /// <summary>
        /// Set minimum outgoing bitrate for sending media.
        /// </summary>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        public override Task<string> SetMinOutgoingBitrateAsync(uint bitrate)
        {
            _logger.LogError("SetMinOutgoingBitrateAsync() | DiectTransport:{TransportId} Bitrate:{bitrate}", TransportId, TransportId);
            throw new NotImplementedException("SetMinOutgoingBitrateAsync is not implemented in DirectTransport");
        }

        /// <summary>
        /// Create a Producer.
        /// </summary>
        public override Task<Producer> ProduceAsync(ProducerOptions producerOptions)
        {
            _logger.LogError("ProduceAsync() | DiectTransport:{TransportId}", TransportId);
            throw new NotImplementedException("ProduceAsync() is not implemented in DirectTransport");
        }

        /// <summary>
        /// Create a Consumer.
        /// </summary>
        /// <param name="consumerOptions"></param>
        /// <returns></returns>
        public override Task<Consumer> ConsumeAsync(ConsumerOptions consumerOptions)
        {
            _logger.LogError("ConsumeAsync() | DiectTransport:{TransportId}", TransportId);
            throw new NotImplementedException("ConsumeAsync() not implemented in DirectTransport");
        }

        public async Task SendRtcpAsync(byte[] rtcpPacket)
        {
            using(await CloseLock.ReadLockAsync())
            {
                if(Closed)
                {
                    throw new InvalidStateException("Transport closed");
                }

                var sendRtcpNotification = new SendRtcpNotificationT
                {
                    Data = rtcpPacket
                };

                var sendRtcpNotificationOffset = SendRtcpNotification.Pack(Channel.BufferBuilder, sendRtcpNotification);

                // Fire and forget
                Channel.NotifyAsync(
                    Event.TRANSPORT_SEND_RTCP,
                    FBS.Notification.Body.Transport_SendRtcpNotification,
                    sendRtcpNotificationOffset.Value,
                    Internal.TransportId
                    ).ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            Channel.OnNotification += OnNotificationHandle;
        }

        private void OnNotificationHandle(string handlerId, Event @event, Notification notification)
        {
            if(handlerId != Internal.TransportId)
            {
                return;
            }

            switch(@event)
            {
                case Event.TRANSPORT_TRACE:
                    {
                        var traceNotification = notification.BodyAsTransport_TraceNotification().UnPack();

                        Emit("trace", traceNotification);

                        // Emit observer event.
                        Observer.Emit("trace", traceNotification);

                        break;
                    }
                default:
                    {
                        _logger.LogError("OnNotificationHandle() | DiectTransport:{TransportId} Ignoring unknown event:{@event}", TransportId, @event);
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
