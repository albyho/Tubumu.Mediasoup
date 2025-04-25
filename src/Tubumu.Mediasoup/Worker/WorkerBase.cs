using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBS.Request;
using FBS.Transport;
using FBS.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public abstract class WorkerBase : EventEmitter, IWorker
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
        protected WorkerBase(ILoggerFactory loggerFactory, MediasoupOptions mediasoupOptions)
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
        public async Task<DumpResponseT> DumpAsync()
        {
            _logger.LogDebug("DumpAsync()");

            await using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Worker closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var response = await _channel.RequestAsync(bufferBuilder, Method.WORKER_DUMP);
                var data = response!.Value.BodyAsWorker_DumpResponse().UnPack();
                return data;
            }
        }

        /// <summary>
        /// Get mediasoup-worker process resource usage.
        /// </summary>
        public async Task<ResourceUsageResponseT> GetResourceUsageAsync()
        {
            _logger.LogDebug("GetResourceUsageAsync()");

            await using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Worker closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var response = await _channel.RequestAsync(bufferBuilder, Method.WORKER_GET_RESOURCE_USAGE);
                var data = response!.Value.BodyAsWorker_ResourceUsageResponse().UnPack();

                return data;
            }
        }

        /// <summary>
        /// Updates the worker settings in runtime. Just a subset of the worker settings can be updated.
        /// </summary>
        public async Task UpdateSettingsAsync(WorkerUpdateableSettings workerUpdateableSettings)
        {
            _logger.LogDebug("UpdateSettingsAsync()");

            await using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Worker closed");
                }

                var logLevel = workerUpdateableSettings.LogLevel ?? WorkerLogLevel.None;
                var logLevelString = logLevel.GetEnumMemberValue();
                var logTags = workerUpdateableSettings.LogTags ?? [];
                var logTagStrings = logTags.Select(m => m.GetEnumMemberValue()).ToList();

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var updateSettingsRequestT = new UpdateSettingsRequestT { LogLevel = logLevelString, LogTags = logTagStrings };

                var requestOffset = UpdateSettingsRequest.Pack(bufferBuilder, updateSettingsRequestT);

                // Fire and forget
                _channel
                    .RequestAsync(
                        bufferBuilder,
                        Method.WORKER_UPDATE_SETTINGS,
                        Body.Worker_UpdateSettingsRequest,
                        requestOffset.Value
                    )
                    .ContinueWithOnFaultedHandleLog(_logger);
            }
        }

        /// <summary>
        /// Create a WebRtcServer.
        /// </summary>
        /// <returns>WebRtcServer</returns>
        public async Task<WebRtcServer> CreateWebRtcServerAsync(WebRtcServerOptions webRtcServerOptions)
        {
            _logger.LogDebug("CreateWebRtcServerAsync()");

            await using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Workder closed");
                }

                var webRtcServerId = Guid.NewGuid().ToString();

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var createWebRtcServerRequestT = new CreateWebRtcServerRequestT
                {
                    WebRtcServerId = webRtcServerId,
                    ListenInfos = webRtcServerOptions.ListenInfos,
                };

                var createWebRtcServerRequestOffset = CreateWebRtcServerRequest.Pack(bufferBuilder, createWebRtcServerRequestT);

                await _channel.RequestAsync(
                    bufferBuilder,
                    Method.WORKER_CREATE_WEBRTCSERVER,
                    Body.Worker_CreateWebRtcServerRequest,
                    createWebRtcServerRequestOffset.Value
                );

                var webRtcServer = new WebRtcServer(
                    _loggerFactory,
                    new WebRtcServerInternal { WebRtcServerId = webRtcServerId },
                    _channel,
                    webRtcServerOptions.AppData
                );

                lock (_webRtcServersLock)
                {
                    _webRtcServers.Add(webRtcServer);
                }

                webRtcServer.On(
                    "@close",
                    (_, _) =>
                    {
                        lock (_webRtcServersLock)
                        {
                            _webRtcServers.Remove(webRtcServer);
                        }

                        return Task.CompletedTask;
                    }
                );

                // Emit observer event.
                Observer.Emit("newwebrtcserver", webRtcServer);

                return webRtcServer;
            }
        }

        /// <summary>
        /// Create a Router.
        /// </summary>
        /// <returns>Router</returns>
        public async Task<Router> CreateRouterAsync(RouterOptions routerOptions)
        {
            _logger.LogDebug("CreateRouterAsync()");

            await using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Workder closed");
                }

                // This may throw.
                var rtpCapabilities = ORTC.GenerateRouterRtpCapabilities(routerOptions.MediaCodecs);

                var routerId = Guid.NewGuid().ToString();

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var createRouterRequestT = new CreateRouterRequestT { RouterId = routerId };

                var createRouterRequestOffset = CreateRouterRequest.Pack(bufferBuilder, createRouterRequestT);

                await _channel.RequestAsync(
                    bufferBuilder,
                    Method.WORKER_CREATE_ROUTER,
                    Body.Worker_CreateRouterRequest,
                    createRouterRequestOffset.Value
                );

                var router = new Router(
                    _loggerFactory,
                    new RouterInternal(routerId),
                    new RouterData { RtpCapabilities = rtpCapabilities },
                    _channel,
                    routerOptions.AppData
                );

                lock (_routersLock)
                {
                    _routers.Add(router);
                }

                router.On(
                    "@close",
                    (_, _) =>
                    {
                        lock (_routersLock)
                        {
                            _routers.Remove(router);
                        }

                        return Task.CompletedTask;
                    }
                );

                // Emit observer event.
                Observer.Emit("newrouter", router);

                return router;
            }
        }

        #endregion Request

        #region IDisposable Support

        private bool _disposedValue = false; // 要检测冗余调用

        protected virtual void DestroyManaged() { }

        protected virtual void DestoryUnmanaged() { }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    DestroyManaged();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                DestoryUnmanaged();

                _disposedValue = true;
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
