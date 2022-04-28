using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tubumu.Utils.Extensions;

namespace Tubumu.Mediasoup
{
    public class WorkerNative : IWorker
	{
        #region P/Invoke Channel

        private static readonly LibMediasoupWorkerNative.ChannelReadFreeFn _channelReadFree = ChannelReadFree;

        private static void ChannelReadFree(IntPtr message, uint messageLen, IntPtr messageCtx)
        {
            ;
        }

        private static readonly LibMediasoupWorkerNative.ChannelReadFn _channelRead = ChannelRead;

        private static LibMediasoupWorkerNative.ChannelReadFreeFn? ChannelRead(IntPtr message, IntPtr messageLen, IntPtr messageCtx,
            IntPtr handle, IntPtr ctx)
        {
            return null;
            return _channelReadFree;
        }

        private static readonly LibMediasoupWorkerNative.ChannelWriteFn _channelWrite = ChannelWrite;

        private static void ChannelWrite(string message, uint messageLen, IntPtr ctx)
        {
            ;
        }

        #endregion

        #region P/Invoke PayloadChannel

        private static readonly LibMediasoupWorkerNative.PayloadChannelReadFreeFn _payloadChannelReadFree = PayloadChannelReadFree;

        private static void PayloadChannelReadFree(IntPtr message, uint messageLen, IntPtr messageCtx)
        {
            ;
        }

        private static readonly LibMediasoupWorkerNative.PayloadChannelReadFn _payloadChannelRead = PayloadChannelRead;

        internal static LibMediasoupWorkerNative.PayloadChannelReadFreeFn? PayloadChannelRead(IntPtr message, IntPtr messageLen, IntPtr messageCtx,
            IntPtr payload, IntPtr payloadLen, IntPtr payloadCapacity,
            IntPtr handle, IntPtr ctx)
        {
            return null;
            return _payloadChannelReadFree;
        }

        private static readonly LibMediasoupWorkerNative.PayloadChannelWriteFn _payloadchannelWrite = PayloadChannelWrite;

        private static void PayloadChannelWrite(string message, uint messageLen,
            IntPtr payload, uint payloadLen,
            IntPtr ctx)
        {
            ;
        }

        #endregion

        private readonly IntPtr _ptr;
        private bool disposedValue;

        public Dictionary<string, object>? AppData => throw new NotImplementedException();

        public EventEmitter Observer => throw new NotImplementedException();

        public WorkerNative(ILoggerFactory loggerFactory, MediasoupOptions mediasoupOptions)
        {
            _ptr = IntPtr.Zero;// GCHandle.ToIntPtr(GCHandle.Alloc(this, GCHandleType.Pinned));

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

            var version = mediasoupOptions.MediasoupStartupSettings.MediasoupVersion;

            LibMediasoupWorkerNative.MediasoupWorkerRun(argv.Count - 1,
             argv.ToArray(),
             version,
             0,
             0,
             0,
             0,
             _channelRead,
             _ptr,
             _channelWrite,
             _ptr,
             _payloadChannelRead,
             _ptr,
             _payloadchannelWrite,
             _ptr
             );
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Router?> CreateRouterAsync(RouterOptions routerOptions)
        {
            throw new NotImplementedException();
        }

        public Task<string?> DumpAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetResourceUsageAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string?> UpdateSettingsAsync(WorkerUpdateableSettings workerUpdateableSettings)
        {
            throw new NotImplementedException();
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~WorkerNative()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

