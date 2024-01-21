namespace FBS.PipeTransport
{
    public class DumpResponseT
    {
        public FBS.Transport.DumpT Base { get; set; }

        public FBS.Transport.TupleT Tuple { get; set; }

        public bool Rtx { get; set; }

        public FBS.SrtpParameters.SrtpParametersT SrtpParameters { get; set; }
    }
}
