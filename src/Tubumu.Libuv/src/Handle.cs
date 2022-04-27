using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    public abstract unsafe class Handle : IHandle, IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void callback(IntPtr req, int status);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void alloc_callback(IntPtr data, int size, out uv_buf_t buf);

        public Loop Loop { get; protected set; }

        public IntPtr NativeHandle { get; protected set; }

        internal GCHandle GCHandle { get; set; }

        private uv_handle_t* handle
        {
            get
            {
                CheckDisposed();
                return (uv_handle_t*)NativeHandle;
            }
        }

        internal IntPtr DataPointer
        {
            get => handle->data;
            set => handle->data = value;
        }

        internal static T FromIntPtr<T>(IntPtr ptr)
        {
            return (T)GCHandle.FromIntPtr(((uv_handle_t*)ptr)->data).Target!;
        }

        public HandleType HandleType => handle->type;

        internal Handle(Loop loop, IntPtr handle)
        {
            Ensure.ArgumentNotNull(loop, "loop");

            NativeHandle = handle;
            GCHandle = GCHandle.Alloc(this);
            Loop = loop;

            Loop.handles[NativeHandle] = this;

            DataPointer = GCHandle.ToIntPtr(GCHandle);
        }

        internal Handle(Loop loop, int size)
            : this(loop, UV.Alloc(size))
        {
        }

        internal Handle(Loop loop, HandleType handleType)
            : this(loop, Size(handleType))
        {
        }

        internal Handle(Loop loop, HandleType handleType, Func<IntPtr, IntPtr, int> constructor)
            : this(loop, handleType)
        {
            Construct(constructor);
        }

        internal Handle(Loop loop, HandleType handleType, Func<IntPtr, IntPtr, int, int> constructor, int arg1)
            : this(loop, handleType)
        {
            Construct(constructor, arg1);
        }

        internal void Construct(Func<IntPtr, IntPtr, int> constructor)
        {
            int r = constructor(Loop.NativeHandle, NativeHandle);
            Ensure.Success(r);
        }

        internal void Construct(Func<IntPtr, IntPtr, int, int> constructor, int arg1)
        {
            int r = constructor(Loop.NativeHandle, NativeHandle, arg1);
            Ensure.Success(r);
        }

        internal void Construct(Func<IntPtr, IntPtr, int, int, int> constructor, int arg1, int arg2)
        {
            int r = constructor(Loop.NativeHandle, NativeHandle, arg1, arg2);
            Ensure.Success(r);
        }

        public event Action? Closed;

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern void uv_close(IntPtr handle, close_callback cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void close_callback(IntPtr handle);

        private Action? closeCallback;

        private static readonly close_callback close_cb = CloseCallback;

        private static void CloseCallback(IntPtr handlePointer)
        {
            var handle = FromIntPtr<Handle>(handlePointer);
            handle.Cleanup(handlePointer, handle.closeCallback);
        }

        public void Cleanup(IntPtr nativeHandle, Action? callback)
        {
            // Remove handle
            if (NativeHandle != IntPtr.Zero)
            {
                Loop.handles.Remove(nativeHandle);

                UV.Free(nativeHandle);

                NativeHandle = IntPtr.Zero;

                Closed?.Invoke();

                callback?.Invoke();

                if (GCHandle.IsAllocated)
                {
                    GCHandle.Free();
                }
            }
        }

        public void Close(Action? callback)
        {
            if (!IsClosing && !IsClosed)
            {
                closeCallback = callback;
                uv_close(NativeHandle, close_cb);
            }
        }

        public void Close()
        {
            Close(null);
        }

        public bool IsClosed => NativeHandle == IntPtr.Zero;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Close();
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_is_active(IntPtr handle);

        public bool IsActive => !IsClosed && uv_is_active(NativeHandle) != 0;

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_is_closing(IntPtr handle);

        public bool IsClosing => !IsClosed && uv_is_closing(NativeHandle) != 0;

        /// <summary>
        /// Is the underlying still alive? Returns true if handle
        /// is not closing or closed.
        /// </summary>
        /// <value><c>true</c> if this instance is not closing or closed; otherwise, <c>false</c>.</value>
        public bool IsAlive => !IsClosed && !IsClosing;

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern void uv_ref(IntPtr handle);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern void uv_unref(IntPtr handle);

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uv_has_ref(IntPtr handle);

        public void Ref()
        {
            if (IsClosed)
            {
                return;
            }
            uv_ref(NativeHandle);
        }

        public void Unref()
        {
            if (IsClosed)
            {
                return;
            }
            uv_unref(NativeHandle);
        }

        public bool HasRef => !IsClosed && uv_has_ref(NativeHandle) != 0;

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int uv_handle_size(HandleType type);

        public static int Size(HandleType type)
        {
            return uv_handle_size(type);
        }

        [DllImport(NativeMethods.Libuv, CallingConvention = CallingConvention.Cdecl)]
        private static extern HandleType uv_guess_handle(int fd);

        public static HandleType Guess(int fd)
        {
            return uv_guess_handle(fd);
        }

        protected void CheckDisposed()
        {
            if (NativeHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException(GetType().ToString(), "handle was closed");
            }
        }

        protected void Invoke(Func<IntPtr, int> function)
        {
            CheckDisposed();

            int r = function(NativeHandle);
            Ensure.Success(r);
        }

        protected void Invoke<T1>(Func<IntPtr, T1, int> function, T1 arg1)
        {
            CheckDisposed();

            int r = function(NativeHandle, arg1);
            Ensure.Success(r);
        }

        protected void Invoke<T1, T2>(Func<IntPtr, T1, T2, int> function, T1 arg1, T2 arg2)
        {
            CheckDisposed();

            int r = function(NativeHandle, arg1, arg2);
            Ensure.Success(r);
        }

        protected void Invoke<T1, T2, T3>(Func<IntPtr, T1, T2, T3, int> function, T1 arg1, T2 arg2, T3 arg3)
        {
            CheckDisposed();

            int r = function(NativeHandle, arg1, arg2, arg3);
            Ensure.Success(r);
        }
    }
}
