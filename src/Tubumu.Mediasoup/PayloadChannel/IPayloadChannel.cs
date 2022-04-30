using System;
using System.Threading.Tasks;

namespace Tubumu.Mediasoup
{
    public interface IPayloadChannel
    {
        event Action<string, string, NotifyData, ArraySegment<byte>>? MessageEvent;

        Task CloseAsync();
        Task NotifyAsync(string @event, object @internal, NotifyData? data, byte[] payload);
        Task<string?> RequestAsync(MethodId methodId, object? @internal = null, object? data = null, byte[]? payload = null);
        void Process(string message, byte[] payload);
    }
}