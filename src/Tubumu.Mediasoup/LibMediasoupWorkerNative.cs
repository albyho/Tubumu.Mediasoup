using System;
using System.Runtime.InteropServices;
using ChannelReadCtx = System.IntPtr;
using ChannelWriteCtx = System.IntPtr;
using PayloadChannelReadCtx = System.IntPtr;
using PayloadChannelWriteCtx = System.IntPtr;
using size_t = System.IntPtr;

namespace Tubumu.Mediasoup
{
	public static class LibMediasoupWorkerNative
	{
		public const string Lib = "mediasoup-worker";

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ChannelReadFreeFn(IntPtr message, uint messageLen, size_t messageCtx);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate ChannelReadFreeFn ChannelReadFn(IntPtr message, uint messageLen, size_t messageCtx,
			IntPtr handle, ChannelReadCtx ctx);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ChannelWriteFn(IntPtr message, uint messageLen, ChannelWriteCtx ctx);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PayloadChannelReadFreeFn(IntPtr message, uint messageLen, size_t messageCtx);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate PayloadChannelReadFreeFn PayloadChannelReadFn(IntPtr message, uint messageLen, size_t messageCtx,
			IntPtr payload, uint payloadLen, size_t payloadCapacity,
			IntPtr handle, ChannelReadCtx ctx);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void PayloadChannelWriteFn(IntPtr message, uint messageLen, IntPtr payload, uint payloadLen, PayloadChannelWriteCtx ctx);

		[DllImport(Lib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "mediasoup_worker_run")]
		public static extern int MediasoupWorkerRun(int argc,
			[MarshalAs(UnmanagedType.LPStr)] string[] argv,
			[MarshalAs(UnmanagedType.LPStr)] string version,
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
