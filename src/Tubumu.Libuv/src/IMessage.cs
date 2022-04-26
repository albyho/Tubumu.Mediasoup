namespace Tubumu.Libuv
{
    public interface IMessage<TEndPoint, TMessage>
    {
        TEndPoint EndPoint { get; set; }
        TMessage Payload { get; set; }
    }
}
