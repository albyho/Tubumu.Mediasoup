using System;
using System.Runtime.InteropServices;
using ChannelReadCtx = System.IntPtr;
using ChannelWriteCtx = System.IntPtr;
using size_t = System.IntPtr;

namespace Tubumu.Mediasoup
{
    internal static class LibMediasoupWorkerNative
    {
        internal const string Lib = "mediasoup-worker";

        #region P/Invoke Channel

        /// <summary>
        /// Free memory
        /// </summary>
        /// <param name="message">uint8_t*</param>
        /// <param name="messageLen">uint32_t</param>
        /// <param name="messageCtx">size_t</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ChannelReadFreeFn(IntPtr message, uint messageLen, size_t messageCtx);

        /// <summary>
        /// Write to Channel
        /// </summary>
        /// <param name="message">uint8_t**</param>
        /// <param name="messageLen">uint32_t*</param>
        /// <param name="messageCtx">size_t*</param>
        /// <param name="handle">const void*</param>
        /// <param name="ctx">void*</param>
        /// <returns>Returns `ChannelReadFree` on successful read that must be used to free `message`</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate ChannelReadFreeFn? ChannelReadFn(IntPtr message, IntPtr messageLen, size_t messageCtx,
            IntPtr handle, ChannelReadCtx ctx);

        /// <summary>
        /// Read from Channel
        /// </summary>
        /// <param name="message">const uint8_t*</param>
        /// <param name="messageLen">uint32_t</param>
        /// <param name="ctx">ChannelWriteCtx</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ChannelWriteFn(IntPtr message, uint messageLen, ChannelWriteCtx ctx);

        #endregion

        /// <summary>
        /// Call when wrote some messages to Channel or PayloadChannel
        /// </summary>
        /// <param name="handle">RequestMessage Queue handle</param>
        /// <returns>Returns `0` on success, or an error code `< 0` on failure</returns>
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_async_send(IntPtr handle);

        /// <summary>
        /// Run worker
        /// </summary>
        /// <param name="argc">Number of arguments</param>
        /// <param name="argv">The arguments</param>
        /// <param name="version">Medisaoup version</param>
        /// <param name="consumerChannelFd">Consumer channel fd, Zero.</param>
        /// <param name="producerChannelFd">Consumer channel fd, Zero.</param>
        /// <param name="_payloadConsumeChannelFd">Not used, Zero.</param>
        /// <param name="_payloadProduceChannelFd">Not used, Zero.</param>
        /// <param name="channelReadFn">Channel read function</param>
        /// <param name="channelReadCtx">Channel read function context</param>
        /// <param name="channelWriteFn">Channel write function</param>
        /// <param name="channelWriteCtx">Channel write function context</param>
        /// <returns>Returns `0` on success, or an error code `< 0` on failure</returns>
        [DllImport(Lib, EntryPoint = "mediasoup_worker_run", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern int MediasoupWorkerRun(int argc,
            [In] string[] argv,
            string version,
            int consumerChannelFd,
            int producerChannelFd,
            int _payloadConsumeChannelFd,
            int _payloadProduceChannelFd,
            ChannelReadFn channelReadFn,
            ChannelReadCtx channelReadCtx,
            ChannelWriteFn channelWriteFn,
            ChannelWriteCtx channelWriteCtx
            );
    }
}
