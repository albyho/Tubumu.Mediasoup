using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ObjectExtensions = Tubumu.Utils.Extensions.Object.ObjectExtensions;

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
        /// <param name="transportInternalData"></param>
        /// <param name="sctpParameters"></param>
        /// <param name="sctpState"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="getRouterRtpCapabilities"></param>
        /// <param name="getProducerById"></param>
        /// <param name="getDataProducerById"></param>
        public DirectTransport(ILoggerFactory loggerFactory,
            TransportInternalData transportInternalData,
            SctpParameters? sctpParameters,
            SctpState? sctpState,
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            Func<RtpCapabilities> getRouterRtpCapabilities,
            Func<string, Producer?> getProducerById,
            Func<string, DataProducer?> getDataProducerById
            ) : base(loggerFactory, transportInternalData, sctpParameters, sctpState, channel, payloadChannel, appData, getRouterRtpCapabilities, getProducerById, getDataProducerById)
        {
            _logger = loggerFactory.CreateLogger<DirectTransport>();

            // Data

            HandleWorkerNotifications();
        }

        /// <summary>
        /// Close the PipeTransport.
        /// </summary>
        public override async Task CloseAsync()
        {
            if (Closed)
            {
                return;
            }

            await CloseLock.WaitAsync();
            try
            {
                if (Closed)
                {
                    return;
                }

                await base.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseAsync()");
            }
            finally
            {
                CloseLock.Set();
            }
        }

        /// <summary>
        /// Router was closed.
        /// </summary>
        public override async Task RouterClosedAsync()
        {
            if (Closed)
            {
                return;
            }

            await CloseLock.WaitAsync();
            try
            {
                if (Closed)
                {
                    return;
                }

                await base.RouterClosedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RouterClosedAsync()");
            }
            finally
            {
                CloseLock.Set();
            }
        }

        /// <summary>
        /// NO-OP method in DirectTransport.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override Task ConnectAsync(object parameters)
        {
            _logger.LogDebug($"ConnectAsync() | DiectTransport:{TransportId}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Set maximum incoming bitrate for receiving media.
        /// </summary>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        public override Task<string?> SetMaxIncomingBitrateAsync(int bitrate)
        {
            _logger.LogDebug($"SetMaxIncomingBitrateAsync() | DiectTransport:{TransportId} Bitrate:{bitrate}");
            throw new NotImplementedException("SetMaxIncomingBitrateAsync() not implemented in DirectTransport");
        }

        /// <summary>
        /// Set maximum outgoing bitrate for sending media.
        /// </summary>
        /// <param name="bitrate"></param>
        /// <returns></returns>
        public override Task<string?> SetMaxOutgoingBitrateAsync(int bitrate)
        {
            throw new NotImplementedException("SetMaxOutgoingBitrateAsync is not implemented in DirectTransport");
        }

        /// <summary>
        /// Create a Producer.
        /// </summary>
        public override Task<Producer> ProduceAsync(ProducerOptions producerOptions)
        {
            _logger.LogDebug($"ProduceAsync() | DiectTransport:{TransportId}");
            throw new NotImplementedException("ProduceAsync() is not implemented in DirectTransport");
        }

        /// <summary>
        /// Create a Consumer.
        /// </summary>
        /// <param name="consumerOptions"></param>
        /// <returns></returns>
        public override Task<Consumer> ConsumeAsync(ConsumerOptions consumerOptions)
        {
            _logger.LogDebug($"ConsumeAsync() | DiectTransport:{TransportId}");
            throw new NotImplementedException("ConsumeAsync() not implemented in DirectTransport");
        }

        public Task SendRtcpAsync(byte[] rtcpPacket)
        {
            return PayloadChannel.NotifyAsync("transport.sendRtcp", Internal, null, rtcpPacket);
        }

        #region Event Handlers

        private void HandleWorkerNotifications()
        {
            Channel.MessageEvent += OnChannelMessage;
            PayloadChannel.MessageEvent += OnPayloadChannelMessage;
        }

        private void OnChannelMessage(string targetId, string @event, string? data)
        {
            if (targetId != Internal.TransportId)
            {
                return;
            }

            switch (@event)
            {
                case "trace":
                    {
                        var trace = JsonSerializer.Deserialize<TransportTraceEventData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        Emit("trace", trace);

                        // Emit observer event.
                        Observer.Emit("trace", trace);

                        break;
                    }

                default:
                    {
                        _logger.LogError($"OnChannelMessage() | DiectTransport:{TransportId} Ignoring unknown event{@event}");
                        break;
                    }
            }
        }

        private void OnPayloadChannelMessage(string targetId, string @event, NotifyData notifyData, ArraySegment<byte> payload)
        {
            if (targetId != Internal.TransportId)
            {
                return;
            }

            switch (@event)
            {
                case "rtcp":
                    {
                        // TODO: (alby) _closed 的使用及线程安全。
                        if (Closed)
                        {
                            break;
                        }

                        Emit("rtcp", payload);

                        break;
                    }

                default:
                    {
                        _logger.LogError($"Ignoring unknown event \"{@event}\"");
                        break;
                    }
            }
        }

        #endregion Event Handlers
    }
}
