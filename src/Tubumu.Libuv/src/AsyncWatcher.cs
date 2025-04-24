using System;
using System.Collections.Generic;

namespace Tubumu.Libuv
{
    public class AsyncWatcher<T> : IHandle, IDisposable
    {
        private readonly Async async;
        private readonly Queue<T> queue = new();

        public AsyncWatcher()
            : this(Loop.Constructor) { }

        public AsyncWatcher(Loop loop)
        {
            async = new Async(loop);
            async.Callback += () =>
            {
                Queue<T> tmp;
                lock (queue)
                {
                    tmp = new Queue<T>();
                    while (queue.Count > 0)
                    {
                        tmp.Enqueue(queue.Dequeue());
                    }
                }
                while (tmp.Count > 0)
                {
                    OnCallback(tmp.Dequeue());
                }
            };
        }

        public void Ref()
        {
            async.Ref();
        }

        public void Unref()
        {
            async.Unref();
        }

        public bool HasRef => async.HasRef;

        public bool IsClosed => async.IsClosed;

        public void Close(Action? callback)
        {
            async.Close(callback);
        }

        public void Dispose()
        {
            this.Close();
        }

        public void Send(T item)
        {
            lock (queue)
            {
                queue.Enqueue(item);
            }
            async.Send();
        }

        public void Send(IEnumerable<T> data)
        {
            Ensure.ArgumentNotNull(data, "data");

            lock (queue)
            {
                foreach (var dataitem in data)
                {
                    queue.Enqueue(dataitem);
                }
            }
            async.Send();
        }

        public void OnCallback(T item)
        {
            Callback?.Invoke(item);
        }

        public event Action<T>? Callback;
    }
}
