using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using Tubumu.Utils.Extensions;
using Tubumu.Libuv;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// A worker represents a mediasoup C++ subprocess that runs in a single CPU core and handles Router instances.
    /// </summary>
    public class Worker : EventEmitter, IDisposable, IWorker
    {
        #region Constants

        private const int StdioCount = 7;

        #endregion Constants

        #region Private Fields

        /// <summary>
        /// Logger factory for create logger.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<Worker> _logger;

        /// <summary>
        /// mediasoup-worker child process.
        /// </summary>
        private Process? _child;

        /// <summary>
        /// Worker process PID.
        /// </summary>
        public int ProcessId { get; private set; }

        /// <summary>
        /// Is spawn done?
        /// </summary>
        private bool _spawnDone;

        /// <summary>
        /// Channel instance.
        /// </summary>
        private readonly IChannel _channel;

        /// <summary>
        /// PayloadChannel instance.
        /// </summary>
        private readonly IPayloadChannel _payloadChannel;

        /// <summary>
        /// Pipes.
        /// </summary>
        private readonly UVStream[] _pipes;

        /// <summary>
        /// Routers set.
        /// </summary>
        private readonly List<Router> _routers = new();

        /// <summary>
        /// Locker.
        /// </summary>
        private readonly object _routersLock = new();

        #endregion Private Fields

        /// <summary>
        /// Closed flag.
        /// </summary>
        private bool _closed;

        /// <summary>
        /// Close locker.
        /// </summary>
        private readonly AsyncAutoResetEvent _closeLock = new();

        /// <summary>
        /// Custom app data.
        /// </summary>
        public Dictionary<string, object>? AppData { get; }

        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// <para>Events:</para>
        /// <para>@emits died - (error: Error)</para>
        /// <para>@emits @success</para>
        /// <para>@emits @failure - (error: Error)</para>
        /// <para>Observer events:</para>
        /// <para>@emits close</para>
        /// <para>@emits newrouter - (router: Router)</para>
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="hostEnvironment"></param>
        /// <param name="mediasoupOptions"></param>
        public Worker(ILoggerFactory loggerFactory, MediasoupOptions mediasoupOptions)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<Worker>();
            _closeLock.Set();

            var workerPath = mediasoupOptions.MediasoupStartupSettings.WorkerPath;
            if (workerPath.IsNullOrWhiteSpace())
            {
                // 见：https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
                string rid = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? "linux"
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                        ? "osx"
                        : RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                            ? "win"
                            : throw new NotSupportedException("Unsupported platform");
                var location = Assembly.GetEntryAssembly()!.Location;
                var directory = Path.GetDirectoryName(location)!;
                workerPath = Path.Combine(directory, "runtimes", rid, "native", "mediasoup-worker");
            }

            var workerSettings = mediasoupOptions.MediasoupSettings.WorkerSettings;

            AppData = workerSettings.AppData;

            var env = new[] { $"MEDIASOUP_VERSION={mediasoupOptions.MediasoupStartupSettings.MediasoupVersion}" };

            var args = new List<string>
            {
               workerPath
            };
            if (workerSettings.LogLevel.HasValue)
            {
                args.Add($"--logLevel={workerSettings.LogLevel.Value.GetEnumMemberValue()}");
            }
            if (!workerSettings.LogTags.IsNullOrEmpty())
            {
                workerSettings.LogTags!.ForEach(m => args.Add($"--logTag={m.GetEnumMemberValue()}"));
            }
            if (workerSettings.RtcMinPort.HasValue)
            {
                args.Add($"--rtcMinPort={workerSettings.RtcMinPort}");
            }
            if (workerSettings.RtcMaxPort.HasValue)
            {
                args.Add($"--rtcMaxPort={workerSettings.RtcMaxPort}");
            }
            if (!workerSettings.DtlsCertificateFile.IsNullOrWhiteSpace())
            {
                args.Add($"--dtlsCertificateFile={workerSettings.DtlsCertificateFile}");
            }
            if (!workerSettings.DtlsPrivateKeyFile.IsNullOrWhiteSpace())
            {
                args.Add($"--dtlsPrivateKeyFile={workerSettings.DtlsPrivateKeyFile}");
            }

            _logger.LogDebug($"Worker() | Spawning worker process: {args.JoinAsString(" ")}");

            _pipes = new Pipe[StdioCount];

            // fd 0 (stdin)   : Just ignore it. (忽略标准输入)
            // fd 1 (stdout)  : Pipe it for 3rd libraries that log their own stuff.
            // fd 2 (stderr)  : Same as stdout.
            // fd 3 (channel) : Producer Channel fd.
            // fd 4 (channel) : Consumer Channel fd.
            // fd 5 (channel) : Producer PayloadChannel fd.
            // fd 6 (channel) : Consumer PayloadChannel fd.
            for (var i = 1; i < StdioCount; i++)
            {
                _pipes[i] = new Pipe() { Writeable = true, Readable = true };
            }

            try
            {
                // 和 Node.js 不同，_child 没有 error 事件。不过，Process.Spawn 可抛出异常。
                _child = Process.Spawn(new ProcessOptions()
                {
                    File = workerPath,
                    Arguments = args.ToArray(),
                    Environment = env,
                    Detached = false,
                    Streams = _pipes,
                }, OnExit);

                ProcessId = _child.Id;
            }
            catch (Exception ex)
            {
                _child = null;
                CloseAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                if (!_spawnDone)
                {
                    _spawnDone = true;
                    _logger.LogError(ex, $"Worker() | Worker process failed [pid:{ProcessId}]");
                    Emit("@failure", ex);
                }
                else
                {
                    // 执行到这里的可能性？
                    _logger.LogError(ex, $"Worker() | Worker process error [pid:{ProcessId}]");
                    Emit("died", ex);
                }
            }

            _channel = new Channel(_loggerFactory.CreateLogger<Channel>(), _pipes[3], _pipes[4], ProcessId);
            _channel.MessageEvent += OnChannelMessage;

            _payloadChannel = new PayloadChannel(_loggerFactory.CreateLogger<PayloadChannel>(), _pipes[5], _pipes[6], ProcessId);

            _pipes.ForEach(m => m?.Resume());
        }

        public async Task CloseAsync()
        {
            if (_closed)
            {
                return;
            }

            await _closeLock.WaitAsync();
            try
            {
                if (_closed)
                {
                    return;
                }

                _logger.LogDebug("CloseAsync() | Worker");

                _closed = true;

                // Kill the worker process.
                if (_child != null)
                {
                    // Remove event listeners but leave a fake 'error' hander to avoid
                    // propagation.
                    _child.Kill(15/*SIGTERM*/);
                    _child = null;
                }

                // Close the Channel instance.
                _channel?.Close();

                // Close the PayloadChannel instance.
                _payloadChannel?.Close();

                // Close every Router.
                Router[] routersForClose;
                lock (_routersLock)
                {
                    routersForClose = _routers.ToArray();
                    _routers.Clear();
                }

                foreach (var router in routersForClose)
                {
                    await router.WorkerClosedAsync();
                }

                // Emit observer event.
                Observer.Emit("close");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseAsync()");
            }
            finally
            {
                _closeLock.Set();
            }
        }

        #region Request

        /// <summary>
        /// Dump Worker.
        /// </summary>
        public async Task<string?> DumpAsync()
        {
            if (_closed)
            {
                return null;
            }

            await _closeLock.WaitAsync();
            try
            {
                if (_closed)
                {
                    return null;
                }

                _logger.LogDebug("DumpAsync()");
                return await _channel.RequestAsync(MethodId.WORKER_DUMP);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DumpAsync()");
                return null;
            }
            finally
            {
                _closeLock.Set();
            }
        }

        /// <summary>
        /// Get mediasoup-worker process resource usage.
        /// </summary>
        public async Task<string?> GetResourceUsageAsync()
        {
            if (_closed)
            {
                return null;
            }

            await _closeLock.WaitAsync();
            try
            {
                if (_closed)
                {
                    return null;
                }

                _logger.LogDebug("GetResourceUsageAsync()");
                return await _channel.RequestAsync(MethodId.WORKER_GET_RESOURCE_USAGE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetResourceUsageAsync()");
                return null;
            }
            finally
            {
                _closeLock.Set();
            }
        }

        /// <summary>
        /// Updates the worker settings in runtime. Just a subset of the worker settings can be updated.
        /// </summary>
        public async Task<string?> UpdateSettingsAsync(WorkerUpdateableSettings workerUpdateableSettings)
        {
            if (_closed)
            {
                return null;
            }

            await _closeLock.WaitAsync();
            try
            {
                if (_closed)
                {
                    return null;
                }

                _logger.LogDebug("UpdateSettingsAsync()");

                var logTags = workerUpdateableSettings.LogTags ?? Array.Empty<WorkerLogTag>();
                var reqData = new
                {
                    LogLevel = (workerUpdateableSettings.LogLevel ?? WorkerLogLevel.None).GetEnumMemberValue(),
                    LogTags = logTags.Select(m => m.GetEnumMemberValue()),
                };
                return await _channel.RequestAsync(MethodId.WORKER_UPDATE_SETTINGS, null, reqData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateSettingsAsync()");
                return null;
            }
            finally
            {
                _closeLock.Set();
            }
        }

        /// <summary>
        /// Create a Router.
        /// </summary>
        public async Task<Router?> CreateRouterAsync(RouterOptions routerOptions)
        {
            if (_closed)
            {
                return null;
            }

            await _closeLock.WaitAsync();
            try
            {
                if (_closed)
                {
                    return null;
                }

                _logger.LogDebug("CreateRouterAsync()");

                // This may throw.
                var rtpCapabilities = ORTC.GenerateRouterRtpCapabilities(routerOptions.MediaCodecs);

                var @internal = new { RouterId = Guid.NewGuid().ToString() };

                await _channel.RequestAsync(MethodId.WORKER_CREATE_ROUTER, @internal);

                var router = new Router(_loggerFactory, @internal.RouterId, rtpCapabilities, _channel, _payloadChannel, AppData);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateRouterAsync()");
                return null;
            }
            finally
            {
                _closeLock.Set();
            }
        }

        #endregion Request

        #region Event handles

        private void OnChannelMessage(string targetId, string @event, string? data)
        {
            if (@event != "running")
            {
                return;
            }

            _channel.MessageEvent -= OnChannelMessage;

            if (!_spawnDone)
            {
                _spawnDone = true;
                Emit("@success");
            }
        }

        private void OnExit(Process process)
        {
            _child = null;
            CloseAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            if (!_spawnDone)
            {
                _spawnDone = true;

                if (process.ExitCode == 42)
                {
                    _logger.LogError($"OnExit() | Worker process failed due to wrong settings [pid:{ProcessId}]");
                    Emit("@failure", new Exception("wrong settings"));
                }
                else
                {
                    _logger.LogError($"OnExit() | Worker process failed unexpectedly [pid:{ProcessId}, code:{process.ExitCode}, signal:{process.TermSignal}]");
                    Emit("@failure", new Exception($"[pid:{ProcessId}, code:{process.ExitCode}, signal:{process.TermSignal}]"));
                }
            }
            else
            {
                _logger.LogError($"OnExit() | Worker process died unexpectedly [pid:{ProcessId}, code:{process.ExitCode}, signal:{process.TermSignal}]");
                Emit("died", new Exception($"Worker process died unexpectedly [pid:{ProcessId}, code:{process.ExitCode}, signal:{process.TermSignal}]"));
            }
        }

        #endregion Event handles

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    _child?.Dispose();
                    _pipes.ForEach(m => m?.Dispose());
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Worker()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
