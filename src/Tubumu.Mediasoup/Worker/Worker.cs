using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tubumu.Libuv;

namespace Tubumu.Mediasoup
{
    /// <summary>
    /// A worker represents a mediasoup C++ subprocess that runs in a single CPU core and handles Router instances.
    /// </summary>
    public class Worker : WorkerBase
    {
        #region Constants

        private const int StdioCount = 7;

        #endregion Constants

        #region Private Fields

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
        /// Pipes.
        /// </summary>
        private readonly UVStream[] _pipes;

        #endregion Private Fields

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
        /// <param name="mediasoupOptions"></param>
        public Worker(ILoggerFactory loggerFactory, MediasoupOptions mediasoupOptions) : base(loggerFactory, mediasoupOptions)
        {
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

        public override async Task CloseAsync()
        {
            _logger.LogDebug("CloseAsync() | Worker");

            using (await _closeLock.WriteLockAsync())
            {
                if (_closed)
                {
                    throw new InvalidStateException("Worker closed");
                }


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
                if (_channel != null)
                {
                    await _channel.CloseAsync();
                }

                // Close the PayloadChannel instance.
                if (_payloadChannel != null)
                {
                    await _payloadChannel.CloseAsync();
                }

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

                // Close every WebRtcServer.
                WebRtcServer[] webRtcServersForClose;
                lock (_webRtcServersLock)
                {
                    webRtcServersForClose = _webRtcServers.ToArray();
                    _webRtcServers.Clear();
                }

                foreach (var webRtcServer in webRtcServersForClose)
                {
                    await webRtcServer.WorkerClosedAsync();
                }

                // Emit observer event.
                Observer.Emit("close");
            }
        }

        protected override void DestoryManaged()
        {
            _child?.Dispose();
            _pipes.ForEach(m => m?.Dispose());
        }

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
                    Emit("@failure", new Exception($"Worker process failed due to wrong settings [pid:{ProcessId}]"));
                }
                else
                {
                    _logger.LogError($"OnExit() | Worker process failed unexpectedly [pid:{ProcessId}, code:{process.ExitCode}, signal:{process.TermSignal}]");
                    Emit("@failure", new Exception($"Worker process failed unexpectedly [pid:{ProcessId}, code:{process.ExitCode}, signal:{process.TermSignal}]"));
                }
            }
            else
            {
                _logger.LogError($"OnExit() | Worker process died unexpectedly [pid:{ProcessId}, code:{process.ExitCode}, signal:{process.TermSignal}]");
                Emit("died", new Exception($"Worker process died unexpectedly [pid:{ProcessId}, code:{process.ExitCode}, signal:{process.TermSignal}]"));
            }
        }

        #endregion Event handles
    }
}
