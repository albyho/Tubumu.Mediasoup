using System.Collections.Generic;

namespace FBS.WebRtcTransport
{
    public class DtlsParametersT
    {
        public List<FBS.WebRtcTransport.FingerprintT> Fingerprints { get; set; }

        public FBS.WebRtcTransport.DtlsRole Role { get; set; }
    }
}
