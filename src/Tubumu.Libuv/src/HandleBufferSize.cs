using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    public partial class HandleBase : ISendBufferSize, IReceiveBufferSize
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int buffer_size_function(IntPtr handle, out int value);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_send_buffer_size(IntPtr handle, out int value);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_recv_buffer_size(IntPtr handle, out int value);

        private int Invoke(buffer_size_function function, int value)
        {
            CheckDisposed();

            var r = function(NativeHandle, out value);
            Ensure.Success(r);
            return r;
        }

        private int Apply(buffer_size_function buffer_size, int value)
        {
            return Invoke(buffer_size, value);
        }

        public int SendBufferSize
        {
            get => Apply(uv_send_buffer_size, 0);
            set => Apply(uv_send_buffer_size, @value);
        }

        public int ReceiveBufferSize
        {
            get => Apply(uv_recv_buffer_size, 0);
            set => Apply(uv_recv_buffer_size, @value);
        }
    }
}
