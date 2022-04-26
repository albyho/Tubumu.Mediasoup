using System;
using System.Threading.Tasks;

namespace Tubumu.Libuv.Threading.Tasks
{
    public static class UVTimerExtensions
    {
        public static Task StartAsync(this UVTimer timer, ulong timeout)
        {
            var tcs = new TaskCompletionSource<object>();
#if TASK_STATUS
			HelperFunctions.SetStatus(tcs.Task, TaskStatus.Running);
#endif
            try
            {
                timer.Start(timeout, () =>
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    tcs.SetResult(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                });
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
            return tcs.Task;
        }

        public static Task StartAsync(this UVTimer timer, TimeSpan timeout)
        {
            return timer.StartAsync((ulong)timeout.TotalMilliseconds);
        }
    }
}
