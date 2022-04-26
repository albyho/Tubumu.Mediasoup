using System;
using System.Text;
using System.Threading.Tasks;

namespace Tubumu.Libuv.Threading.Tasks
{
    public static class StreamExtensions
    {
        public static Task<TData?> ReadStructAsync<TData>(this IUVStream<TData> stream) where TData : struct
        {
            var tcs = new TaskCompletionSource<TData?>();
#if TASK_STATUS
			HelperFunctions.SetStatus(tcs.Task, TaskStatus.Running);
#endif

            Action<Exception?, TData?>? finish = null;

            Action<Exception?> error = (e) => finish?.Invoke(e, null);
            Action<TData> data = (val) => finish?.Invoke(null, val);
            Action end = () => finish?.Invoke(null, null);

            finish = HelperFunctions.Finish(tcs, () =>
            {
                stream.Pause();
                stream.Error -= error;
                stream.Complete -= end;
                stream.Data -= data;
            });

            try
            {
                stream.Error += error;
                stream.Complete += end;
                stream.Data += data;
                stream.Resume();
            }
            catch (Exception e)
            {
                finish(e, null);
            }

            return tcs.Task;
        }

        #region WriteAsync

        public static Task WriteAsync(this IUVStream<ArraySegment<byte>> stream, ArraySegment<byte> data)
        {
            return HelperFunctions.Wrap(data, stream.Write);
        }

        public static Task WriteAsync(this IUVStream<ArraySegment<byte>> stream, byte[] data, int index, int count)
        {
            return HelperFunctions.Wrap(data, index, count, stream.Write);
        }

        //public static Task WriteAsync(this IUVStream<ArraySegment<byte>> stream, byte[] data)
        //{
        //    return HelperFunctions.Wrap(data, stream.Write);
        //}

        public static Task<int> WriteAsync(this IUVStream<ArraySegment<byte>> stream, Encoding encoding, string text)
        {
            return HelperFunctions.Wrap<Encoding, string, int>(encoding, text, stream.Write);
        }

        public static Task<int> WriteAsync(this IUVStream<ArraySegment<byte>> stream, string text)
        {
            return HelperFunctions.Wrap<string, int>(text, stream.Write);
        }

        #endregion WriteAsync

        public static Task ShutdownAsync(this IUVStream<ArraySegment<byte>> stream)
        {
            return HelperFunctions.Wrap(stream.Shutdown);
        }
    }
}
