using System;
using System.Text.Json.Serialization;

namespace FBS.RtxStream
{
    public class ParamsT
    {
        public uint Ssrc { get; set; }

        public byte PayloadType { get; set; }

        public string MimeType { get; set; }

        public uint ClockRate { get; set; }

        public string Rrid { get; set; }

        public string Cname { get; set; }
    }
}
