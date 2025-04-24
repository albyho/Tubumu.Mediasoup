using System;
using System.Threading;
using System.Threading.Tasks;
using Tubumu.Libuv.Threading.Tasks;

namespace Tubumu.Libuv
{
    public partial class Loop
    {
        private TaskFactory? taskfactory = null;

        public TaskFactory TaskFactory
        {
            get
            {
                taskfactory ??= new TaskFactory(Scheduler);
                return taskfactory;
            }
        }

        public static TaskScheduler Scheduler => LoopTaskScheduler.Instance;

        public bool Run(Func<Task> asyncMethod)
        {
            var previousContext = SynchronizationContext.Current;
            try
            {
                var loop = this;
                SynchronizationContext.SetSynchronizationContext(new LoopSynchronizationContext(loop));
                var task = asyncMethod();
#if TASK_STATUS
                HelperFunctions.SetStatus(task, TaskStatus.Running);
#endif
                task.ContinueWith(
                    (t) =>
                    {
                        loop.Unref();
                        loop.Sync(() => { });
                    }
                );
                loop.Ref();

                var returnValue = loop.Run();

                return task.Exception != null ? throw task.Exception : returnValue;
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        public static Loop? Current
        {
            get
            {
                if (currentLoop != null)
                {
                    return currentLoop;
                }

                var current = SynchronizationContext.Current;
                if (current is LoopSynchronizationContext context)
                {
                    return context.Loop;
                }

                // TODO: Think about returning exception
                return null;
            }
        }

        /// <summary>
        /// Returns Default Loop value when creating Tubumu.Libuv objects.
        /// </summary>
        /// <value>A loop.</value>
        internal static Loop Constructor => Current ?? Default;
    }
}
