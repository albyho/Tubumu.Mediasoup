namespace FBS.PlainTransport
{
    public class ConnectResponseT
    {
        public FBS.Transport.TupleT Tuple { get; set; }

        public FBS.Transport.TupleT RtcpTuple { get; set; }

        public FBS.SrtpParameters.SrtpParametersT SrtpParameters { get; set; }
    }
}
