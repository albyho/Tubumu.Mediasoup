using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FBS.Notification;
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
        /// <param name="internal_"></param>
        /// <param name="channel"></param>
        /// <param name="appData"></param>
        /// <param name="getProducerById"></param>
        public ActiveSpeakerObserver(
            ILoggerFactory loggerFactory,
            RtpObserverInternal internal_,
            IChannel channel,
            Dictionary<string, object>? appData,
            Func<string, Task<Producer?>> getProducerById
        )
            : base(loggerFactory, internal_, channel, appData, getProducerById)
        {
            _logger = loggerFactory.CreateLogger<ActiveSpeakerObserver>();
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        protected override async void OnNotificationHandle(string handlerId, Event @event, Notification notification)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if(handlerId != Internal.RtpObserverId)
            {
                return;
            }

            switch(@event)
            {
                case Event.ACTIVESPEAKEROBSERVER_DOMINANT_SPEAKER:
                    {
                        var dominantSpeakerNotification = notification.BodyAsActiveSpeakerObserver_DominantSpeakerNotification().UnPack();

                        var producer = GetProducerById(dominantSpeakerNotification.ProducerId);
                        if(producer != null)
                        {
                            var dominantSpeaker = new ActiveSpeakerObserverDominantSpeaker
                            {
                                Producer = await GetProducerById(dominantSpeakerNotification.ProducerId)
                            };

                            Emit("dominantspeaker", dominantSpeaker);

                            // Emit observer event.
                            Observer.Emit("dominantspeaker", dominantSpeaker);
                        }

                        break;
                    }
                default:
                    {
                        _logger.LogError($"OnNotificationHandle() | Ignoring unknown event{@event}");
                        break;
                    }
            }
        }
    }
}
