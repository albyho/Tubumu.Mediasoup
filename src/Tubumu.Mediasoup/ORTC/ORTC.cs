﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using FBS.RtpParameters; // MediaKind, RtcpFeedbackT RtcpParametersT, RtpEncodingParametersT
using FBS.SctpParameters; // SctpParamerters, NumSctpStreamsT, SctpStreamParametersT
using Force.DeepCloner;

namespace Tubumu.Mediasoup
{
    public static class ORTC
    {
        private static readonly Regex MimeTypeRegex = new(
            "^(audio|video)/(.+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        private static readonly Regex RtxMimeTypeRegex = new("^.+/rtx$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly byte[] DynamicPayloadTypes = new byte[]
        {
            100,
            101,
            102,
            103,
            104,
            105,
            106,
            107,
            108,
            109,
            110,
            111,
            112,
            113,
            114,
            115,
            116,
            117,
            118,
            119,
            120,
            121,
            122,
            123,
            124,
            125,
            126,
            127,
            96,
            97,
            98,
            99,
        };

        /// <summary>
        /// Validates RtpCapabilities. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateRtpCapabilities(RtpCapabilities caps)
        {
            if (caps == null)
            {
                throw new ArgumentNullException(nameof(caps));
            }

            caps.Codecs ??= new List<RtpCodecCapability>();

            foreach (var codec in caps.Codecs)
            {
                ValidateRtpCodecCapability(codec);
            }

            // headerExtensions is optional. If unset, fill with an empty array.
            caps.HeaderExtensions ??= Array.Empty<RtpHeaderExtension>();

            foreach (var ext in caps.HeaderExtensions)
            {
                ValidateRtpHeaderExtension(ext);
            }
        }

        /// <summary>
        /// Validates RtpCodecCapability. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateRtpCodecCapability(RtpCodecCapability codec)
        {
            // mimeType is mandatory.
            if (codec.MimeType.IsNullOrWhiteSpace())
            {
                throw new ArgumentException($"{nameof(codec.MimeType)} can't be null or white space.");
            }

            var mimeType = codec.MimeType.ToLower();
            if (!MimeTypeRegex.IsMatch(mimeType))
            {
                throw new ArgumentException($"{nameof(codec.MimeType)} is not matched.");
            }

            // Just override kind with media component in mimeType.
            codec.Kind = mimeType.StartsWith("video") ? MediaKind.VIDEO : MediaKind.AUDIO;

            // preferredPayloadType is optional.
            // 在 Node.js 实现中，判断了 preferredPayloadType 在有值的情况下的数据类型。在强类型语言中不需要。

            // clockRate is mandatory.
            // 在 Node.js 实现中，判断了 mandatory 的数据类型。在强类型语言中不需要。

            // channels is optional. If unset, set it to 1 (just if audio).
            if (codec.Kind == MediaKind.AUDIO && (!codec.Channels.HasValue || codec.Channels < 1))
            {
                codec.Channels = 1;
            }

            // parameters is optional. If unset, set it to an empty object.
            codec.Parameters ??= new Dictionary<string, object>();

            foreach (var item in codec.Parameters)
            {
                var key = item.Key;
                var value = item.Value;
                if (value == null)
                {
                    codec.Parameters[item.Key] = "";
                    value = "";
                }

                if (!value.IsStringType() && !value.IsNumericType())
                {
                    throw new ArgumentOutOfRangeException($"invalid codec parameter [key:{key}, value:{value}]");
                }

                // Specific parameters validation.
                if (key == "apt" && !value.IsNumericType())
                {
                    throw new ArgumentOutOfRangeException($"invalid codec apt parameter [key:{key}]");
                }
            }

            // rtcpFeedback is optional. If unset, set it to an empty array.
            codec.RtcpFeedback ??= new List<RtcpFeedbackT>(0);

            foreach (var fb in codec.RtcpFeedback)
            {
                ValidateRtcpFeedback(fb);
            }
        }

        /// <summary>
        /// Validates RtcpFeedback. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateRtcpFeedback(RtcpFeedbackT fb)
        {
            if (fb == null)
            {
                throw new ArgumentNullException(nameof(fb));
            }

            // type is mandatory.
            if (fb.Type.IsNullOrWhiteSpace())
            {
                throw new ArgumentException(nameof(fb.Type));
            }

            // parameter is optional. If unset set it to an empty string.
            if (fb.Parameter.IsNullOrWhiteSpace())
            {
                fb.Parameter = "";
            }
        }

        /// <summary>
        /// Validates RtpHeaderExtension. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateRtpHeaderExtension(RtpHeaderExtension ext)
        {
            if (ext == null)
            {
                throw new ArgumentNullException(nameof(ext));
            }

            // 在 Node.js 实现中，判断了 kind 的值。在强类型语言中不需要。

            // uri is mandatory.
            // if(ext.Uri.IsNullOrWhiteSpace())
            // {
            //     throw new ArgumentException($"{nameof(ext.Uri)} can't be null or white space.");
            // }

            // preferredId is mandatory.
            // 在 Node.js 实现中，判断了 preferredId 的数据类型。在强类型语言中不需要。

            // preferredEncrypt is optional. If unset set it to false.
            // if(!ext.PreferredEncrypt.HasValue)
            // {
            //     ext.PreferredEncrypt = false;
            // }

            // direction is optional. If unset set it to sendrecv.
            if (!ext.Direction.HasValue)
            {
                ext.Direction = RtpHeaderExtensionDirection.SendReceive;
            }
        }

        /// <summary>
        /// Validates RtpParameters. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateRtpParameters(RtpParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            // mid is optional.
            // 在 Node.js 实现中，判断了 mid 的数据类型。在强类型语言中不需要。

            // codecs is mandatory.
            if (parameters.Codecs == null)
            {
                throw new ArgumentNullException($"{nameof(parameters)}.Codecs");
            }

            foreach (var codec in parameters.Codecs)
            {
                ValidateRtpCodecParameters(codec);
            }

            // headerExtensions is optional. If unset, fill with an empty array.
            parameters.HeaderExtensions ??= new List<RtpHeaderExtensionParameters>();

            foreach (var ext in parameters.HeaderExtensions)
            {
                ValidateRtpHeaderExtensionParameters(ext);
            }

            // encodings is optional. If unset, fill with an empty array.
            // parameters.Encodings ??= new List<RtpEncodingParametersT>();

            foreach (var encoding in parameters.Encodings)
            {
                ValidateRtpEncodingParameters(encoding);
            }

            // rtcp is optional. If unset, fill with an empty object.
            // 对 RtcpParameters 序列化时，CNAME 为 null 会忽略，因为客户端库对其有校验。
            // parameters.Rtcp ??= new RtcpParametersT();
            ValidateRtcpParameters(parameters.Rtcp);
        }

