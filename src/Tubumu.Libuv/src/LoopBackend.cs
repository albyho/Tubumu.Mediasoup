using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    public class LoopBackend
    {
        private readonly IntPtr nativeHandle;

        internal LoopBackend(Loop loop)
        {
            nativeHandle = loop.NativeHandle;
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_backend_fd(IntPtr loop);

        public int FileDescriptor => uv_backend_fd(nativeHandle);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_backend_timeout(IntPtr loop);

        public int Timeout => uv_backend_timeout(nativeHandle);
    }
}
