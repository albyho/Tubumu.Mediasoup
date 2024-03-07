using System.Collections.Generic;
using System.Threading.Tasks;
using FBS.RtpParameters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tubumu.Mediasoup;
using Tubumu.Meeting.Server;
using Tubumu.Utils.Models;
using RtpCodecParameters = Tubumu.Mediasoup.RtpCodecParameters;
using RtpParameters = Tubumu.Mediasoup.RtpParameters;

namespace Tubumu.Meeting.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BroadcasterController : ControllerBase
    {
        private readonly ILogger<BroadcasterController> _logger;

        private readonly Scheduler _scheduler;

        public BroadcasterController(ILogger<BroadcasterController> logger, Scheduler scheduler)
        {
            _logger = logger;
            _scheduler = scheduler;
        }

        [HttpGet]
        public ApiResult Get()
        {
            return new ApiResult();
        }

        [HttpGet("Broadcast")]
        public async Task<ApiResult> Broadcast()
        {
            var roomId = "0";
            var deviceId = "100001@100001";
            var videoSsrc = 2222u;
            var audioSsrc = videoSsrc + 2;

            await _scheduler.LeaveAsync(deviceId);

            var joinRequest = new JoinRequest
            {
                RtpCapabilities = new RtpCapabilities(),
                DisplayName = $"Device:{deviceId}",
                Sources = new[] { "video:cam", "audio:mic" },
                AppData = new Dictionary<string, object> { ["type"] = "Device" },
            };

            _ = await _scheduler.JoinAsync(deviceId, "", null!, joinRequest);

            var joinRoomRequest = new JoinRoomRequest
            {
                RoomId = roomId,
            };
            _ = await _scheduler.JoinRoomAsync(deviceId, "", joinRoomRequest);

            var createPlainTransportRequest = new CreatePlainTransportRequest
            {
                Comedia = true,
                RtcpMux = false,
                Producing = true,
                Consuming = false,
            };
            var transport = await _scheduler.CreatePlainTransportAsync(deviceId, "", createPlainTransportRequest);

            // Audio: "{ \"codecs\": [{ \"mimeType\":\"audio/opus\", \"payloadType\":${AUDIO_PT}, \"clockRate\":48000, \"channels\":2, \"parameters\":{ \"sprop-stereo\":1 } }], \"encodings\": [{ \"ssrc\":${AUDIO_SSRC} }] }"
            // Video :"{ \"codecs\": [{ \"mimeType\":\"video/vp8\", \"payloadType\":${VIDEO_PT}, \"clockRate\":90000 }], \"encodings\": [{ \"ssrc\":${VIDEO_SSRC} }] }"
            var videoProduceRequest = new ProduceRequest
            {
                Kind = MediaKind.VIDEO,
                Source = "video",
                RtpParameters = new RtpParameters
                {
                    Codecs = new List<RtpCodecParameters>
                    {
                        new ()
                        {
                            MimeType = "video/h264",
                            PayloadType = 98,
                            ClockRate = 90000,
                        }
                    },
                    Encodings = new List<RtpEncodingParametersT> {
                        new ()
                        {
                            Ssrc = videoSsrc
                        }
                    },
                },
                AppData = new Dictionary<string, object>
                {
                    ["peerId"] = deviceId,
                }
            };
            _ = await _scheduler.ProduceAsync(deviceId, "", videoProduceRequest);

            var audioProduceRequest = new ProduceRequest
            {
                Kind = MediaKind.AUDIO,
                Source = "audio",
                RtpParameters = new RtpParameters
                {
                    Codecs = new List<RtpCodecParameters>
                    {
                        new ()
                        {
                        MimeType = "audio/PCMA",
                        PayloadType = 8,
                        ClockRate = 8000,
                        }
                    },
                    Encodings = new List<RtpEncodingParametersT> {
                        new ()
                        {
                            Ssrc = audioSsrc
                        }
                    },
                },
                AppData = new Dictionary<string, object>
                {
                    ["peerId"] = deviceId,
                }
            };
            _ = await _scheduler.ProduceAsync(deviceId, "", audioProduceRequest);

            var result = new CreatePlainTransportResult
            {
                TransportId = transport.TransportId,
                Address = transport.Data.Tuple.LocalAddress,
                Port = transport.Data.Tuple.LocalPort,
            };
            return new ApiResult<CreatePlainTransportResult>
            {
                Data = result
            };
        }
    }
}
