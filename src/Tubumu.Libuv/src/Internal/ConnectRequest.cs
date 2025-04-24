using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct uv_connect_t
    {
        public RequestType type;
        public IntPtr data;

        /*
        #if !__MonoCS__
        NativeOverlapped overlapped;
        IntPtr queued_bytes;
        uv_err_t error;
        IntPtr next_req;
        #endif
        */
        public IntPtr cb;
        public IntPtr handle;
    }

    internal unsafe class ConnectRequest : CallbackPermaRequest
    {
        private readonly uv_connect_t* connect;

        public ConnectRequest()
            : base(RequestType.UV_CONNECT)
        {
            connect = (uv_connect_t*)Handle;
        }

        public IntPtr ConnectHandle => connect->handle;
    }
}
