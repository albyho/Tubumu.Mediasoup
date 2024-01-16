using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
{
    public class PassthroughObserver : RtpObserver
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<PassthroughObserver> _logger;

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits rtp</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits pause</para>
        /// <para>@emits resume</para>
        /// <para>@emits addproducer - (producer: Producer)</para>
        /// <para>@emits removeproducer - (producer: Producer)</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="@internal"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="getProducerById"></param>
        public PassthroughObserver(ILoggerFactory loggerFactory,
            RtpObserverInternal @internal,
            IChannel channel,
            Dictionary<string, object>? appData,
            Func<string, Task<Producer?>> getProducerById
            ) : base(loggerFactory, @internal, channel, appData, getProducerById)
        {
            _logger = loggerFactory.CreateLogger<PassthroughObserver>();
        }

        protected override void OnPayloadChannelMessage(string targetId, string @event, string? data, ArraySegment<byte> payload)
        {
            if (targetId != Internal.RtpObserverId)
            {
                return;
            }

            switch (@event)
            {
                case "rtp":
                    {
                        var notification = JsonSerializer.Deserialize<ActiveSpeakerObserverNotificationData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        var producer = GetProducerById(notification.ProducerId);
                        if (producer != null)
                        {
                            Emit("rtp", payload);
                        }

                        break;
                    }
                default:
                    {
                        _logger.LogError($"OnPayloadChannelMessage() | Ignoring unknown event{@event}");
                        break;
                    }
            }
        }
    }
}
