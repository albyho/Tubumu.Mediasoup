﻿using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    public abstract class BasePipeListener<TListener, TStream>
        : Listener<TStream>,
            ILocalAddress<string>,
            IBindable<TListener, string>
        where TStream : class, IUVStream<ArraySegment<byte>>
        where TListener : IListener<TStream>
    {
        internal BasePipeListener(Loop loop, bool ipc)
            : base(loop, HandleType.UV_NAMED_PIPE, NativeMethods.uv_pipe_init, ipc ? 1 : 0) { }

        public void Bind(string name)
        {
            Ensure.ArgumentNotNull(name, nameof(name));
            Invoke(NativeMethods.uv_pipe_bind, name);
        }

        public string LocalAddress =>
            UV.ToString(
                4096,
                (IntPtr buffer, ref IntPtr length) => NativeMethods.uv_pipe_getsockname(NativeHandle, buffer, ref length)
            );
    }

    public class PipeListener : BasePipeListener<PipeListener, Pipe>
    {
        public PipeListener()
            : this(Loop.Constructor) { }

        public PipeListener(Loop loop)
            : base(loop, false) { }

        protected override UVStream Create()
        {
            return new Pipe(Loop);
        }
    }

    public class IPCPipeListener : BasePipeListener<IPCPipeListener, IPCPipe>
    {
        public IPCPipeListener()
            : this(Loop.Constructor) { }

        public IPCPipeListener(Loop loop)
            : base(loop, true) { }

        protected override UVStream Create()
        {
            return new IPCPipe(Loop);
        }
    }

    public class Pipe : UVStream, IConnectable<Pipe, string>, IRemoteAddress<string>
    {
        private readonly unsafe uv_pipe_t* pipe_t;

        public Pipe()
            : this(Loop.Constructor) { }

        public Pipe(Loop loop)
            : this(loop, false) { }

        internal unsafe Pipe(Loop loop, bool interProcessCommunication)
            : base(loop, HandleType.UV_NAMED_PIPE, NativeMethods.uv_pipe_init, interProcessCommunication ? 1 : 0)
        {
            pipe_t = (uv_pipe_t*)(NativeHandle.ToInt64() + Size(HandleType.UV_STREAM));
        }

        public unsafe bool InterProcessCommunication => pipe_t->rpc >= 1;

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void uv_pipe_connect(IntPtr req, IntPtr handle, string name, callback connect_cb);

        public void Connect(string name, Action<Exception?>? callback)
        {
            CheckDisposed();

            Ensure.ArgumentNotNull(name, "name");

            var cpr = new ConnectRequest();
            Pipe pipe = this;

            cpr.Callback = (status, cpr2) => Ensure.Success(status, callback, name);

            uv_pipe_connect(cpr.Handle, pipe.NativeHandle, name, ConnectRequest.CallbackDelegate);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_pipe_getpeername(IntPtr handle, IntPtr buf, ref IntPtr len);

        public string RemoteAddress
        {
            get
            {
                CheckDisposed();

                return UV.ToString(
                    4096,
                    (IntPtr buffer, ref IntPtr length) => uv_pipe_getpeername(NativeHandle, buffer, ref length)
                );
            }
        }
    }

    public class IPCPipe : Pipe
    {
        public IPCPipe()
            : this(Loop.Constructor) { }

        public IPCPipe(Loop loop)
            : base(loop, true) { }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_write2(
            IntPtr req,
            IntPtr handle,
            uv_buf_t[] bufs,
            int bufcnt,
            IntPtr sendHandle,
            callback callback
        );

        public void Write(Handle handle, ArraySegment<byte> segment, Action<Exception?>? callback)
        {
            CheckDisposed();

            GCHandle datagchandle = GCHandle.Alloc(segment.Array, GCHandleType.Pinned);
            CallbackPermaRequest cpr = new(RequestType.UV_WRITE)
            {
                Callback = (status, cpr2) =>
                {
                    datagchandle.Free();
                    Ensure.Success(status, callback);
                },
            };

            var ptr = (IntPtr)(datagchandle.AddrOfPinnedObject().ToInt64() + segment.Offset);

            var buf = new uv_buf_t[] { new uv_buf_t(ptr, segment.Count) };
            int r = uv_write2(cpr.Handle, NativeHandle, buf, 1, handle.NativeHandle, CallbackPermaRequest.CallbackDelegate);
            Ensure.Success(r);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_pipe_pending_count(IntPtr handle);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern HandleType uv_pipe_pending_type(IntPtr pipe);

        protected override void OnData(ArraySegment<byte> data)
        {
            var count = uv_pipe_pending_count(NativeHandle);
            if (count-- > 0)
            {
                var type = uv_pipe_pending_type(NativeHandle);
                Handle? handle = null;
                switch (type)
                {
                    case HandleType.UV_UDP:
                        handle = new Udp(Loop);
                        break;

                    case HandleType.UV_TCP:
                        handle = new Tcp(Loop);
                        break;

                    case HandleType.UV_NAMED_PIPE:
                        handle = new Pipe(Loop);
                        break;
                }
                if (handle != null)
                {
                    Invoke(NativeMethods.uv_accept, handle.NativeHandle);
                    OnHandleData(handle, data);
                }
            }
            base.OnData(data);
        }

        protected virtual void OnHandleData(Handle handle, ArraySegment<byte> data)
        {
            HandleData?.Invoke(handle, data);
        }

        public event Action<Handle, ArraySegment<byte>>? HandleData;
    }
}
