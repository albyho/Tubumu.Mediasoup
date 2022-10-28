using System;
using System.Threading.Tasks;

namespace Tubumu.Mediasoup
{
    public interface IChannel
    {
        event Action<string, string, string?>? MessageEvent;

        Task CloseAsync();
        Task<string?> RequestAsync(MethodId methodId, string? handlerId = null, object? data = null);
        void ProcessMessage(string message);
    }
}