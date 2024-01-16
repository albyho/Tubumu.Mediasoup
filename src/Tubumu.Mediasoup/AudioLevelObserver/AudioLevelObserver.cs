using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
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
        /// <param name="loggerFactory"></param>
        /// <param name="@internal"></param>
        /// <param name="channel"></param>
        /// <param name="payloadChannel"></param>
        /// <param name="appData"></param>
        /// <param name="getProducerById"></param>
        public AudioLevelObserver(ILoggerFactory loggerFactory,
            RtpObserverInternal @internal,
            IChannel channel,
            Dictionary<string, object>? appData,
            Func<string, Task<Producer?>> getProducerById
            ) : base(loggerFactory, @internal, channel, appData, getProducerById)
        {
            _logger = loggerFactory.CreateLogger<AudioLevelObserver>();
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
                case "volumes":
                    {
                        var notification = JsonSerializer.Deserialize<AudioLevelObserverVolumeNotificationData[]>(data!, ObjectExtensions.DefaultJsonSerializerOptions)!;

                        var volumes = new List<AudioLevelObserverVolume>();
                        foreach (var item in notification)
                        {
                            var producer = await GetProducerById(item.ProducerId);
                            if (producer != null)
                            {
                                volumes.Add(new AudioLevelObserverVolume
                                {
                                    Producer = producer,
                                    Volume = item.Volume,
                                });
                            }
                        }

                        if (volumes.Count > 0)
                        {
                            Emit("volumes", volumes);

                            // Emit observer event.
                            Observer.Emit("volumes", volumes);
                        }

                        break;
                    }
                case "silence":
                    {
                        Emit("silence");

                        // Emit observer event.
                        Observer.Emit("silence");

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
