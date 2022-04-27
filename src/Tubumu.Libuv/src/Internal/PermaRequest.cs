using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    internal unsafe class PermaRequest : IDisposable
    {
        public IntPtr Handle { get; protected set; }

        protected uv_req_t* request;

        public IntPtr Data
        {
            get => request->data;
            set => request->data = value;
        }

        public RequestType RequestType
        {
            get => request->type;
            set => request->type = value;
        }

        public GCHandle GCHandle
        {
            get => GCHandle.FromIntPtr(Data);
            set => Data = GCHandle.ToIntPtr(value);
        }

        public PermaRequest(int size)
            : this(UV.Alloc(size))
        {
        }

        public PermaRequest(IntPtr handle)
        {
            Handle = handle;
            request = (uv_req_t*)handle;

            GCHandle = GCHandle.Alloc(this, GCHandleType.Normal);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (Data != IntPtr.Zero)
            {
                if (GCHandle.IsAllocated)
                {
                    GCHandle.Free();
                }
                Data = IntPtr.Zero;
            }

            if (Handle != IntPtr.Zero)
            {
                UV.Free(Handle);
                Handle = IntPtr.Zero;
            }
        }

        public static unsafe T? GetObject<T>(IntPtr ptr) where T : class
        {
            uv_req_t* req = (uv_req_t*)ptr.ToPointer();
            return GCHandle.FromIntPtr(req->data).Target as T;
        }
    }
}
