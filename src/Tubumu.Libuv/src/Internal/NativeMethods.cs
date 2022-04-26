using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    internal static class NativeMethods
    {
        public const string Libuv = "libuv";

        [DllImport(Libuv, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_listen(IntPtr stream, int backlog, Handle.callback callback);

        [DllImport(Libuv, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_accept(IntPtr server, IntPtr client);

        [DllImport(Libuv, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_tcp_getsockname(IntPtr handle, IntPtr addr, ref int length);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void uv_fs_cb(IntPtr IntPtr);

        [DllImport(Libuv, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_pipe_init(IntPtr loop, IntPtr handle, int ipc);

        [DllImport(Libuv, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_pipe_bind(IntPtr handle, string name);

        [DllImport(Libuv, CallingConvention = CallingConvention.Cdecl)]
        public static extern int uv_pipe_getsockname(IntPtr handle, IntPtr buf, ref IntPtr len);
    }
}
