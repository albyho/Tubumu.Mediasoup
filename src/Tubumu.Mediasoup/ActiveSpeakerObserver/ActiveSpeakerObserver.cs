using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
{
    public class ActiveSpeakerObserver : RtpObserver
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<ActiveSpeakerObserver> _logger;

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits volumes - (volumes: AudioLevelObserverVolume[])</para>
        /// <para>@emits silence</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits pause</para>
        /// <para>@emits resume</para>
        /// <para>@emits addproducer - (producer: Producer)</para>
        /// <para>@emits removeproducer - (producer: Producer)</para>
        /// <para>@emits volumes - (volumes: AudioLevelObserverVolume[])</para>
        /// <para>@emits silence</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="@internal"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        /// <param name="getProducerById"></param>
        public ActiveSpeakerObserver(ILoggerFactory loggerFactory,
            RtpObserverInternal @internal,
            IChannel channel,
            IPayloadChannel payloadChannel,
            Dictionary<string, object>? appData,
            Func<string, Task<Producer?>> getProducerById
            ) : base(loggerFactory, @internal, channel, payloadChannel, appData, getProducerById)
        {
            _logger = loggerFactory.CreateLogger<ActiveSpeakerObserver>();
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        protected override async void OnChannelMessage(string targetId, string @event, string? data)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if (targetId != Internal.RtpObserverId)
            {
                return;
            }

            switch (@event)
            {
                case "dominantspeaker":
                    {
                        var notification = JsonSerializer.Deserialize<ActiveSpeakerObserverNotificationData>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        var producer = GetProducerById(notification.ProducerId);
                        if (producer != null)
                        {
                            var dominantSpeaker = new ActiveSpeakerObserverActivity
                            {
                                Producer = await GetProducerById(notification.ProducerId)
                            };

                            Emit("dominantspeaker", dominantSpeaker);

                            // Emit observer event.
                            Observer.Emit("dominantspeaker", dominantSpeaker);
                        }

                        break;
                    }
                default:
                    {
                        _logger.LogError($"OnChannelMessage() | Ignoring unknown event{@event}");
                        break;
                    }
            }
        }
    }
}
