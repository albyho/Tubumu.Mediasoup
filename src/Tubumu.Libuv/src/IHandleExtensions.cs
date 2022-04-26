namespace Tubumu.Libuv
{
    public static class IHandleExtensions
    {
        public static void Close(this IHandle handle)
        {
            handle.Close(null);
        }
    }
}
