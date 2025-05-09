﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FBS.DirectTransport;
using FBS.Notification;
using FBS.Request;
using FBS.Transport;
using Microsoft.Extensions.Logging;

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
        public DirectTransport(
            ILoggerFactory loggerFactory,
            TransportInternal @internal,
            DumpResponseT data,
            IChannel channel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Task<Producer?>> getProducerById,
            Func<string, Task<DataProducer?>> getDataProducerById
        )
            : base(
                loggerFactory,
                @internal,
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
            // Build Request
            var bufferBuilder = Channel.BufferPool.Get();

            var response = await Channel.RequestAsync(bufferBuilder, Method.TRANSPORT_DUMP, null, null, Internal.TransportId);
            var data = response!.Value.BodyAsDirectTransport_DumpResponse().UnPack();

            return data;
        }

        /// <summary>
        /// Get Transport stats.
        /// </summary>
        protected override async Task<object[]> OnGetStatsAsync()
        {
            // Build Request
            var bufferBuilder = Channel.BufferPool.Get();

            var response = await Channel.RequestAsync(
                bufferBuilder,
                Method.TRANSPORT_GET_STATS,
                null,
                null,
                Internal.TransportId
            );
            var data = response!.Value.BodyAsDirectTransport_GetStatsResponse().UnPack();
            return [data];
        }

        /// <summary>
        /// NO-OP method in DirectTransport.
        /// </summary>
        protected override Task OnConnectAsync(object parameters)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set maximum incoming bitrate for receiving media.
        /// </summary>
        public override Task<string> SetMaxIncomingBitrateAsync(uint bitrate)
        {
            _logger.LogError(
                "SetMaxIncomingBitrateAsync() | DiectTransport:{TransportId} Bitrate:{bitrate}",
                TransportId,
                TransportId
            );
            throw new NotImplementedException("SetMaxIncomingBitrateAsync() not implemented in DirectTransport");
        }

        /// <summary>
        /// Set maximum outgoing bitrate for sending media.
        /// </summary>
        public override Task<string> SetMaxOutgoingBitrateAsync(uint bitrate)
        {
            _logger.LogError(
                "SetMaxOutgoingBitrateAsync() | DiectTransport:{TransportId} Bitrate:{bitrate}",
                TransportId,
                TransportId
            );
            throw new NotImplementedException("SetMaxOutgoingBitrateAsync is not implemented in DirectTransport");
        }

        /// <summary>
        /// Set minimum outgoing bitrate for sending media.
        /// </summary>
        public override Task<string> SetMinOutgoingBitrateAsync(uint bitrate)
        {
            _logger.LogError(
                "SetMinOutgoingBitrateAsync() | DiectTransport:{TransportId} Bitrate:{bitrate}",
                TransportId,
                TransportId
            );
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
        public override Task<Consumer> ConsumeAsync(ConsumerOptions consumerOptions)
        {
            _logger.LogError("ConsumeAsync() | DiectTransport:{TransportId}", TransportId);
            throw new NotImplementedException("ConsumeAsync() not implemented in DirectTransport");
        }

        public async Task SendRtcpAsync(byte[] rtcpPacket)
        {
            await using (await CloseLock.ReadLockAsync())
            {
                if (Closed)
                {
                    throw new InvalidStateException("Transport closed");
                }

                // Build Request
                var bufferBuilder = Channel.BufferPool.Get();

                var sendRtcpNotification = new SendRtcpNotificationT { Data = rtcpPacket };

                var sendRtcpNotificationOffset = SendRtcpNotification.Pack(bufferBuilder, sendRtcpNotification);

                // Fire and forget
                Channel
                    .NotifyAsync(
                        bufferBuilder,
                        Event.TRANSPORT_SEND_RTCP,
                        FBS.Notification.Body.Transport_SendRtcpNotification,
                        sendRtcpNotificationOffset.Value,
                        Internal.TransportId
                    )
                    .ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            Channel.OnNotification += OnNotificationHandle;
        }

        private void OnNotificationHandle(string handlerId, Event @event, Notification notification)
        {
            if (handlerId != Internal.TransportId)
            {
                return;
            }

            switch (@event)
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
                    _logger.LogError(
                        "OnNotificationHandle() | DiectTransport:{TransportId} Ignoring unknown event:{@event}",
                        TransportId,
                        @event
                    );
                    break;
                }
            }
        }

        #endregion Event Handlers
    }
}
