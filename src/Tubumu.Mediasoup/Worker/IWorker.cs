using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tubumu.Mediasoup
{
    public interface IWorker : IEventEmitter, IDisposable
    {
        Dictionary<string, object>? AppData { get; }
        EventEmitter Observer { get; }

        Task CloseAsync();
        Task<Router?> CreateRouterAsync(RouterOptions routerOptions);
        Task<string?> DumpAsync();
        Task<string?> GetResourceUsageAsync();
        Task<string?> UpdateSettingsAsync(WorkerUpdateableSettings workerUpdateableSettings);
    }
}