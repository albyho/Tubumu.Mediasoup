using FBS.RtpParameters;
using System.Linq;

namespace Tubumu.Mediasoup
{
    public static class RtpParametersExtensions
    {
        public static RtpParametersT SerializeRtpParameters(this RtpParameters rtpParameters)
        {
            var result = new RtpParametersT
            {
                Mid = rtpParameters.Mid,
                Encodings = rtpParameters.Encodings,
                Rtcp = rtpParameters.Rtcp,

                // Codecs
                Codecs = (from c in rtpParameters.Codecs
                          select new RtpCodecParametersT
                          {
                              MimeType = c.MimeType,
                              PayloadType = c.PayloadType,
                              ClockRate = c.ClockRate,
                              Channels = c.Channels,
                              Parameters = (from p in c.Parameters
                                            select new ParameterT
                                            {
                                                Name = p.Key,
                                                Value = p.Value.ConvertToValueUnion()
                                            }).ToList(),
                              RtcpFeedback = c.RtcpFeedback,
                          }).ToList(),

                // RTP header extensions
                HeaderExtensions = (from h in rtpParameters.HeaderExtensions
                                    select new RtpHeaderExtensionParametersT
                                    {
                                        Uri = h.Uri,
                                        Id = h.Id,
                                        Encrypt = h.Encrypt,
                                        Parameters = (from p in h.Parameters
                                                      select new ParameterT
                                                      {
                                                          Name = p.Key,
                                                          Value = p.Value.ConvertToValueUnion()
                                                      }).ToList(),
                                    }).ToList()
            };

            return result;
        }

        public static RtpParameters DeserializeRtpParameters(this RtpParametersT rtpParameters)
        {
            var result = new RtpParameters
            {
                Mid = rtpParameters.Mid,
                Encodings = rtpParameters.Encodings,
                Rtcp = rtpParameters.Rtcp,

                // Codecs
                Codecs = (from c in rtpParameters.Codecs
                          select new RtpCodecParameters
                          {
                              MimeType = c.MimeType,
                              PayloadType = c.PayloadType,
                              ClockRate = c.ClockRate,
                              Channels = c.Channels,
                              Parameters = c.Parameters.ToDictionary(k => k.Name, v => v.Value.Value_),
                              RtcpFeedback = c.RtcpFeedback,
                          }).ToList(),

                // RTP header extensions
                HeaderExtensions = (from h in rtpParameters.HeaderExtensions
                                    select new RtpHeaderExtensionParameters
                                    {
                                        Uri = h.Uri,
                                        Id = h.Id,
                                        Encrypt = h.Encrypt,
                                        Parameters = h.Parameters.ToDictionary(k => k.Name, v => v.Value.Value_),
                                    }).ToList()
            };

            return result;
        }
    }
}
