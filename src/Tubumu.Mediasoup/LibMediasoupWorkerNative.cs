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
        /// <param name="argc"></param>
        /// <param name=""></param>
        /// <returns>Returns `0` on success, or an error code `< 0` on failure</returns>
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_async_send(IntPtr handle);

        /// <summary>
        /// Run worker
        /// </summary>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        /// <param name="version"></param>
        /// <param name="consumerChannelFd"></param>
        /// <param name="producerChannelFd"></param>
        /// <param name="_payloadConsumeChannelFd">已不使用</param>
        /// <param name="_payloadProduceChannelFd">已不使用</param>
        /// <param name="channelReadFn"></param>
        /// <param name="channelReadCtx"></param>
        /// <param name="channelWriteFn"></param>
        /// <param name="channelWriteCtx"></param>
        /// <returns>Returns `0` on success, or an error code `< 0` on failure</returns>
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mediasoup_worker_run")]
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
