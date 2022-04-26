using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    public enum PollEvent : int
    {
        Read = 1,
        Write = 2,
    }

    public class Poll : Handle
    {
        private delegate void poll_callback(IntPtr handle, int status, int events);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_poll_init(IntPtr loop, IntPtr handle, int fd);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_poll_start(IntPtr handle, int events, poll_callback callback);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_poll_stop(IntPtr handle);

        public Poll(int fd)
            : this(Loop.Constructor, fd)
        {
        }

        public Poll(Loop loop, int fd)
            : base(loop, HandleType.UV_POLL, uv_poll_init, fd)
        {
            poll_cb += pollcallback;
        }

        public void Start(PollEvent events)
        {
            Invoke(uv_poll_start, (int)events, poll_cb);
        }

        public void Stop()
        {
            Invoke(uv_poll_stop);
        }

        private event poll_callback poll_cb;

        private void pollcallback(IntPtr handle, int status, int events)
        {
            OnEvent((PollEvent)events);
        }

        public event Action<PollEvent>? Event;

        private void OnEvent(PollEvent events)
        {
            Event?.Invoke(events);
        }
    }
}
