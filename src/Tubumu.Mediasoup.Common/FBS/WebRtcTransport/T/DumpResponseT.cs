using System.Collections.Generic;

namespace FBS.WebRtcTransport
{
    public class DumpResponseT
    {
        public FBS.Transport.DumpT Base { get; set; }

        public FBS.WebRtcTransport.IceRole IceRole { get; set; }

        public FBS.WebRtcTransport.IceParametersT IceParameters { get; set; }

        public List<FBS.WebRtcTransport.IceCandidateT> IceCandidates { get; set; }

        public FBS.WebRtcTransport.IceState IceState { get; set; }

        public FBS.Transport.TupleT? IceSelectedTuple { get; set; }

        public FBS.WebRtcTransport.DtlsParametersT DtlsParameters { get; set; }

        public FBS.WebRtcTransport.DtlsState DtlsState { get; set; }
    }
}
