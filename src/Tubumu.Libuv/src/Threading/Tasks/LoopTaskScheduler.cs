using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tubumu.Libuv.Threading.Tasks
{
    internal class LoopTaskScheduler : TaskScheduler
    {
        static LoopTaskScheduler()
        {
            Instance = new LoopTaskScheduler();
        }

        public static LoopTaskScheduler Instance { get; private set; }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return new Task[] { };
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        protected override void QueueTask(Task task) { }
    }
}
