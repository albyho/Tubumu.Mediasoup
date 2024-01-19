using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FBS.Worker;

namespace Tubumu.Mediasoup
{
    public interface IWorker : IEventEmitter, IDisposable
    {
        Dictionary<string, object> AppData { get; }

        EventEmitter Observer { get; }

        Task CloseAsync();

        Task<Router> CreateRouterAsync(RouterOptions routerOptions);

        Task<DumpResponseT> DumpAsync();

        Task<ResourceUsageResponseT> GetResourceUsageAsync();

        Task UpdateSettingsAsync(WorkerUpdateableSettings workerUpdateableSettings);
    }
}
