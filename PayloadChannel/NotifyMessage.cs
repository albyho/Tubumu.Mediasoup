using System;

namespace TubumuMeeting.Mediasoup
{
    public class NotifyMessage
    {
        public ArraySegment<byte> Message { get; set; }

        public int PPID { get; set; }
    }
}
