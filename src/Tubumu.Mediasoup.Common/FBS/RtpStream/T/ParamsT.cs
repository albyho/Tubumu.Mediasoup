namespace FBS.RtpStream
{
    public class ParamsT
    {
        public uint EncodingIdx { get; set; }

        public uint Ssrc { get; set; }

        public byte PayloadType { get; set; }

        public string MimeType { get; set; }

        public uint ClockRate { get; set; }

        public string Rid { get; set; }

        public string Cname { get; set; }

        public uint? RtxSsrc { get; set; }

        public byte? RtxPayloadType { get; set; }

        public bool UseNack { get; set; }

        public bool UsePli { get; set; }

        public bool UseFir { get; set; }

        public bool UseInBandFec { get; set; }

        public bool UseDtx { get; set; }

        public byte SpatialLayers { get; set; }

        public byte TemporalLayers { get; set; }
    }
}
