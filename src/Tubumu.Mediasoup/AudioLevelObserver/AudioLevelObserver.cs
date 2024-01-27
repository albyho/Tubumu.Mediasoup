using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FBS.Notification;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
{
    public class AudioLevelObserver : RtpObserver
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<AudioLevelObserver> _logger;

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
        public AudioLevelObserver(
            ILoggerFactory loggerFactory,
            RtpObserverInternal internal_,
            IChannel channel,
            Dictionary<string, object>? appData,
            Func<string, Task<Producer?>> getProducerById
        )
            : base(loggerFactory, internal_, channel, appData, getProducerById)
        {
            _logger = loggerFactory.CreateLogger<AudioLevelObserver>();
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        protected override async void OnNotificationHandle(string handlerId, Event @event, Notification data)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if(handlerId != Internal.RtpObserverId)
            {
                return;
            }

            switch(@event)
            {
                case Event.AUDIOLEVELOBSERVER_VOLUMES:
                    {
                        var volumesNotification = data.BodyAsAudioLevelObserver_VolumesNotification().UnPack();

                        var volumes = new List<AudioLevelObserverVolume>();
                        foreach(var item in volumesNotification.Volumes)
                        {
                            var producer = await GetProducerById(item.ProducerId);
                            if(producer != null)
                            {
                                volumes.Add(new AudioLevelObserverVolume { Producer = producer, Volume = item.Volume_, });
                            }
                        }

                        if(volumes.Count > 0)
                        {
                            Emit("volumes", volumes);

                            // Emit observer event.
                            Observer.Emit("volumes", volumes);
                        }

                        break;
                    }
                case Event.AUDIOLEVELOBSERVER_SILENCE:
                    {
                        Emit("silence");

                        // Emit observer event.
                        Observer.Emit("silence");

                        break;
                    }
                default:
                    {
                        _logger.LogError("OnNotificationHandle() | Ignoring unknown event: {@event}", @event);
                        break;
                    }
            }
        }
    }
}
