using System;

namespace Tubumu.Libuv
{
    public interface IMessageReceiver<TMessage>
    {
        event Action<TMessage> Message;
    }
}
