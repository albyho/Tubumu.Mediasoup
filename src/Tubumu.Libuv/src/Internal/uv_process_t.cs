using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct uv_process_t
    {
        public IntPtr exit_cb;
        public int pid;
    }
}
