using System.Collections.Generic;

namespace FBS.RtpParameters
{
    public class RtpMappingT
    {
        public List<FBS.RtpParameters.CodecMappingT> Codecs { get; set; }

        public List<FBS.RtpParameters.EncodingMappingT> Encodings { get; set; }
    }
}
