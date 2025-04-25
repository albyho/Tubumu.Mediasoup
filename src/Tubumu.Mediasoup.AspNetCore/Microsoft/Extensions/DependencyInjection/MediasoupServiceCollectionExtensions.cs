using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using FBS.Transport;
using Microsoft.Extensions.Configuration;
using Tubumu.Mediasoup;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MediasoupServiceCollectionExtensions
    {
        public static IServiceCollection AddMediasoup(this IServiceCollection services, Action<MediasoupOptions>? configure)
        {
            var mediasoupOptions = MediasoupOptions.Default;
            return AddMediasoup(services, mediasoupOptions, configure);
        }

        public static IServiceCollection AddMediasoup(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<MediasoupOptions>? configure = null
        )
        {
            var mediasoupOptions = MediasoupOptions.Default;
            Configure(mediasoupOptions, configuration);
            return AddMediasoup(services, mediasoupOptions, configure);
        }

        public static IServiceCollection AddMediasoup(
            this IServiceCollection services,
            MediasoupOptions mediasoupOptions,
            Action<MediasoupOptions>? configure = null
        )
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
            var mediasoupStartupSettings = configuration.GetSection("MediasoupStartupSettings").Get<MediasoupStartupSettings>()!;
            var mediasoupSettings = configuration.GetSection("MediasoupSettings").Get<MediasoupSettings>()!;
            var workerSettings = mediasoupSettings.WorkerSettings;
            var routerSettings = mediasoupSettings.RouterSettings;
            var webRtcServerSettings = mediasoupSettings.WebRtcServerSettings;
            var webRtcTransportSettings = mediasoupSettings.WebRtcTransportSettings;
            var plainTransportSettings = mediasoupSettings.PlainTransportSettings;

            // MediasoupStartupSettings
            mediasoupOptions.MediasoupStartupSettings.MediasoupVersion = mediasoupStartupSettings.MediasoupVersion;
            mediasoupOptions.MediasoupStartupSettings.WorkerInProcess = mediasoupStartupSettings.WorkerInProcess;
            mediasoupOptions.MediasoupStartupSettings.WorkerPath = mediasoupStartupSettings.WorkerPath;
            mediasoupOptions.MediasoupStartupSettings.NumberOfWorkers =
                !mediasoupStartupSettings.NumberOfWorkers.HasValue || mediasoupStartupSettings.NumberOfWorkers <= 0
                    ? Environment.ProcessorCount
                    : mediasoupStartupSettings.NumberOfWorkers;
            mediasoupOptions.MediasoupStartupSettings.UseWebRtcServer = mediasoupStartupSettings.UseWebRtcServer;

            // WorkerSettings
            mediasoupOptions.MediasoupSettings.WorkerSettings.LogLevel = workerSettings.LogLevel;
            mediasoupOptions.MediasoupSettings.WorkerSettings.LogTags = workerSettings.LogTags;
            mediasoupOptions.MediasoupSettings.WorkerSettings.DtlsCertificateFile = workerSettings.DtlsCertificateFile;
            mediasoupOptions.MediasoupSettings.WorkerSettings.DtlsPrivateKeyFile = workerSettings.DtlsPrivateKeyFile;

            // RouterSettings
            if (routerSettings?.RtpCodecCapabilities.IsNullOrEmpty() == false)
            {
                mediasoupOptions.MediasoupSettings.RouterSettings = routerSettings;

                // Fix RtpCodecCapabilities[x].Parameters 。从配置文件反序列化时将数字转换成了字符串，而 mediasoup-worker 有严格的数据类型验证，故这里进行修正。
                foreach (var codec in routerSettings.RtpCodecCapabilities.Where(m => m.Parameters != null))
                {
                    foreach (var key in codec.Parameters!.Keys.ToArray())
                    {
                        var value = codec.Parameters[key];
                        if (int.TryParse(value?.ToString(), out var intValue))
                        {
                            codec.Parameters[key] = intValue;
                        }
                    }
                }
            }

            var localIPv4IPAddresses = IPAddressExtensions
                .GetLocalIPAddresses(AddressFamily.InterNetwork)
                .Where(m => m != IPAddress.Loopback)
                .ToArray();
            if (localIPv4IPAddresses.IsNullOrEmpty())
            {
                throw new ArgumentException("无法获取本机 IPv4 地址。");
            }

            // WebRtcServerSettings
            mediasoupOptions.MediasoupSettings.WebRtcServerSettings.ListenInfos = webRtcServerSettings.ListenInfos;
            // 如果没有设置 ListenInfos 则获取本机所有的 IPv4 地址进行设置。
            var webRtcServerListenInfos = mediasoupOptions.MediasoupSettings.WebRtcServerSettings.ListenInfos;
            if (webRtcServerListenInfos.IsNullOrEmpty())
            {
                webRtcServerListenInfos = (
                    from ip in localIPv4IPAddresses
                    let ipString = ip.ToString()
                    select new ListenInfoT
                    {
                        Protocol = Protocol.TCP,
                        Ip = ipString,
                        AnnouncedAddress = ipString,
                        Port = 44444,
                    }
                ).ToList();

                mediasoupOptions.MediasoupSettings.WebRtcServerSettings.ListenInfos = webRtcServerListenInfos;
            }
            else
            {
                foreach (var listenIp in webRtcServerListenInfos)
                {
                    if (listenIp.AnnouncedAddress.IsNullOrWhiteSpace())
                    {
                        // 如果没有设置 AnnouncedAddress：
                        // 如果 Ip 属性的值不是 Any 则赋值为 Ip 属性的值，否则取本机的任意一个 IPv4 地址进行设置。(注意：可能获取的并不是正确的 IP)
                        listenIp.AnnouncedAddress =
                            listenIp.Ip == IPAddress.Any.ToString() ? localIPv4IPAddresses[0].ToString() : listenIp.Ip;
                    }
                }
            }

            // WebRtcTransportSettings
            mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.ListenInfos = webRtcTransportSettings.ListenInfos;
            mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.InitialAvailableOutgoingBitrate =
                webRtcTransportSettings.InitialAvailableOutgoingBitrate;
            mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.MinimumAvailableOutgoingBitrate =
                webRtcTransportSettings.MinimumAvailableOutgoingBitrate;
            mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.MaxSctpMessageSize =
                webRtcTransportSettings.MaxSctpMessageSize;

            // 如果没有设置 ListenInfos 则获取本机所有的 IPv4 地址进行设置。
            var webRtcTransportListenInfos = mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.ListenInfos;
            if (webRtcTransportListenInfos.IsNullOrEmpty())
            {
                webRtcTransportListenInfos = (
                    from ip in localIPv4IPAddresses
                    let ipString = ip.ToString()
                    select new ListenInfoT { Ip = ipString, AnnouncedAddress = ipString }
                ).ToList();
                mediasoupOptions.MediasoupSettings.WebRtcTransportSettings.ListenInfos = webRtcTransportListenInfos;
            }
            else
            {
                foreach (var listenIp in webRtcTransportListenInfos)
                {
                    if (listenIp.AnnouncedAddress.IsNullOrWhiteSpace())
                    {
                        // 如果没有设置 AnnouncedAddress：
                        // 如果 Ip 属性的值不是 Any 则赋值为 Ip 属性的值，否则取本机的任意一个 IPv4 地址进行设置。(注意：可能获取的并不是正确的 IP)
                        listenIp.AnnouncedAddress =
                            listenIp.Ip == IPAddress.Any.ToString() ? localIPv4IPAddresses[0].ToString() : listenIp.Ip;
                    }
                }
            }

            // PlainTransportSettings
            mediasoupOptions.MediasoupSettings.PlainTransportSettings.ListenInfo = plainTransportSettings.ListenInfo;
            mediasoupOptions.MediasoupSettings.PlainTransportSettings.MaxSctpMessageSize =
                plainTransportSettings.MaxSctpMessageSize;

            var plainTransportlistenInfo = mediasoupOptions.MediasoupSettings.PlainTransportSettings.ListenInfo;
            if (plainTransportlistenInfo == null)
            {
                plainTransportlistenInfo = new ListenInfoT
                {
                    Ip = localIPv4IPAddresses[0].ToString(),
                    AnnouncedAddress = localIPv4IPAddresses[0].ToString(),
                };
                mediasoupOptions.MediasoupSettings.PlainTransportSettings.ListenInfo = plainTransportlistenInfo;
            }
            else if (plainTransportlistenInfo.AnnouncedAddress.IsNullOrWhiteSpace())
            {
                // 如果没有设置 AnnouncedAddress：
                // 如果 Ip 属性的值不是 Any 则赋值为 Ip 属性的值，否则取本机的任意一个 IPv4 地址进行设置。(注意：可能获取的并不是正确的 IP)
                plainTransportlistenInfo.AnnouncedAddress =
                    plainTransportlistenInfo.Ip == IPAddress.Any.ToString() ? localIPv4IPAddresses[0].ToString() : plainTransportlistenInfo.Ip;
            }
        }
    }
}