        /// <summary>
        /// Validates RtpCodecParameters. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateRtpCodecParameters(RtpCodecParameters codec)
        {
            if (codec == null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            // mimeType is mandatory.
            if (codec.MimeType.IsNullOrWhiteSpace())
            {
                throw new ArgumentException($"{nameof(codec.MimeType)} can't be null or white space.");
            }

            var mimeType = codec.MimeType.ToLower();
            if (!MimeTypeRegex.IsMatch(mimeType))
            {
                throw new ArgumentException($"{nameof(codec.MimeType)} is not matched.");
            }

            // payloadType is mandatory.
            // 在 Node.js 实现中，判断了 payloadType 的数据类型。在强类型语言中不需要。

            // clockRate is mandatory.
            // 在 Node.js 实现中，判断了 clockRate 的数据类型。在强类型语言中不需要。

            // channels is optional. If unset, set it to 1 (just if audio).
            // 在 Node.js 实现中，如果是 `video` 会 delete 掉 Channels 。
            if (mimeType.StartsWith("audio") && (!codec.Channels.HasValue || codec.Channels < 1))
            {
                codec.Channels = 1;
            }

            // parameters is optional. If unset, set it to an empty object.
            codec.Parameters ??= new Dictionary<string, object>();

            foreach (var item in codec.Parameters)
            {
                var key = item.Key;
                var value = item.Value;
                if (value == null)
                {
                    codec.Parameters[item.Key] = "";
                    value = "";
                }

                if (!value.IsStringType() && !value.IsNumericType())
                {
                    throw new ArgumentOutOfRangeException($"invalid codec parameter [key:{key}, value:{value}]");
                }

                if (key == "apt" && !value.IsNumericType())
                {
                    throw new ArgumentOutOfRangeException($"invalid codec apt parameter [key:{key}]");
                }
            }

            // rtcpFeedback is optional. If unset, set it to an empty array.
            // codec.RtcpFeedback ??= new List<RtcpFeedbackT>(0);

            foreach (var fb in codec.RtcpFeedback)
            {
                ValidateRtcpFeedback(fb);
            }
        }

        /// <summary>
        /// Validates RtpHeaderExtensionParameteters. It may modify given data by adding
        /// missing fields with default values. It throws if invalid.
        /// </summary>
        public static void ValidateRtpHeaderExtensionParameters(RtpHeaderExtensionParameters ext)
        {
            if (ext == null)
            {
                throw new ArgumentNullException(nameof(ext));
            }

            // uri is mandatory.
            // if(ext.Uri.IsNullOrWhiteSpace())
            // {
            //     throw new ArgumentException($"{nameof(ext.Uri)} can't be null or white space.");
            // }

            // id is mandatory.
            // 在 Node.js 实现中，判断了 id 的数据类型。在强类型语言中不需要。

            // encrypt is optional. If unset set it to false.
            // if(!ext.Encrypt.HasValue)
            // {
            //     ext.Encrypt = false;
            // }

            // parameters is optional. If unset, set it to an empty object.
            ext.Parameters ??= new Dictionary<string, object>();

            foreach (var item in ext.Parameters)
            {
                var key = item.Key;
                var value = item.Value;

                if (value == null)
                {
                    ext.Parameters[item.Key] = "";
                    value = "";
                }

                if (!value.IsStringType() && !value.IsNumericType())
                {
                    throw new ArgumentOutOfRangeException($"invalid codec parameter[key:{key}, value:{value}]");
                }
            }
        }

        /// <summary>
        /// Validates RtpEncodingParameters. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateRtpEncodingParameters(RtpEncodingParametersT encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            // ssrc is optional.
            // 在 Node.js 实现中，判断了 ssrc 的数据类型。在强类型语言中不需要。

            // rid is optional.
            // 在 Node.js 实现中，判断了 rid 的数据类型。在强类型语言中不需要。

            // rtx is optional.
            // 在 Node.js 实现中，判断了 rtx 的数据类型。在强类型语言中不需要。
            if (encoding.Rtx != null)
            {
                // RTX ssrc is mandatory if rtx is present.
                // 在 Node.js 实现中，判断了 rtx.ssrc 的数据类型。在强类型语言中不需要。
            }

            // dtx is optional. If unset set it to false.
            // 在 Node.js 实现中，判断了 Dtx 的数据类型。在强类型语言中不需要。(RtpEncodingParametersT 和 RtpEncodingParameters 不同)
            // if(!encoding.Dtx.HasValue)
            // {
            //     encoding.Dtx = false;
            // }

            // scalabilityMode is optional.
            // 在 Node.js 实现中，判断了 scalabilityMode 的数据类型。在强类型语言中不需要。
        }

