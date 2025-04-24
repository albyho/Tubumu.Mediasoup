using System;
using System.Runtime.InteropServices;

namespace Tubumu.Libuv
{
    internal class CActionBase : IDisposable
    {
        private GCHandle GCHandle { get; set; }

        public CActionBase()
        {
            GCHandle = GCHandle.Alloc(this);
        }

        ~CActionBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (GCHandle.IsAllocated)
            {
                GCHandle.Free();
            }
        }
    }

    internal class CAction : CActionBase
    {
        public Action Callback { get; protected set; }

        private readonly Action cb;

        public CAction(Action callback)
            : base()
        {
            cb = callback;
            Callback = PrivateCallback;
        }

        private void PrivateCallback()
        {
            cb?.Invoke();

            Dispose();
        }
    }

    internal class CAction<T1> : CActionBase
    {
        public Action<T1> Callback { get; protected set; }

        private readonly Action<T1> cb;

        public CAction(Action<T1> callback)
            : base()
        {
            cb = callback;
            Callback = PrivateCallback;
        }

        private void PrivateCallback(T1 arg1)
        {
            cb?.Invoke(arg1);

            Dispose();
        }
    }

    internal class CAction<T1, T2> : CActionBase
    {
        public Action<T1, T2> Callback { get; protected set; }

        private readonly Action<T1, T2> cb;

        public CAction(Action<T1, T2> callback)
            : base()
        {
            cb = callback;
            Callback = PrivateCallback;
        }

        private void PrivateCallback(T1 arg1, T2 arg2)
        {
            cb?.Invoke(arg1, arg2);

            Dispose();
        }
    }

    internal class CAction<T1, T2, T3> : CActionBase
    {
        public Action<T1, T2, T3> Callback { get; protected set; }

        private readonly Action<T1, T2, T3> cb;

        public CAction(Action<T1, T2, T3> callback)
        {
            cb = callback;
            Callback = PrivateCallback;
        }

        private void PrivateCallback(T1 arg1, T2 arg2, T3 arg3)
        {
            cb?.Invoke(arg1, arg2, arg3);

            Dispose();
        }
    }
}
