using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FBS.Notification;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
{
    public class WorkerNative : WorkerBase
    {
        private readonly string[] _argv;

        private readonly string _version;

        private readonly IntPtr _channelPtr;

        public WorkerNative(ILoggerFactory loggerFactory, MediasoupOptions mediasoupOptions)
            : base(loggerFactory, mediasoupOptions)
        {
            var workerSettings = mediasoupOptions.MediasoupSettings.WorkerSettings;
            var argv = new List<string>
            {
                "", // Ignore `workerPath`
            };

            if (workerSettings.LogLevel.HasValue)
            {
                argv.Add($"--logLevel={workerSettings.LogLevel.Value.GetEnumMemberValue()}");
            }

            if (!workerSettings.LogTags.IsNullOrEmpty())
            {
                workerSettings.LogTags!.ForEach(m => argv.Add($"--logTag={m.GetEnumMemberValue()}"));
            }

            if (!workerSettings.DtlsCertificateFile.IsNullOrWhiteSpace())
            {
                argv.Add($"--dtlsCertificateFile={workerSettings.DtlsCertificateFile}");
            }

            if (!workerSettings.DtlsPrivateKeyFile.IsNullOrWhiteSpace())
            {
                argv.Add($"--dtlsPrivateKeyFile={workerSettings.DtlsPrivateKeyFile}");
            }

            if (!workerSettings.LibwebrtcFieldTrials.IsNullOrWhiteSpace())
            {
                argv.Add($"--libwebrtcFieldTrials={workerSettings.LibwebrtcFieldTrials}");
            }

            if (workerSettings.DisableLiburing.HasValue && workerSettings.DisableLiburing.Value)
            {
                argv.Add($"--disableLiburing=true");
            }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            argv.Add(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            _argv = argv.ToArray();
            _version = mediasoupOptions.MediasoupStartupSettings.MediasoupVersion;

            var threadId = Environment.CurrentManagedThreadId;

            _channel = new ChannelNative(_loggerFactory.CreateLogger<ChannelNative>(), threadId);
            _channel.OnNotification += OnNotificationHandle;
            _channelPtr = GCHandle.ToIntPtr(GCHandle.Alloc(_channel, GCHandleType.Normal));
        }

        public void Run()
        {
            var workerRunResult = LibMediasoupWorkerNative.MediasoupWorkerRun(
                _argv.Length - 1,
                _argv,
                _version,
                0,
                0,
                ChannelNative.OnChannelRead,
                _channelPtr,
                ChannelNative.OnChannelWrite,
                _channelPtr
            );

            void OnExit()
            {
                if (workerRunResult == 42)
                {
                    _logger.LogError("OnExit() | Worker run failed due to wrong settings");
                    Emit("@failure", new Exception("Worker run failed due to wrong settings"));
                }
                else if (workerRunResult == 0)
                {
                    _logger.LogError("OnExit() | Worker died unexpectedly");
                    Emit("died", new Exception("Worker died unexpectedly"));
                }
                else
                {
                    _logger.LogError("OnExit() | Worker run failed unexpectedly");
                    Emit("@failure", new Exception("Worker run failed unexpectedly"));
                }
            }

            OnExit();
        }

        public override Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        protected override void DestoryUnmanaged()
        {
            if (_channelPtr != IntPtr.Zero)
            {
                var handle = GCHandle.FromIntPtr(_channelPtr);
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        #region Event handles

        private void OnNotificationHandle(string handlerId, Event @event, Notification notification)
        {
            if (@event != Event.WORKER_RUNNING)
            {
                return;
            }

            _channel.OnNotification -= OnNotificationHandle;
            Emit("@success");
        }

        #endregion
    }
}
