using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Force.DeepCloner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tubumu.Libuv;
using Tubumu.Mediasoup;

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
                            var cLocal = c;
                            worker.On(
                                "@success",
                                async (_, _) =>
                                {
                                    mediasoupServer.AddWorker(worker);
                                    logger.LogInformation("Worker[{ThreadId}] create success.", threadId);
                                    if (mediasoupOptions.MediasoupStartupSettings.UseWebRtcServer)
                                    {
                                        await CreateWebRtcServerAsync(worker, (ushort)cLocal, defaultWebRtcServerSettings);
                                    }
                                }
                            );
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
                            var cLocal = c;
                            worker.On(
                                "@success",
                                async (_, _) =>
                                {
                                    mediasoupServer.AddWorker(worker);
                                    logger.LogInformation("Worker[{ProcessId}] create success.", worker.ProcessId);
                                    if (mediasoupOptions.MediasoupStartupSettings.UseWebRtcServer)
                                    {
                                        await CreateWebRtcServerAsync(worker, (ushort)cLocal, defaultWebRtcServerSettings);
                                    }
                                }
                            );
                        }
                    });
                });
            }

            return app;
        }

        private static Task<WebRtcServer> CreateWebRtcServerAsync(
            WorkerBase worker,
            ushort portIncrement,
            WebRtcServerSettings defaultWebRtcServerSettings
        )
        {
            var webRtcServerSettings = defaultWebRtcServerSettings.DeepClone();
            var listenInfos = webRtcServerSettings.ListenInfos;
            foreach (var listenInfo in listenInfos)
            {
                if (listenInfo.Port != 0)
                {
                    listenInfo.Port = (ushort)(listenInfo.Port + portIncrement);
                }
            }

            var webRtcServerOptions = new WebRtcServerOptions
            {
                ListenInfos = listenInfos,
                AppData = new Dictionary<string, object>(),
            };
            return worker.CreateWebRtcServerAsync(webRtcServerOptions);
        }
    }
}
