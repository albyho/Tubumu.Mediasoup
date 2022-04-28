using System;
using System.Runtime.InteropServices;
using ChannelReadCtx = System.IntPtr;
using ChannelWriteCtx = System.IntPtr;
using PayloadChannelReadCtx = System.IntPtr;
using PayloadChannelWriteCtx = System.IntPtr;
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
        /// <param name="message"></param>
        /// <param name="messageLen"></param>
        /// <param name="ctx"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ChannelWriteFn([MarshalAs(UnmanagedType.LPStr)] string message, uint messageLen, ChannelWriteCtx ctx);

        #endregion

        #region P/Invoke PayloadChannel

        /// <summary>
        /// Free memory
        /// </summary>
        /// <param name="message">uint8_t*</param>
        /// <param name="messageLen">uint32_t</param>
        /// <param name="messageCtx">size_t</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void PayloadChannelReadFreeFn(IntPtr message, uint messageLen, size_t messageCtx);

        /// <summary>
        /// Write to PayloadChannel
        /// </summary>
        /// <param name="message">uint8_t**</param>
        /// <param name="messageLen">uint32_t*</param>
        /// <param name="messageCtx">size_t*</param>
        /// <param name="payload">uint8_t**</param>
        /// <param name="payloadLen">uint32_t*</param>
        /// <param name="payloadCapacity">size_t*</param>
        /// <param name="handle">const void*</param>
        /// <param name="ctx">void*</param>
        /// <returns>Returns `PayloadChannelReadFree` on successful read that must be used to free `message`</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate PayloadChannelReadFreeFn? PayloadChannelReadFn(IntPtr message, IntPtr messageLen, IntPtr messageCtx,
            IntPtr payload, IntPtr payloadLen, IntPtr payloadCapacity,
            IntPtr handle, ChannelReadCtx ctx);

        /// <summary>
        /// Read from PayloadChannel
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageLen"></param>
        /// <param name="payload"></param>
        /// <param name="payloadLen"></param>
        /// <param name="ctx"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void PayloadChannelWriteFn([MarshalAs(UnmanagedType.LPStr)] string message, uint messageLen,
            IntPtr payload, uint payloadLen,
            PayloadChannelWriteCtx ctx);

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
        /// <param name="payloadConsumeChannelFd"></param>
        /// <param name="payloadProduceChannelFd"></param>
        /// <param name="channelReadFn"></param>
        /// <param name="channelReadCtx"></param>
        /// <param name="channelWriteFn"></param>
        /// <param name="channelWriteCtx"></param>
        /// <param name="payloadChannelReadFn"></param>
        /// <param name="payloadChannelReadCtx"></param>
        /// <param name="payloadChannelWriteFn"></param>
        /// <param name="payloadChannelWriteCtx"></param>
        /// <returns>Returns `0` on success, or an error code `< 0` on failure</returns>
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mediasoup_worker_run")]
        internal static extern int MediasoupWorkerRun(int argc,
            [In] string[] argv,
            string version,
            int consumerChannelFd,
            int producerChannelFd,
            int payloadConsumeChannelFd,
            int payloadProduceChannelFd,
            ChannelReadFn channelReadFn,
            ChannelReadCtx channelReadCtx,
            ChannelWriteFn channelWriteFn,
            ChannelWriteCtx channelWriteCtx,
            PayloadChannelReadFn payloadChannelReadFn,
            PayloadChannelReadCtx payloadChannelReadCtx,
            PayloadChannelWriteFn payloadChannelWriteFn,
            PayloadChannelWriteCtx payloadChannelWriteCtx
            );
    }
}
