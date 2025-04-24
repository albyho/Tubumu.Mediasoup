using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    public class Prepare : StartableCallbackHandle
    {
        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_prepare_init(IntPtr loop, IntPtr prepare);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_prepare_start(IntPtr prepare, uv_handle_cb callback);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_prepare_stop(IntPtr prepare);

        public Prepare()
            : this(Loop.Constructor) { }

        public Prepare(Loop loop)
            : base(loop, HandleType.UV_PREPARE, uv_prepare_init) { }

        public override void Start()
        {
            Invoke(uv_prepare_start);
        }

        public override void Stop()
        {
            Invoke(uv_prepare_stop);
        }
    }
}
