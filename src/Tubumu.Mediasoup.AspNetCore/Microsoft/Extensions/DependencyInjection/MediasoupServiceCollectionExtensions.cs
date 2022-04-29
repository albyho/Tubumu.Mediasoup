using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Tubumu.Mediasoup;
using Tubumu.Utils.Extensions;
using Tubumu.Utils.Extensions.Ip;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MediasoupServiceCollectionExtensions
    {
        public static IServiceCollection AddMediasoup(this IServiceCollection services, Action<MediasoupOptions>? configure)
        {
            var mediasoupOptions = MediasoupOptions.Default;
            return AddMediasoup(services, mediasoupOptions, configure);
        }

        public static IServiceCollection AddMediasoup(this IServiceCollection services, IConfiguration configuration, Action<MediasoupOptions>? configure = null)
        {
            var mediasoupOptions = MediasoupOptions.Default;
            Configure(mediasoupOptions, configuration);
            return AddMediasoup(services, mediasoupOptions, configure);

        }

        public static IServiceCollection AddMediasoup(this IServiceCollection services, MediasoupOptions mediasoupOptions, Action<MediasoupOptions>? configure = null)
        {
            configure?.Invoke(mediasoupOptions);
            services.AddSingleton(mediasoupOptions);
            services.AddSingleton<MediasoupServer>();
            services.AddTransient<Worker>();
            services.AddTransient<WorkerNative>();
            return services;
        }

        private static void Configure(MediasoupOptions mediasoupOptions, IConfiguration configuration)
        {
            var mediasoupStartupSettings = configuration.GetSection("MediasoupStartupSettings").Get<MediasoupStartupSettings>();
            var mediasoupSettings = configuration.GetSection("MediasoupSettings").Get<MediasoupSettings>();
            var workerSettings = mediasoupSettings.WorkerSettings;
            var routerSettings = mediasoupSettings.RouterSettings;
            var webRtcTransportSettings = mediasoupSettings.WebRtcTransportSettings;
            var plainTransportSettings = mediasoupSettings.PlainTransportSettings;

            // MediasoupStartupSettings
            if (mediasoupStartupSettings != null)
            {
                mediasoupOptions.MediasoupStartupSettings.MediasoupVersion = mediasoupStartupSettings.MediasoupVersion;
                mediasoupOptions.MediasoupStartupSettings.WorkerInProcess = mediasoupStartupSettings.WorkerInProcess;
                mediasoupOptions.MediasoupStartupSettings.WorkerPath = mediasoupStartupSettings.WorkerPath;
                mediasoupOptions.MediasoupStartupSettings.NumberOfWorkers = !mediasoupStartupSettings.NumberOfWorkers.HasValue || mediasoupStartupSettings.NumberOfWorkers <= 0 ? Environment.ProcessorCount : mediasoupStartupSettings.NumberOfWorkers;
            }

            // WorkerSettings
            if (workerSettings != null)
            {
                mediasoupOptions.MediasoupSettings.WorkerSettings.LogLevel = workerSettings.LogLevel;
                mediasoupOptions.MediasoupSettings.WorkerSettings.LogTags = workerSettings.LogTags;
                mediasoupOptions.MediasoupSettings.WorkerSettings.RtcMinPort = workerSettings.RtcMinPort;
                mediasoupOptions.MediasoupSettings.WorkerSettings.RtcMaxPort = workerSettings.RtcMaxPort;
                mediasoupOptions.MediasoupSettings.WorkerSettings.DtlsCertificateFile = workerSettings.DtlsCertificateFile;
                mediasoupOptions.MediasoupSettings.WorkerSettings.DtlsPrivateKeyFile = workerSettings.DtlsPrivateKeyFile;
            }

            // RouteSettings
            if (routerSettings != null && !routerSettings.RtpCodecCapabilities.IsNullOrEmpty())
            {
                mediasoupOptions.MediasoupSettings.RouterSettings = routerSettings;

                // Fix RtpCodecCapabilities[x].Parameters 。从配置文件反序列化时将数字转换成了字符串，这里进行修正。
                foreach (var codec in routerSettings.RtpCodecCapabilities.Where(m => m.Parameters != null))
                {
                    foreach (var key in codec.Parameters.Keys.ToArray())
                    {
                        var value = codec.Parameters[key];
                        if (value != null && int.TryParse(value.ToString(), out var intValue))
                        {
                            codec.Parameters[key] = intValue;
                        }
                    }
                }
            }

            // WebRtcTransportSettings
            if (webRtcTransportSettings != null)
            {
                mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.ListenIps = webRtcTransportSettings.ListenIps;
                mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.InitialAvailableOutgoingBitrate = webRtcTransportSettings.InitialAvailableOutgoingBitrate;
                mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.MinimumAvailableOutgoingBitrate = webRtcTransportSettings.MinimumAvailableOutgoingBitrate;
                mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.MaxSctpMessageSize = webRtcTransportSettings.MaxSctpMessageSize;

                // 如果没有设置 ListenIps 则获取本机所有的 IPv4 地址进行设置。
                var listenIps = mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.ListenIps;
                if (listenIps.IsNullOrEmpty())
                {
                    var localIPv4IPAddresses = IPAddressExtensions.GetLocalIPAddresses(AddressFamily.InterNetwork).Where(m => m != IPAddress.Loopback);
                    if (EnumerableExtensions.IsNullOrEmpty(localIPv4IPAddresses))
                    {
                        throw new ArgumentException("无法获取本机 IPv4 配置 WebRtcTransport。");
                    }

                    listenIps = (from ip in localIPv4IPAddresses
                                 let ipString = ip.ToString()
                                 select new TransportListenIp
                                 {
                                     Ip = ipString,
                                     AnnouncedIp = ipString
                                 }).ToArray();
                    mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.ListenIps = listenIps;
                }
                else
                {
                    var localIPv4IPAddress = IPAddressExtensions.GetLocalIPv4IPAddress();
                    if (localIPv4IPAddress == null)
                    {
                        throw new ArgumentException("无法获取本机 IPv4 配置 WebRtcTransport。");
                    }

                    foreach (var listenIp in listenIps)
                    {
                        if (listenIp.AnnouncedIp.IsNullOrWhiteSpace())
                        {
                            // 如果没有设置 AnnouncedIp：
                            // 如果 Ip 属性的值不是 Any 则赋值为 Ip 属性的值，否则取本机的任意一个 IPv4 地址进行设置。(注意：可能获取的并不是正确的 IP)
                            listenIp.AnnouncedIp = listenIp.Ip == IPAddress.Any.ToString() ? localIPv4IPAddress.ToString() : listenIp.Ip;
                        }
                    }
                }
            }

            // PlainTransportSettings
            if (plainTransportSettings != null)
            {
                mediasoupOptions.MediasoupSettings.PlainTransportSettings.ListenIp = plainTransportSettings.ListenIp;
                mediasoupOptions.MediasoupSettings.PlainTransportSettings.MaxSctpMessageSize = plainTransportSettings.MaxSctpMessageSize;

                var localIPv4IPAddress = IPAddressExtensions.GetLocalIPv4IPAddress();
                if (localIPv4IPAddress == null)
                {
                    throw new ArgumentException("无法获取本机 IPv4 配置 PlainTransport。");
                }

                var listenIp = mediasoupOptions.MediasoupSettings.PlainTransportSettings.ListenIp;
                if (listenIp == null)
                {
                    listenIp = new TransportListenIp
                    {
                        Ip = localIPv4IPAddress.ToString(),
                        AnnouncedIp = localIPv4IPAddress.ToString(),
                    };
                    mediasoupOptions.MediasoupSettings.PlainTransportSettings.ListenIp = listenIp;
                }
                else if (listenIp.AnnouncedIp.IsNullOrWhiteSpace())
                {
                    // 如果没有设置 AnnouncedIp：
                    // 如果 Ip 属性的值不是 Any 则赋值为 Ip 属性的值，否则取本机的任意一个 IPv4 地址进行设置。(注意：可能获取的并不是正确的 IP)
                    listenIp.AnnouncedIp = listenIp.Ip == IPAddress.Any.ToString() ? localIPv4IPAddress.ToString() : listenIp.Ip;
                }
            }
        }
    }
}
