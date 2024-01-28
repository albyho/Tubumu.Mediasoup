using System;

namespace Tubumu.Libuv.Utilities
{
    public static partial class UtilitiesExtensions
    {
        public static void Pump<T>(this IUVStream<T> readStream, IUVStream<T> writeStream)
        {
            Pump(readStream, writeStream, null);
        }

        public static void Pump<T>(this IUVStream<T> readStream, IUVStream<T> writeStream, Action<Exception?>? callback)
        {
            bool pending = false;
            bool done = false;

            Action<Exception?>? call = null;
            void complete()
            {
                call?.Invoke(null);
            }

            call = (ex) =>
            {
                if(done)
                {
                    return;
                }

                readStream.Error -= call;
                readStream.Complete -= complete;

                done = true;
                callback?.Invoke(ex);
            };

            readStream.Data += (data) =>
            {
                writeStream.Write(data, null);
                if(writeStream.WriteQueueSize > 0)
                {
                    pending = true;
                    readStream.Pause();
                }
            };

            writeStream.Drain += () =>
            {
                if(pending)
                {
                    pending = false;
                    readStream.Resume();
                }
            };

            readStream.Error += call;
            readStream.Complete += complete;

            readStream.Resume();
        }
    }
}
