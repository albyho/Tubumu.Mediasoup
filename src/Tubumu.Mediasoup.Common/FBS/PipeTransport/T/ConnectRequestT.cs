namespace FBS.PipeTransport
{
    public class ConnectRequestT
    {
        public string Ip { get; set; }

        public ushort? Port { get; set; }

        public FBS.SrtpParameters.SrtpParametersT SrtpParameters { get; set; }
    }
}
