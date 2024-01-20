using System.Collections.Generic;

namespace FBS.RtpParameters
{
    public class RtpHeaderExtensionParametersT
    {
        public RtpHeaderExtensionUri Uri { get; set; }

        public byte Id { get; set; }

        public bool Encrypt { get; set; }

        public List<ParameterT> Parameters { get; set; }
    }
}
