using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct uv_req_t
    {
        public IntPtr data;
        public RequestType type;
    }
}
