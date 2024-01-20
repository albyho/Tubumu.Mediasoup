using FBS.SrtpParameters;

namespace Tubumu.Mediasoup
{
    public class PipeTransportConnectParameters
    {
        public string Ip { get; set; }

        public int Port { get; set; }

        public SrtpParametersT? SrtpParameters { get; set; }
    }
}
