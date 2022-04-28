using System;
using System.Threading.Tasks;

namespace Tubumu.Mediasoup
{
    public interface IChannel
    {
        event Action<string, string, string?>? MessageEvent;

        void Close();
        Task<string?> RequestAsync(MethodId methodId, object? @internal = null, object? data = null);
    }
}