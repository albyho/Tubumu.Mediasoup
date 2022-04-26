using System;

namespace Tubumu.Libuv
{
    public interface IMessageSender<TMessage>
    {
        void Send(TMessage message, Action<Exception?>? callback);
    }
}
