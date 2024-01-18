using System;
using System.Threading.Tasks;
using FBS.Message;
using FBS.Notification;
using FBS.Request;
using FBS.Response;
using Google.FlatBuffers;

namespace Tubumu.Mediasoup
{
    public interface IChannel
    {
        FlatBufferBuilder BufferBuilder { get; }

        event Action<string, Event, Notification>? OnNotification;

        Task CloseAsync();

        Task<Response?> RequestAsync(Method method, FBS.Request.Body? bodyType, int? bodyOffset, string? handlerId);

        Task NotifyAsync(Event @event, FBS.Notification.Body? bodyType, int? bodyOffset, string? handlerId);

        void ProcessMessage(Message message);
    }
}
