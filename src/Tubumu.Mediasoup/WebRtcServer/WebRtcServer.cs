using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FBS.Request;
using FBS.WebRtcServer;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Tubumu.Mediasoup
{
    public class WebRtcServer : EventEmitter
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<WebRtcServer> _logger;

        #region Internal data.

        private readonly WebRtcServerInternal _internal;

        public string WebRtcServerId => _internal.WebRtcServerId;

        #endregion Internal data.

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly IChannel _channel;

        /// <summary>
        /// Closed flag.
        /// </summary>
        private bool _closed = false;

        /// <summary>
        /// Close locker.
        /// </summary>
        private readonly AsyncReaderWriterLock _closeLock = new();

        /// <summary>
        /// Custom app data.
        /// </summary>
        public Dictionary<string, object> AppData { get; }

        /// <summary>
        /// Transports map.
        /// </summary>
        private readonly Dictionary<string, WebRtcTransport> _webRtcTransports = new();
        private readonly AsyncReaderWriterLock _webRtcTransportsLock = new();

        /// <summary>
        /// Observer instance.
        /// </summary>
        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits workerclose</para>
        /// <para>@emits @close</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits webrtctransporthandled - (webRtcTransport: WebRtcTransport)</para>
        /// <para>@emits webrtctransportunhandled - (webRtcTransport: WebRtcTransport)</para>
        /// </summary>
        public WebRtcServer(
            ILoggerFactory loggerFactory,
            WebRtcServerInternal @internal,
            IChannel channel,
            Dictionary<string, object>? appData
        )
        {
            _logger = loggerFactory.CreateLogger<WebRtcServer>();

            _internal = @internal;
            _channel = channel;
            AppData = appData ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Close the WebRtcServer.
        /// </summary>
        public async Task CloseAsync()
        {
            _logger.LogDebug("CloseAsync() | WebRtcServerId:{WebRtcServerId}", WebRtcServerId);

            await using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var closeWebRtcServerRequest = new FBS.Worker.CloseWebRtcServerRequestT
                {
                    WebRtcServerId = _internal.WebRtcServerId,
                };

                var closeWebRtcServerRequestOffset = FBS.Worker.CloseWebRtcServerRequest.Pack(
                    bufferBuilder,
                    closeWebRtcServerRequest
                );

                // Fire and forget
                _channel
                    .RequestAsync(
                        bufferBuilder,
                        Method.WORKER_WEBRTCSERVER_CLOSE,
                        Body.Worker_CloseWebRtcServerRequest,
                        closeWebRtcServerRequestOffset.Value
                    )
                    .ContinueWithOnFaultedHandleLog(_logger);

                await CloseInternalAsync();

                Emit("@close");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        /// <summary>
        /// Worker was closed.
        /// </summary>
        public async Task WorkerClosedAsync()
        {
            _logger.LogDebug("WorkerClosedAsync() | WebRtcServerId:{WebRtcServerId}", WebRtcServerId);

            await using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    return;
                }

                _closed = true;

                await CloseInternalAsync();

                Emit("workerclose");

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        private async Task CloseInternalAsync()
        {
            await using (await _webRtcTransportsLock.WriteLockAsync())
            {
                // Close every WebRtcTransport.
                foreach (var webRtcTransport in _webRtcTransports.Values)
                {
                    await webRtcTransport.ListenServerClosedAsync();

                    // Emit observer event.
                    Observer.Emit("webrtctransportunhandled", webRtcTransport);
                }

                _webRtcTransports.Clear();
            }
        }

        /// <summary>
        /// Dump Router.
        /// </summary>
        public async Task<DumpResponseT> DumpAsync()
        {
            _logger.LogDebug("DumpAsync() | WebRtcServerId:{WebRtcServerId}", WebRtcServerId);

            await using (await _closeLock.ReadLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("WebRtcServer closed");
                }

                // Build Request
                var bufferBuilder = _channel.BufferPool.Get();

                var response = await _channel.RequestAsync(
                    bufferBuilder,
                    Method.WEBRTCSERVER_DUMP,
                    null,
                    null,
                    _internal.WebRtcServerId
                );

                /* Decode Response. */
                var data = response!.Value.BodyAsWebRtcServer_DumpResponse().UnPack();
                return data;
            }
        }

        public async Task HandleWebRtcTransportAsync(WebRtcTransport webRtcTransport)
        {
            await using (await _webRtcTransportsLock.WriteLockAsync())
            {
                _webRtcTransports[webRtcTransport.TransportId] = webRtcTransport;
            }

            // Emit observer event.
            Observer.Emit("webrtctransporthandled", webRtcTransport);

            webRtcTransport.On(
                "@close",
                async (_, _) =>
                {
                    await using (await _webRtcTransportsLock.WriteLockAsync())
                    {
                        _webRtcTransports.Remove(webRtcTransport.TransportId);
                    }

                    // Emit observer event.
                    Observer.Emit("webrtctransportunhandled", webRtcTransport);
                }
            );
        }
    }
}
