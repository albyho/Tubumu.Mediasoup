using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    public enum TTYMode : int
    {
        Normal = 0,
        Raw,
    }

    public class TTY : UVStream
    {
        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_tty_init(IntPtr loop, IntPtr tty, int fd, int readable);

        public TTY(int fd)
            : this(Loop.Constructor, fd) { }

        public TTY(Loop loop, int fd)
            : this(loop, fd, true) { }

        public TTY(int fd, bool readable)
            : this(Loop.Constructor, fd, readable) { }

        public TTY(Loop loop, int fd, bool readable)
            : base(loop, HandleType.UV_TTY, uv_tty_init, fd, readable ? 1 : 0) { }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_tty_set_mode(IntPtr tty, int mode);

        public TTYMode Mode
        {
            set => Invoke(uv_tty_set_mode, (int)value);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern void uv_tty_reset_mode();

        public static void ResetMode()
        {
            uv_tty_reset_mode();
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_tty_get_winsize(IntPtr tty, out int width, out int height);

        public bool GetWindowSize(out int width, out int height)
        {
            CheckDisposed();

            int r = uv_tty_get_winsize(NativeHandle, out width, out height);
            return r == 0;
        }
    }
}
