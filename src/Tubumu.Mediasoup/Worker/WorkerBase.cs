using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public abstract class WorkerBase : EventEmitter, IDisposable, IWorker
    {
        #region Protected Fields

        /// <summary>
        /// Logger factory for create logger.
        /// </summary>
        protected readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Logger.
        /// </summary>
        protected readonly ILogger<Worker> _logger;

        /// <summary>
        /// Channel instance.
        /// </summary>
        protected IChannel _channel;

        /// <summary>
        /// Router set.
        /// </summary>
        protected readonly List<Router> _routers = new();

        /// <summary>
        /// _routers locker.
        /// </summary>
        protected readonly object _routersLock = new();

        /// <summary>
        /// WebRtcServers set.
        /// </summary>
        protected readonly List<WebRtcServer> _webRtcServers = new();

        /// <summary>
        /// _webRtcServer locker.
        /// </summary>
        protected readonly object _webRtcServersLock = new();

        /// <summary>
        /// Closed flag.
        /// </summary>
        protected bool _closed;

        /// <summary>
        /// Close locker.
        /// </summary>
        protected readonly AsyncReaderWriterLock _closeLock = new();

        #endregion Protected Fields

        /// <summary>
        /// Custom app data.
        /// </summary>
        public Dictionary<string, object> AppData { get; }

        /// <summary>
        /// Observer instance.
        /// </summary>
        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits died - (error: Error)</para>
        /// <para>@emits @success</para>
        /// <para>@emits @failure - (error: Error)</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits newwebrtcserver - (webRtcServer: WebRtcServer)</para>
        /// <para>@emits newrouter - (router: Router)</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="mediasoupOptions"></param>
        public WorkerBase(ILoggerFactory loggerFactory, MediasoupOptions mediasoupOptions)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Worker>();

            var workerSettings = mediasoupOptions.MediasoupSettings.WorkerSettings;

            AppData = workerSettings.AppData ?? new Dictionary<string, object>();
        }

        public abstract Task CloseAsync();

        #region Request

        /// <summary>
        /// Dump Worker.
        /// </summary>
        public async Task<string> DumpAsync()
        {
            _logger.LogDebug("DumpAsync()");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Worker closed");
                }

                return (await _channel.RequestAsync(MethodId.WORKER_DUMP))!;
            }
        }

        /// <summary>
        /// Get mediasoup-worker process resource usage.
        /// </summary>
        public async Task<string> GetResourceUsageAsync()
        {
            _logger.LogDebug("GetResourceUsageAsync()");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Worker closed");
                }

                return (await _channel.RequestAsync(MethodId.WORKER_GET_RESOURCE_USAGE))!;
            }
        }

        /// <summary>
        /// Updates the worker settings in runtime. Just a subset of the worker settings can be updated.
        /// </summary>
        public async Task UpdateSettingsAsync(WorkerUpdateableSettings workerUpdateableSettings)
        {
            _logger.LogDebug("UpdateSettingsAsync()");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Worker closed");
                }

                var logTags = workerUpdateableSettings.LogTags ?? Array.Empty<WorkerLogTag>();
                var reqData = new
                {
                    LogLevel = (workerUpdateableSettings.LogLevel ?? WorkerLogLevel.None).GetEnumMemberValue(),
                    LogTags = logTags.Select(m => m.GetEnumMemberValue()),
                };

                // Fire and forget
                _channel.RequestAsync(MethodId.WORKER_UPDATE_SETTINGS, null, reqData).ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        /// <summary>
        /// Create a WebRtcServer.
        /// </summary>
        /// <returns></returns>
        public async Task<WebRtcServer> CreateWebRtcServerAsync(WebRtcServerOptions webRtcServerOptions)
        {
            _logger.LogDebug("CreateWebRtcServerAsync()");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Workder closed");
                }
                var reqData = new {
                    WebRtcServerId = Guid.NewGuid().ToString(),
                    webRtcServerOptions.ListenInfos
                };

                await _channel.RequestAsync(MethodId.WORKER_CREATE_WEBRTC_SERVER, null, reqData);

                var webRtcServer = new WebRtcServer(_loggerFactory,
                        new WebRtcServerInternal
                        {
                            WebRtcServerId = reqData.WebRtcServerId
                        },
                        _channel,
                        webRtcServerOptions.AppData
                    );

                lock (_webRtcServersLock)
                {
                    _webRtcServers.Add(webRtcServer);
                }

                webRtcServer.On("@close", (_, _) =>
                {
                    lock (_webRtcServersLock)
                    {
                        _webRtcServers.Remove(webRtcServer);
                    }
                    return Task.CompletedTask;
                });

                // Emit observer event.
                Observer.Emit("newwebrtcserver", webRtcServer);

                return webRtcServer;
            }
        }

        /// <summary>
        /// Create a Router.
        /// </summary>
        public async Task<Router> CreateRouterAsync(RouterOptions routerOptions)
        {
            _logger.LogDebug("CreateRouterAsync()");

            using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Workder closed");
                }

                // This may throw.
                var rtpCapabilities = ORTC.GenerateRouterRtpCapabilities(routerOptions.MediaCodecs);

                var reqData = new { RouterId = Guid.NewGuid().ToString() };

                await _channel.RequestAsync(MethodId.WORKER_CREATE_ROUTER, null, reqData);

                var router = new Router(_loggerFactory,
                    new RouterInternal(reqData.RouterId),
                    new RouterData
                    {
                        RtpCapabilities = rtpCapabilities
                    },
                    _channel,
                    routerOptions.AppData);

                lock (_routersLock)
                {
                    _routers.Add(router);
                }

                router.On("@close", (_, _) =>
                {
                    lock (_routersLock)
                    {
                        _routers.Remove(router);
                    }
                    return Task.CompletedTask;
                });

                // Emit observer event.
                Observer.Emit("newrouter", router);

                return router;
            }
        }

        #endregion Request

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void DestoryManaged()
        {

        }

        protected virtual void DestoryUnmanaged()
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    DestoryManaged();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                DestoryUnmanaged();

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~WorkerBase()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
             GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
