using System;

namespace Tubumu.Libuv
{
    internal unsafe class WorkRequest : PermaRequest
    {
        public static readonly int Size = UV.Sizeof(Tubumu.Libuv.RequestType.UV_WORK);

        public WorkRequest()
            : base(Size)
        {
        }

        private readonly Action? before;
        private readonly Action? after;

        public WorkRequest(Action before, Action after)
            : this()
        {
            this.before = before;
            this.after = after;
        }

        public static void BeforeCallback(IntPtr req)
        {
            var workreq = PermaRequest.GetObject<WorkRequest>(req);
            workreq?.before?.Invoke();
        }

        public static void AfterCallback(IntPtr req)
        {
            var workreq = PermaRequest.GetObject<WorkRequest>(req);
            workreq?.after?.Invoke();
            workreq?.Dispose();
        }
    }
}
