using System.Collections.Generic;

namespace Tubumu.Mediasoup
{
    public class ProducerOptions
    {
        /// <summary>
        /// Producer id (just for Router.pipeToRouter() method).
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Media kind ('audio' or 'video').
        /// </summary>
        public MediaKind Kind { get; set; }

        /// <summary>
        /// RTP parameters defining what the endpoint is sending.
        /// </summary>
        public RtpParameters RtpParameters { get; set; }

        /// <summary>
        /// Whether the producer must start in paused mode. Default false.
        /// </summary>
        public bool? Paused { get; set; } = false;

        /// <summary>
        /// Just for video. Time (in ms) before asking the sender for a new key frame
        /// after having asked a previous one. Default 0.
        /// </summary>
        public int? KeyFrameRequestDelay { get; set; } = 0;

        /// <summary>
        /// Custom application data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; set; }
    }
}