        /// <summary>
        /// Validates RtcpParameters. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateRtcpParameters(RtcpParametersT rtcp)
        {
            if (rtcp == null)
            {
                throw new ArgumentNullException(nameof(rtcp));
            }

            // cname is optional.
            // 在 Node.js 实现中，判断了 cname 的数据类型。在强类型语言中不需要。

            // reducedSize is optional. If unset set it to true.
            // if(!rtcp.ReducedSize.HasValue)
            // {
            //     rtcp.ReducedSize = true;
            // }
        }

        /// <summary>
        /// Validates SctpCapabilities. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateSctpCapabilities(SctpCapabilities caps)
        {
            if (caps == null)
            {
                throw new ArgumentNullException(nameof(caps));
            }

            // numStreams is mandatory.
            if (caps.NumStreams == null)
            {
                throw new ArgumentNullException($"{nameof(caps)}.NumStreams");
            }

            ValidateNumSctpStreams(caps.NumStreams);
        }

        /// <summary>
        /// Validates NumSctpStreams. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateNumSctpStreams(
            NumSctpStreamsT _ /*numStreams*/
        )
        {
            // OS is mandatory.
            // 在 Node.js 实现中，判断了 OS 的数据类型。在强类型语言中不需要。

            // MIS is mandatory.
            // 在 Node.js 实现中，判断了 MIS 的数据类型。在强类型语言中不需要。
        }

