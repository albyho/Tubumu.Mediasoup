namespace Tubumu.Libuv
{
    public interface IBindable<TType, TEndPoint>
    {
        void Bind(TEndPoint endPoint);
    }
}
