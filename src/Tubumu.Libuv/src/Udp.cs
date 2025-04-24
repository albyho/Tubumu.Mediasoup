using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    public class Udp
        : HandleBase,
            IMessageSender<UdpMessage>,
            IMessageReceiver<UdpReceiveMessage>,
            ITrySend<UdpMessage>,
            IBindable<Udp, IPEndPoint>
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void recv_start_callback(IntPtr handle, IntPtr nread, ref uv_buf_t buf, IntPtr sockaddr, ushort flags);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_udp_init(IntPtr loop, IntPtr handle);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_udp_bind(IntPtr handle, ref sockaddr_in sockaddr, uint flags);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_udp_bind(IntPtr handle, ref sockaddr_in6 sockaddr, uint flags);

        private ByteBufferAllocatorBase? allocator;

        public ByteBufferAllocatorBase ByteBufferAllocator
        {
            get => allocator ?? Loop.ByteBufferAllocator;
            set => allocator = value;
        }

        public Udp()
            : this(Loop.Constructor) { }

        public Udp(Loop loop)
            : base(loop, HandleType.UV_UDP, uv_udp_init) { }

        private void Bind(IPAddress ipAddress, int port, bool dualstack)
        {
            CheckDisposed();

            UV.Bind(this, uv_udp_bind, uv_udp_bind, ipAddress, port, dualstack);
        }

        public void Bind(int port)
        {
            Bind(IPAddress.IPv6Any, port, true);
        }

        public void Bind(IPEndPoint endPoint)
        {
            Ensure.ArgumentNotNull(endPoint, "endPoint");
            Bind(endPoint.Address, endPoint.Port, false);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_udp_send(
            IntPtr req,
            IntPtr handle,
            uv_buf_t[] bufs,
            int nbufs,
            ref sockaddr_in addr,
            callback callback
        );

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_udp_send(
            IntPtr req,
            IntPtr handle,
            uv_buf_t[] bufs,
            int nbufs,
            ref sockaddr_in6 addr,
            callback callback
        );

        public void Send(UdpMessage message, Action<Exception?>? callback)
        {
            CheckDisposed();

            Ensure.ArgumentNotNull(message.EndPoint, "message EndPoint");
            Ensure.AddressFamily(message.EndPoint.Address);

            var ipEndPoint = message.EndPoint;
            var data = message.Payload;

            var datagchandle = GCHandle.Alloc(data.Array, GCHandleType.Pinned);
            var cpr = new CallbackPermaRequest(RequestType.UV_UDP_SEND)
            {
                Callback = (status, cpr2) =>
                {
                    datagchandle.Free();
                    Ensure.Success(status, callback);
                },
            };

            var ptr = (IntPtr)(datagchandle.AddrOfPinnedObject().ToInt64() + data.Offset);

            int r;
            uv_buf_t[] buf = new uv_buf_t[] { new uv_buf_t(ptr, data.Count) };

            if (ipEndPoint.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                sockaddr_in address = UV.ToStruct(ipEndPoint.Address.ToString(), ipEndPoint.Port);
                r = uv_udp_send(cpr.Handle, NativeHandle, buf, 1, ref address, CallbackPermaRequest.CallbackDelegate);
            }
            else
            {
                sockaddr_in6 address = UV.ToStruct6(ipEndPoint.Address.ToString(), ipEndPoint.Port);
                r = uv_udp_send(cpr.Handle, NativeHandle, buf, 1, ref address, CallbackPermaRequest.CallbackDelegate);
            }
            Ensure.Success(r);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_udp_recv_start(IntPtr handle, alloc_callback alloc_callback, recv_start_callback callback);

        private static readonly recv_start_callback recv_start_cb = recv_callback;

        private static void recv_callback(IntPtr handlePointer, IntPtr nread, ref uv_buf_t buf, IntPtr sockaddr, ushort flags)
        {
            var handle = FromIntPtr<Udp>(handlePointer);
            handle.recv_callback(handlePointer, nread, sockaddr, flags);
        }

        private void recv_callback(IntPtr handle, IntPtr nread, IntPtr sockaddr, ushort flags)
        {
            var n = (int)nread;

            if (n == 0)
            {
                return;
            }

            if (Message != null)
            {
                var ep = UV.GetIPEndPoint(sockaddr, true);

                var msg = new UdpReceiveMessage(
                    ep,
                    ByteBufferAllocator.Retrieve(n),
                    (flags & (short)uv_udp_flags.UV_UDP_PARTIAL) > 0
                );

                Message(msg);
            }
        }

        public void Resume()
        {
            CheckDisposed();

            var r = uv_udp_recv_start(NativeHandle, ByteBufferAllocator.AllocCallback, recv_start_cb);
            Ensure.Success(r);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_udp_recv_stop(IntPtr handle);

        public void Pause()
        {
            Invoke(uv_udp_recv_stop);
        }

        public event Action<UdpReceiveMessage>? Message;

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_udp_set_ttl(IntPtr handle, int ttl);

        public byte TTL
        {
            set => Invoke(uv_udp_set_ttl, (int)value);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_udp_set_broadcast(IntPtr handle, int on);

        public bool Broadcast
        {
            set => Invoke(uv_udp_set_broadcast, value ? 1 : 0);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_udp_set_multicast_ttl(IntPtr handle, int ttl);

        public byte MulticastTTL
        {
            set => Invoke(uv_udp_set_multicast_ttl, (int)value);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_udp_set_multicast_loop(IntPtr handle, int on);

        public bool MulticastLoop
        {
            set => Invoke(uv_udp_set_multicast_loop, value ? 1 : 0);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_udp_getsockname(IntPtr handle, IntPtr addr, ref int length);

        public IPEndPoint LocalAddress
        {
            get
            {
                CheckDisposed();

                return UV.GetSockname(this, uv_udp_getsockname);
            }
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_udp_try_send(IntPtr handle, uv_buf_t[] bufs, int nbufs, ref sockaddr_in addr);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_udp_try_send(IntPtr handle, uv_buf_t[] bufs, int nbufs, ref sockaddr_in6 addr);

        public unsafe int TrySend(UdpMessage message)
        {
            Ensure.ArgumentNotNull(message, "message");

            var data = message.Payload;
            var ipEndPoint = message.EndPoint;

            fixed (byte* bytePtr = data.Array)
            {
                var ptr = (IntPtr)(bytePtr + message.Payload.Offset);
                int r;
                var buf = new uv_buf_t[] { new uv_buf_t(ptr, data.Count) };

                if (ipEndPoint.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    sockaddr_in address = UV.ToStruct(ipEndPoint.Address.ToString(), ipEndPoint.Port);
                    r = uv_udp_try_send(NativeHandle, buf, 1, ref address);
                }
                else
                {
                    sockaddr_in6 address = UV.ToStruct6(ipEndPoint.Address.ToString(), ipEndPoint.Port);
                    r = uv_udp_try_send(NativeHandle, buf, 1, ref address);
                }
                return r;
            }
        }
    }
}