        /// <summary>
        /// Validates SctpParameters. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateSctpParameters(
            SctpParameters _ /*parameters*/
        )
        {
            // port is mandatory.
            // 在 Node.js 实现中，判断了 port 的数据类型。在强类型语言中不需要。

            // OS is mandatory.
            // 在 Node.js 实现中，判断了 OS 的数据类型。在强类型语言中不需要。

            // MIS is mandatory.
            // 在 Node.js 实现中，判断了 MIS 的数据类型。在强类型语言中不需要。

            // maxMessageSize is mandatory.
            // 在 Node.js 实现中，判断了 maxMessageSize 的数据类型。在强类型语言中不需要。
        }

        /// <summary>
        /// Validates SctpStreamParameters. It may modify given data by adding missing
        /// fields with default values.
        /// It throws if invalid.
        /// </summary>
        public static void ValidateSctpStreamParameters(SctpStreamParametersT parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            // streamId is mandatory.
            // 在 Node.js 实现中，判断了 streamId 的数据类型。在强类型语言中不需要。

            // ordered is optional.
            var orderedGiven = true;
            if (!parameters.Ordered.HasValue)
            {
                orderedGiven = false;
                parameters.Ordered = true;
            }

            // maxPacketLifeTime is optional.
            // 在 Node.js 实现中，判断了 maxPacketLifeTime 的数据类型。在强类型语言中不需要。

            // maxRetransmits is optional.
            // 在 Node.js 实现中，判断了 maxRetransmits 的数据类型。在强类型语言中不需要。

            if (parameters.MaxPacketLifeTime.HasValue && parameters.MaxRetransmits.HasValue)
            {
                throw new ArgumentException("cannot provide both maxPacketLifeTime and maxRetransmits");
            }

            if (
                orderedGiven
                && parameters.Ordered.Value
                && (parameters.MaxPacketLifeTime.HasValue || parameters.MaxRetransmits.HasValue)
            )
            {
                throw new ArgumentException("cannot be ordered with maxPacketLifeTime or maxRetransmits");
            }
            else if (!orderedGiven && (parameters.MaxPacketLifeTime.HasValue || parameters.MaxRetransmits.HasValue))
            {
                parameters.Ordered = false;
            }
        }

        /// <summary>
        /// Generate RTP capabilities for the Router based on the given media codecs and
        /// mediasoup supported RTP capabilities.
        /// </summary>
        public static RtpCapabilities GenerateRouterRtpCapabilities(RtpCodecCapability[] mediaCodecs)
        {
            if (mediaCodecs == null)
            {
                throw new ArgumentNullException(nameof(mediaCodecs));
            }

            // Normalize supported RTP capabilities.
            ValidateRtpCapabilities(RtpCapabilities.SupportedRtpCapabilities);

            var clonedSupportedRtpCapabilities = RtpCapabilities.SupportedRtpCapabilities.DeepClone();
            var dynamicPayloadTypes = DynamicPayloadTypes.DeepClone().ToList();
            var caps = new RtpCapabilities
            {
                Codecs = new List<RtpCodecCapability>(),
                HeaderExtensions = clonedSupportedRtpCapabilities.HeaderExtensions,
            };

            foreach (var mediaCodec in mediaCodecs)
            {
                // This may throw.
                ValidateRtpCodecCapability(mediaCodec);

                var matchedSupportedCodec =
                    clonedSupportedRtpCapabilities.Codecs!.FirstOrDefault(supportedCodec =>
                        MatchCodecs(mediaCodec, supportedCodec, false)
                    ) ?? throw new Exception($"media codec not supported[mimeType:{mediaCodec.MimeType}]");

                // Clone the supported codec.
                var codec = matchedSupportedCodec.DeepClone();

                // If the given media codec has preferredPayloadType, keep it.
                if (mediaCodec.PreferredPayloadType.HasValue)
                {
                    codec.PreferredPayloadType = mediaCodec.PreferredPayloadType;

                    // Also remove the pt from the list in available dynamic values.
                    dynamicPayloadTypes.Remove(codec.PreferredPayloadType.Value);
                }
                // Otherwise if the supported codec has preferredPayloadType, use it.
                else if (codec.PreferredPayloadType.HasValue)
                {
                    // No need to remove it from the list since it's not a dynamic value.
                }
                // Otherwise choose a dynamic one.
                else
                {
                    // Take the first available pt and remove it from the list.
                    var pt = dynamicPayloadTypes.FirstOrDefault();

                    if (pt == 0)
                    {
                        throw new Exception("cannot allocate more dynamic codec payload types");
                    }

                    dynamicPayloadTypes.RemoveAt(0);

                    codec.PreferredPayloadType = pt;
                }

                // Ensure there is not duplicated preferredPayloadType values.
                if (caps.Codecs.Any(c => c.PreferredPayloadType == codec.PreferredPayloadType))
                {
                    throw new Exception("duplicated codec.preferredPayloadType");
                }

                // Merge the media codec parameters.
                codec.Parameters = codec.Parameters!.Merge(mediaCodec.Parameters!);

                // Append to the codec list.
                caps.Codecs.Add(codec);

                // Add a RTX video codec if video.
                if (codec.Kind == MediaKind.VIDEO)
                {
                    // Take the first available pt and remove it from the list.
                    var pt = dynamicPayloadTypes.FirstOrDefault();

                    if (pt == 0)
                    {
                        throw new Exception("cannot allocate more dynamic codec payload types");
                    }

                    dynamicPayloadTypes.RemoveAt(0);

                    var rtxCodec = new RtpCodecCapability
                    {
                        Kind = codec.Kind,
                        MimeType = $"{codec.Kind.GetEnumMemberValue()}/rtx",
                        PreferredPayloadType = pt,
                        ClockRate = codec.ClockRate,
                        Parameters = new Dictionary<string, object> { { "apt", codec.PreferredPayloadType } },
                        RtcpFeedback = new List<RtcpFeedbackT>(0),
                    };

                    // Append to the codec list.
                    caps.Codecs.Add(rtxCodec);
                }
            }

            return caps;
        }

        /// <summary>
        /// <para>
        /// Get a mapping in codec payloads and encodings in the given Producer RTP
        /// parameters as values expected by the Router.
        /// </para>
        /// <para>It may throw if invalid or non supported RTP parameters are given.</para>
        /// </summary>
        public static RtpMappingT GetProducerRtpParametersMapping(RtpParameters parameters, RtpCapabilities caps)
        {
            var rtpMapping = new RtpMappingT { Codecs = new List<CodecMappingT>(), Encodings = new List<EncodingMappingT>() };

            // Match parameters media codecs to capabilities media codecs.
            var codecToCapCodec = new Dictionary<RtpCodecParameters, RtpCodecCapability>();

            foreach (var codec in parameters.Codecs)
            {
                if (IsRtxMimeType(codec.MimeType))
                {
                    continue;
                }

                // Search for the same media codec in capabilities.
                var matchedCapCodec = caps.Codecs!.FirstOrDefault(capCodec => MatchCodecs(codec, capCodec, true, true));
                codecToCapCodec[codec] =
                    matchedCapCodec
                    ?? throw new NotSupportedException(
                        $"Unsupported codec[mimeType:{codec.MimeType}, payloadType:{codec.PayloadType}, Channels:{codec.Channels}]"
                    );
            }

            // Match parameters RTX codecs to capabilities RTX codecs.
            foreach (var codec in parameters.Codecs)
            {
                if (!IsRtxMimeType(codec.MimeType))
                {
                    continue;
                }

                // Search for the associated media codec.
                var associatedMediaCodec =
                    parameters.Codecs.FirstOrDefault(mediaCodec =>
                        MatchCodecsWithPayloadTypeAndApt(mediaCodec.PayloadType, codec.Parameters!)
                    ) ?? throw new Exception($"missing media codec found for RTX PT {codec.PayloadType}");

                var capMediaCodec = codecToCapCodec[associatedMediaCodec];

                // Ensure that the capabilities media codec has a RTX codec.
                var associatedCapRtxCodec = caps.Codecs!.FirstOrDefault(capCodec =>
                    IsRtxMimeType(capCodec.MimeType)
                    && MatchCodecsWithPayloadTypeAndApt(capMediaCodec.PreferredPayloadType, capCodec.Parameters!)
                );
                codecToCapCodec[codec] =
                    associatedCapRtxCodec
                    ?? throw new Exception($"no RTX codec for capability codec PT {capMediaCodec.PreferredPayloadType}");
            }

            // Generate codecs mapping.
            foreach (var item in codecToCapCodec)
            {
                rtpMapping.Codecs.Add(
                    new CodecMappingT
                    {
                        PayloadType = item.Key.PayloadType,
                        MappedPayloadType = item.Value.PreferredPayloadType!.Value,
                    }
                );
            }

            // Generate encodings mapping.
            var mappedSsrc = Utils.GenerateRandomNumber();

            foreach (var encoding in parameters.Encodings)
            {
                var mappedEncoding = new EncodingMappingT
                {
                    MappedSsrc = mappedSsrc++,
                    Rid = encoding.Rid,
                    Ssrc = encoding.Ssrc,
                    ScalabilityMode = encoding.ScalabilityMode,
                };

                rtpMapping.Encodings.Add(mappedEncoding);
            }

            return rtpMapping;
        }

        /// <summary>
        /// Generate RTP parameters to be internally used by Consumers given the RTP
        /// parameters in a Producer and the RTP capabilities in the Router.
        /// </summary>
        public static RtpParameters GetConsumableRtpParameters(
            MediaKind kind,
            RtpParameters parameters,
            RtpCapabilities caps,
            RtpMappingT rtpMapping
        )
        {
            var consumableParams = new RtpParameters
            {
                Codecs = new List<RtpCodecParameters>(),
                HeaderExtensions = new List<RtpHeaderExtensionParameters>(),
                Encodings = new List<RtpEncodingParametersT>(),
                Rtcp = new RtcpParametersT(),
            };

            foreach (var codec in parameters.Codecs)
            {
                if (IsRtxMimeType(codec.MimeType))
                {
                    continue;
                }

                var consumableCodecPt = rtpMapping
                    .Codecs.Where(entry => entry.PayloadType == codec.PayloadType)
                    .Select(m => m.MappedPayloadType)
                    .FirstOrDefault();

                var matchedCapCodec = caps.Codecs!.FirstOrDefault(capCodec =>
                    capCodec.PreferredPayloadType == consumableCodecPt
                );

                var consumableCodec = new RtpCodecParameters
                {
                    MimeType = matchedCapCodec!.MimeType,
                    PayloadType = matchedCapCodec.PreferredPayloadType!.Value,
                    ClockRate = matchedCapCodec.ClockRate,
                    Channels = matchedCapCodec.Channels,
                    Parameters = codec.Parameters, // Keep the Producer codec parameters.
                    RtcpFeedback = matchedCapCodec.RtcpFeedback,
                };

                consumableParams.Codecs.Add(consumableCodec);

                var consumableCapRtxCodec = caps.Codecs!.FirstOrDefault(capRtxCodec =>
                    IsRtxMimeType(capRtxCodec.MimeType)
                    && MatchCodecsWithPayloadTypeAndApt(consumableCodec.PayloadType, capRtxCodec.Parameters!)
                );

                if (consumableCapRtxCodec != null)
                {
                    var consumableRtxCodec = new RtpCodecParameters
                    {
                        MimeType = consumableCapRtxCodec.MimeType,
                        PayloadType = consumableCapRtxCodec.PreferredPayloadType!.Value,
                        ClockRate = consumableCapRtxCodec.ClockRate,
                        Channels = consumableCapRtxCodec.Channels,
                        Parameters = consumableCapRtxCodec.Parameters, // Keep the Producer codec parameters.
                        RtcpFeedback = consumableCapRtxCodec.RtcpFeedback,
                    };

                    consumableParams.Codecs.Add(consumableRtxCodec);
                }
            }

            foreach (var capExt in caps.HeaderExtensions!)
            {
                // Just take RTP header extension that can be used in Consumers.
                if (
                    capExt.Kind != kind
                    || (
                        capExt.Direction != RtpHeaderExtensionDirection.SendReceive
                        && capExt.Direction != RtpHeaderExtensionDirection.SendOnly
                    )
                )
                {
                    continue;
                }

                var consumableExt = new RtpHeaderExtensionParameters
                {
                    Uri = capExt.Uri,
                    Id = capExt.PreferredId,
                    Encrypt = capExt.PreferredEncrypt,
                    Parameters = new Dictionary<string, object>(),
                };

                consumableParams.HeaderExtensions.Add(consumableExt);
            }

            // Clone Producer encodings since we'll mangle them.
            var consumableEncodings = parameters.Encodings!.DeepClone();

            for (var i = 0; i < consumableEncodings.Count; ++i)
            {
                var consumableEncoding = consumableEncodings[i];
                var mappedSsrc = rtpMapping.Encodings[i].MappedSsrc;

                // Remove useless fields.
                // 在 Node.js 实现中，rid, rtx, codecPayloadType 被 delete 了。
                consumableEncoding.Rid = null;
                consumableEncoding.Rtx = null;
                consumableEncoding.CodecPayloadType = null;

                // Set the mapped ssrc.
                consumableEncoding.Ssrc = mappedSsrc;

                consumableParams.Encodings.Add(consumableEncoding);
            }

            consumableParams.Rtcp = new RtcpParametersT { Cname = parameters.Rtcp!.Cname, ReducedSize = true };

            return consumableParams;
        }

        /// <summary>
        /// Check whether the given RTP capabilities can consume the given Producer.
        /// </summary>
        public static bool CanConsume(RtpParameters consumableParams, RtpCapabilities caps)
        {
            // This may throw.
            ValidateRtpCapabilities(caps);

            var matchingCodecs = new List<RtpCodecParameters>();

            foreach (var codec in consumableParams.Codecs)
            {
                var matchedCapCodec = caps.Codecs!.FirstOrDefault(capCodec => MatchCodecs(capCodec, codec, true));

                if (matchedCapCodec == null)
                {
                    continue;
                }

                matchingCodecs.Add(codec);
            }

            // Ensure there is at least one media codec.
            return matchingCodecs.Count != 0 && !IsRtxMimeType(matchingCodecs[0].MimeType);
        }

        /// <summary>
        /// <para>Generate RTP parameters for a specific Consumer.</para>
        /// <para>
        /// It reduces encodings to just one and takes into account given RTP capabilities
        /// to reduce codecs, codecs' RTCP feedback and header extensions, and also enables
        /// or disabled RTX.
        /// </para>
        /// </summary>
        public static RtpParameters GetConsumerRtpParameters(RtpParameters consumableParams, RtpCapabilities caps, bool pipe)
        {
            var consumerParams = new RtpParameters
            {
                Codecs = new List<RtpCodecParameters>(),
                HeaderExtensions = new List<RtpHeaderExtensionParameters>(),
                Encodings = new List<RtpEncodingParametersT>(),
                Rtcp = consumableParams.Rtcp,
            };

            foreach (var capCodec in caps.Codecs!)
            {
                ValidateRtpCodecCapability(capCodec);
            }

            var consumableCodecs = consumableParams.Codecs.DeepClone();

            var rtxSupported = false;

            foreach (var codec in consumableCodecs)
            {
                var matchedCapCodec = caps.Codecs.FirstOrDefault(capCodec => MatchCodecs(capCodec, codec, true));

                if (matchedCapCodec == null)
                {
                    continue;
                }

                codec.RtcpFeedback = matchedCapCodec.RtcpFeedback;

                consumerParams.Codecs.Add(codec);
            }

            // Must sanitize the list of matched codecs by removing useless RTX codecs.
            var codecsToRemove = new List<RtpCodecParameters>();
            foreach (var codec in consumerParams.Codecs)
            {
                if (IsRtxMimeType(codec.MimeType))
                {
                    if (!codec.Parameters!.TryGetValue("apt", out var apt))
                    {
                        throw new Exception("\"apt\" key is not exists.");
                    }

                    var apiInteger = 0;
                    var aptJsonElement = apt as JsonElement?;
                    apiInteger = aptJsonElement != null ? aptJsonElement.Value.GetInt32() : Convert.ToInt32(apt);

                    // Search for the associated media codec.
                    var associatedMediaCodec = consumerParams.Codecs.FirstOrDefault(mediaCodec =>
                        mediaCodec.PayloadType == apiInteger
                    );
                    if (associatedMediaCodec != null)
                    {
                        rtxSupported = true;
                    }
                    else
                    {
                        codecsToRemove.Add(codec);
                    }
                }
            }

            codecsToRemove.ForEach(m => consumerParams.Codecs.Remove(m));

            // Ensure there is at least one media codec.
            if (consumerParams.Codecs.Count == 0 || IsRtxMimeType(consumerParams.Codecs[0].MimeType))
            {
                throw new Exception("no compatible media codecs");
            }

            consumerParams.HeaderExtensions = consumableParams
                .HeaderExtensions!.Where(ext =>
                    caps.HeaderExtensions!.Any(capExt => capExt.PreferredId == ext.Id && capExt.Uri == ext.Uri)
                )
                .ToList();

            // Reduce codecs' RTCP feedback. Use Transport-CC if available, REMB otherwise.
            if (consumerParams.HeaderExtensions.Any(ext => ext.Uri == RtpHeaderExtensionUri.TransportWideCcDraft01))
            {
                foreach (var codec in consumerParams.Codecs)
                {
                    codec.RtcpFeedback = codec.RtcpFeedback.Where(fb => fb.Type != "goog-remb").ToList();
                }
            }
            else if (consumerParams.HeaderExtensions.Any(ext => ext.Uri == RtpHeaderExtensionUri.AbsSendTime))
            {
                foreach (var codec in consumerParams.Codecs)
                {
                    codec.RtcpFeedback = codec.RtcpFeedback.Where(fb => fb.Type != "transport-cc").ToList();
                }
            }
            else
            {
                foreach (var codec in consumerParams.Codecs)
                {
                    codec.RtcpFeedback = codec
                        .RtcpFeedback.Where(fb => fb.Type is not "transport-cc" and not "goog-remb")
                        .ToList();
                }
            }

            if (!pipe)
            {
                var consumerEncoding = new RtpEncodingParametersT { Ssrc = Utils.GenerateRandomNumber() };

                if (rtxSupported)
                {
                    consumerEncoding.Rtx = new RtxT { Ssrc = consumerEncoding.Ssrc.Value + 1 };
                }

                // If any in the consumableParams.Encodings has scalabilityMode, process it
                // (assume all encodings have the same value).
                var encodingWithScalabilityMode = consumableParams.Encodings.FirstOrDefault(encoding =>
                    !encoding.ScalabilityMode.IsNullOrWhiteSpace()
                );

                var scalabilityMode = encodingWithScalabilityMode?.ScalabilityMode;

                // If there is simulast, mangle spatial layers in scalabilityMode.
                if (consumableParams.Encodings.Count > 1)
                {
                    var scalabilityModeObject = ScalabilityMode.Parse(scalabilityMode!);

                    scalabilityMode = $"S{consumableParams.Encodings.Count}T{scalabilityModeObject.TemporalLayers}";
                }

                if (!scalabilityMode.IsNullOrWhiteSpace())
                {
                    consumerEncoding.ScalabilityMode = scalabilityMode;
                }

                // Use the maximum maxBitrate in any encoding and honor it in the Consumer's
                // encoding.
                var maxEncodingMaxBitrate = consumableParams.Encodings.Max(m => m.MaxBitrate);
                if (maxEncodingMaxBitrate > 0)
                {
                    consumerEncoding.MaxBitrate = maxEncodingMaxBitrate;
                }

                // Set a single encoding for the Consumer.
                consumerParams.Encodings.Add(consumerEncoding);
            }
            else
            {
                var consumableEncodings = consumableParams.Encodings.DeepClone();
                var baseSsrc = Utils.GenerateRandomNumber();
                var baseRtxSsrc = Utils.GenerateRandomNumber();

                for (var i = 0; i < consumableEncodings!.Count; ++i)
                {
                    var encoding = consumableEncodings[i];
                    encoding.Ssrc = baseSsrc + (uint)i;
                    encoding.Rtx = rtxSupported ? new RtxT { Ssrc = baseRtxSsrc + (uint)i } : null;

                    consumerParams.Encodings.Add(encoding);
                }
            }

            return consumerParams;
        }

        /// <summary>
        /// <para>Generate RTP parameters for a pipe Consumer.</para>
        /// <para>
        /// It keeps all original consumable encodings and removes support for BWE. If
        /// enableRtx is false, it also removes RTX and NACK support.
        /// </para>
        /// </summary>
        public static RtpParameters GetPipeConsumerRtpParameters(RtpParameters consumableParams, bool enableRtx = false)
        {
            var consumerParams = new RtpParameters
            {
                Codecs = new List<RtpCodecParameters>(),
                HeaderExtensions = new List<RtpHeaderExtensionParameters>(),
                Encodings = new List<RtpEncodingParametersT>(),
                Rtcp = consumableParams.Rtcp,
            };

            var consumableCodecs = consumableParams.Codecs.DeepClone();

            foreach (var codec in consumableCodecs)
            {
                if (!enableRtx && IsRtxMimeType(codec.MimeType))
                {
                    continue;
                }

                codec.RtcpFeedback = codec
                    .RtcpFeedback.Where(fb =>
                        (fb.Type == "nack" && fb.Parameter == "pli")
                        || (fb.Type == "ccm" && fb.Parameter == "fir")
                        || (enableRtx && fb.Type == "nack" && fb.Parameter.IsNullOrWhiteSpace())
                    )
                    .ToList();

                consumerParams.Codecs.Add(codec);
            }

            // Reduce RTP extensions by disabling transport MID and BWE related ones.
            consumerParams.HeaderExtensions = consumableParams
                .HeaderExtensions!.Where(ext =>
                    ext.Uri != RtpHeaderExtensionUri.Mid
                    && ext.Uri != RtpHeaderExtensionUri.AbsSendTime
                    && ext.Uri != RtpHeaderExtensionUri.TransportWideCcDraft01
                )
                .ToList();

            var consumableEncodings = consumableParams.Encodings.DeepClone();

            var baseSsrc = Utils.GenerateRandomNumber();
            var baseRtxSsrc = Utils.GenerateRandomNumber();

            for (var i = 0; i < consumableEncodings.Count; ++i)
            {
                var encoding = consumableEncodings[i];
                encoding.Ssrc = (uint)(baseSsrc + i);

                if (enableRtx)
                {
                    encoding.Rtx = new RtxT { Ssrc = (uint)(baseRtxSsrc + i) };
                }
                else
                {
                    // 在 Node.js 实现中，delete 了 rtx 。
                    encoding.Rtx = null;
                }

                consumerParams.Encodings.Add(encoding);
            }

            return consumerParams;
        }

        private static bool IsRtxMimeType(string mimeType)
        {
            return RtxMimeTypeRegex.IsMatch(mimeType);
        }

        /// <summary>
        /// key 要么都存在于 a 和 b，要么都不存在于 a 和 b。
        /// </summary>
        private static bool CheckDirectoryValueEquals(IDictionary<string, object> a, IDictionary<string, object> b, string key)
        {
            if (a != null && b != null)
            {
                var got1 = a.TryGetValue(key, out var aPacketizationMode);
                var got2 = b.TryGetValue(key, out var bPacketizationMode);
                // 同时存在但不相等
                if (got1 && got2 && !aPacketizationMode!.Equals(bPacketizationMode))
                {
                    return false;
                }
                // 其中之一存在
                else if (got1 ^ got2)
                {
                    return false;
                }
            }
            else if (a != null && b == null)
            {
                // b 为 null的情况下，确保不存在于 a
                var got = a.ContainsKey("packetization-mode");
                if (got)
                {
                    return false;
                }
            }
            else if (a == null && b != null)
            {
                // a 为 null的情况下，确保不存在于 b
                var got = b.ContainsKey("packetization-mode");
                if (got)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool MatchCodecs(RtpCodecBase aCodec, RtpCodecBase bCodec, bool strict = false, bool modify = false)
        {
            var aMimeType = aCodec.MimeType.ToLower();
            var bMimeType = bCodec.MimeType.ToLower();

            if (aMimeType != bMimeType || aCodec.ClockRate != bCodec.ClockRate || aCodec.Channels != bCodec.Channels)
            {
                return false;
            }

            // Per codec special checks.
            switch (aMimeType)
            {
                case "audio/multiopus":
                {
                    var aNumStreams = aCodec.Parameters!["num_streams"];
                    var bNumStreams = bCodec.Parameters!["num_streams"];

                    if (aNumStreams != bNumStreams)
                    {
                        return false;
                    }

                    var aCoupledStreams = aCodec.Parameters["coupled_streams"];
                    var bCoupledStreams = bCodec.Parameters["coupled_streams"];

                    if (aCoupledStreams != bCoupledStreams)
                    {
                        return false;
                    }

                    break;
                }
                case "video/h264":
                case "video/h264-svc":
                {
                    // If strict matching check profile-level-id.
                    if (strict)
                    {
                        if (!CheckDirectoryValueEquals(aCodec.Parameters!, aCodec.Parameters!, "packetization-mode"))
                        {
                            return false;
                        }

                        if (!H264ProfileLevelId.Utils.IsSameProfile(aCodec.Parameters!, bCodec.Parameters!))
                        {
                            return false;
                        }

                        string? selectedProfileLevelId;

                        try
                        {
                            selectedProfileLevelId = H264ProfileLevelId.Utils.GenerateProfileLevelIdForAnswer(
                                aCodec.Parameters!,
                                bCodec.Parameters!
                            );
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"MatchCodecs() | {ex.Message}");
                            return false;
                        }

                        if (modify)
                        {
                            if (!selectedProfileLevelId.IsNullOrWhiteSpace())
                            {
                                aCodec.Parameters!["profile-level-id"] = selectedProfileLevelId!;
                            }
                            else
                            {
                                aCodec.Parameters!.Remove("profile-level-id");
                            }
                        }
                    }

                    break;
                }
                case "video/vp9":
                {
                    if (strict)
                    {
                        if (!CheckDirectoryValueEquals(aCodec.Parameters!, aCodec.Parameters!, "profile-id"))
                        {
                            return false;
                        }
                    }

                    break;
                }

                default:
                    break;
            }

            return true;
        }

        private static bool MatchCodecsWithPayloadTypeAndApt(int? payloadType, IDictionary<string, object> parameters)
        {
            if (payloadType == null && parameters == null)
            {
                return true;
            }

            if (parameters == null)
            {
                return false;
            }

            if (!parameters.TryGetValue("apt", out var apt))
            {
                return false;
            }

            var aptJsonElement = apt as JsonElement?;
            var aptInteger = aptJsonElement != null ? aptJsonElement.Value.GetInt32() : Convert.ToInt32(apt);

            return payloadType == aptInteger;
        }
    }
}
