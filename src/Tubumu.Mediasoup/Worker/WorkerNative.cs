using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tubumu.Utils.Extensions;

namespace Tubumu.Mediasoup
{
    public class WorkerNative : WorkerBase
    {
        private readonly string[] _argv;
        private readonly string _version;
        private readonly IntPtr _channlPtr;
        private readonly IntPtr _payloadChannlPtr;

        public WorkerNative(ILoggerFactory loggerFactory, MediasoupOptions mediasoupOptions) : base(loggerFactory, mediasoupOptions)
        {
            var workerSettings = mediasoupOptions.MediasoupSettings.WorkerSettings;
            var argv = new List<string>
            {
               "" // Ignore `workerPath`
            };
            if (workerSettings.LogLevel.HasValue)
            {
                argv.Add($"--logLevel={workerSettings.LogLevel.Value.GetEnumMemberValue()}");
            }
            if (!workerSettings.LogTags.IsNullOrEmpty())
            {
                workerSettings.LogTags!.ForEach(m => argv.Add($"--logTag={m.GetEnumMemberValue()}"));
            }
            if (workerSettings.RtcMinPort.HasValue)
            {
                argv.Add($"--rtcMinPort={workerSettings.RtcMinPort}");
            }
            if (workerSettings.RtcMaxPort.HasValue)
            {
                argv.Add($"--rtcMaxPort={workerSettings.RtcMaxPort}");
            }
            if (!workerSettings.DtlsCertificateFile.IsNullOrWhiteSpace())
            {
                argv.Add($"--dtlsCertificateFile={workerSettings.DtlsCertificateFile}");
            }
            if (!workerSettings.DtlsPrivateKeyFile.IsNullOrWhiteSpace())
            {
                argv.Add($"--dtlsPrivateKeyFile={workerSettings.DtlsPrivateKeyFile}");
            }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            argv.Add(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            _argv = argv.ToArray();
            _version = mediasoupOptions.MediasoupStartupSettings.MediasoupVersion;

            var threadId = Environment.CurrentManagedThreadId;

            _channel = new ChannelNative(_loggerFactory.CreateLogger<Channel>(), threadId);
            _channel.MessageEvent += OnChannelMessage;
            _channlPtr = GCHandle.ToIntPtr(GCHandle.Alloc(_channel, GCHandleType.Normal));

            _payloadChannel = new PayloadChannelNative(_loggerFactory.CreateLogger<PayloadChannel>(), threadId);
            _payloadChannlPtr = GCHandle.ToIntPtr(GCHandle.Alloc(_payloadChannel, GCHandleType.Normal));
        }

        public void Run()
        {
            var workerRunResult = LibMediasoupWorkerNative.MediasoupWorkerRun(_argv.Length - 1,
             _argv,
             _version,
             0,
             0,
             0,
             0,
             ChannelNative.OnChannelRead,
             _channlPtr,
             ChannelNative.OnChannelWrite,
             _channlPtr,
             PayloadChannelNative.OnPayloadChannelRead,
             _payloadChannlPtr,
             PayloadChannelNative.OnPayloadchannelWrite,
             _payloadChannlPtr
             );

            void OnExit()
            {
                if (workerRunResult == 42)
                {
                    _logger.LogError($"OnExit() | Worker run failed due to wrong settings");
                    Emit("@failure", new Exception("wrong settings"));
                }

                else if (workerRunResult == 0)
                {
                    _logger.LogError($"OnExit() | Worker process died unexpectedly");
                    Emit("died", new Exception($"Worker died unexpectedly"));
                }
                else
                {
                    _logger.LogError($"OnExit() | Worker run failed unexpectedly");
                    Emit("@failure", new Exception("unexpectedly"));
                }
            }

            OnExit();
        }

        public override Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Destory()
        {
            if (_channlPtr != IntPtr.Zero)
            {
                var handle = GCHandle.FromIntPtr(_channlPtr);
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }

            if (_payloadChannlPtr != IntPtr.Zero)
            {
                var handle = GCHandle.FromIntPtr(_payloadChannlPtr);
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        #region Event handles

        private void OnChannelMessage(string targetId, string @event, string? data)
        {
            if (@event != "running")
            {
                return;
            }

            _channel.MessageEvent -= OnChannelMessage;
            Emit("@success");
        }

        #endregion
    }
}
