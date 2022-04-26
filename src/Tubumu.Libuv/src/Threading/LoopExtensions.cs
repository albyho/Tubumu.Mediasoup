using System;
using System.Threading;

namespace Tubumu.Libuv.Threading
{
    public static class LoopExtensions
    {
        public static void QueueUserWorkItem(this Loop loop, Action work, Action? after = null)
        {
            loop.Ref();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                work?.Invoke();
                loop.Sync(() =>
                {
                    loop.Unref();
                    after?.Invoke();
                });
            });
        }

        public static void QueueUserWorkItem<T>(this Loop loop, T state, Action<T> work, Action? after = null)
        {
            loop.Ref();
            ThreadPool.QueueUserWorkItem(o =>
            {
                work?.Invoke((T)o!);
                loop.Sync(() =>
                {
                    loop.Unref();
                    after?.Invoke();
                });
            }, state);
        }
    }
}
