namespace FBS.PlainTransport
{
    public class DumpResponseT
    {
        public FBS.Transport.DumpT Base { get; set; }

        public bool RtcpMux { get; set; }

        public bool Comedia { get; set; }

        public FBS.Transport.TupleT Tuple { get; set; }

        public FBS.Transport.TupleT RtcpTuple { get; set; }

        public FBS.SrtpParameters.SrtpParametersT SrtpParameters { get; set; }
    }
}
