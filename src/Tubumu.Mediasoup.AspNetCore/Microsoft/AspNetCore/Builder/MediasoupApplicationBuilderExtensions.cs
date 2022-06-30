using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tubumu.Libuv;
using Tubumu.Mediasoup;
using Force.DeepCloner;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    public static class MediasoupApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMediasoup(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<MediasoupServer>();
            var mediasoupOptions = app.ApplicationServices.GetRequiredService<MediasoupOptions>();
            var defaultWebRtcServerSettings = mediasoupOptions.MediasoupSettings.WebRtcServerSettings;
            var mediasoupServer = app.ApplicationServices.GetRequiredService<MediasoupServer>();
            var numberOfWorkers = mediasoupOptions.MediasoupStartupSettings.NumberOfWorkers;
            numberOfWorkers = !numberOfWorkers.HasValue || numberOfWorkers <= 0 ? Environment.ProcessorCount : numberOfWorkers;

            if (mediasoupOptions.MediasoupStartupSettings.WorkerInProcess)
            {
                for (var c = 0; c < numberOfWorkers; c++)
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try
                        {
                            var threadId = Environment.CurrentManagedThreadId;
                            var worker = app.ApplicationServices.GetRequiredService<WorkerNative>();
                            worker.On("@success", async (_, _) =>
                            {
                                mediasoupServer.AddWorker(worker);
                                logger.LogInformation($"Worker[{threadId}] create success.");
                                if (mediasoupOptions.MediasoupStartupSettings.UseWebRtcServer)
                                {
                                    await CreateWebRtcServerAsync(worker, (ushort)c, defaultWebRtcServerSettings);
                                }
                            });
                            worker.Run();
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Worker create failure.");
                        }
                    });
                }
            }
            else
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Loop.Default.Run(() =>
                    {
                        for (var c = 0; c < numberOfWorkers; c++)
                        {
                            var worker = app.ApplicationServices.GetRequiredService<Worker>();
                            worker.On("@success", async (_, _) =>
                            {
                                mediasoupServer.AddWorker(worker);
                                logger.LogInformation($"Worker[{worker.ProcessId}] create success.");
                                if (mediasoupOptions.MediasoupStartupSettings.UseWebRtcServer)
                                {
                                    await CreateWebRtcServerAsync(worker, (ushort)c, defaultWebRtcServerSettings);
                                }
                            });
                        }
                    });
                });
            }

            return app;
        }

        private static Task<WebRtcServer> CreateWebRtcServerAsync(WorkerBase worker, ushort portIncrement, WebRtcServerSettings defaultWebRtcServerSettings)
        {
            var webRtcServerSettings = defaultWebRtcServerSettings.DeepClone();
            var listenInfos = webRtcServerSettings.ListenInfos;
            foreach (var listenInfo in listenInfos)
            {
                listenInfo.Port += portIncrement;
            }

            var webRtcServerOptions = new WebRtcServerOptions
            {
                ListenInfos = listenInfos,
                AppData = new Dictionary<string, object>()
            };
            return worker.CreateWebRtcServerAsync(webRtcServerOptions);
        }
    }

}
