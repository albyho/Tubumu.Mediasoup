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
        event Action<string, Event, Notification>? OnNotification;

        Task CloseAsync();

        Task<Response?> RequestAsync(FlatBufferBuilder bufferBuilder, Method method, FBS.Request.Body? bodyType = null, int? bodyOffset = null, string? handlerId = null);

        Task NotifyAsync(FlatBufferBuilder bufferBuilder, Event @event, FBS.Notification.Body? bodyType, int? bodyOffset, string? handlerId);

        void ProcessMessage(Message message);
    }
}
