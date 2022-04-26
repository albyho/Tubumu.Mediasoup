namespace Tubumu.Libuv
{
    public interface ITrySend<TMessage>
    {
        int TrySend(TMessage message);
    }
}
