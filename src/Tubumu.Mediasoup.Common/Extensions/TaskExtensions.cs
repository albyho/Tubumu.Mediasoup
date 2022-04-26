using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup.Extensions
{
    public static class TaskExtensions
    {
        public static void ContinueWithOnFaultedLog(this Task task, ILogger logger)
        {
            task.ContinueWith(val =>
            {
                // we need to access val.Exception property otherwise unobserved exception will be thrown
                // ReSharper disable once PossibleNullReferenceException
                foreach (var ex in val.Exception!.Flatten().InnerExceptions)
                {
                    logger.LogError(ex, $"Task exception");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void ContinueWithOnFaultedHandleLog(this Task task, ILogger logger)
        {
            task.ContinueWith(val =>
            {
                val.Exception!.Handle(ex =>
                {
                    logger.LogError(ex, $"Task exception");
                    return true;
                });
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
