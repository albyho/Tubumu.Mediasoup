using System;
using System.Threading.Tasks;

namespace Tubumu.Libuv.Threading.Tasks
{
    internal static class ListenerExtensions
    {
        public static Task<TClient?> AcceptAsync<TClient>(this IListener<TClient> listener)
        {
            var tcs = new TaskCompletionSource<TClient?>();
#if TASK_STATUS
			HelperFunctions.SetStatus(tcs.Task, TaskStatus.Running);
#endif

            try
            {
                tcs.SetResult(listener.Accept());
            }
            catch(UVException ex)
            {
                if(ex.ErrorCode != UVErrorCode.EAGAIN)
                {
                    tcs.SetException(ex);
                }
            }

            if(tcs.Task.IsCompleted)
            {
                return tcs.Task;
            }

            Action<Exception?, TClient?>? finish = null;

            void connectioncb()
            {
                try
                {
                    finish?.Invoke(null, listener.Accept());
                }
                catch(Exception ex)
                {
                    finish?.Invoke(ex, default);
                }
            }

            finish = HelperFunctions.Finish(tcs, () =>
            {
                listener.Connection -= connectioncb;
            });

            listener.Connection += connectioncb;

            return tcs.Task;
        }
    }
}
