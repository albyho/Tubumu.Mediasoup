namespace Tubumu.Libuv
{
    public interface ITryWrite<TData>
    {
        int TryWrite(TData data);
    }
}
