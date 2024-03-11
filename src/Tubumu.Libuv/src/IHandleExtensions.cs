namespace Tubumu.Libuv
{
    internal static class IHandleExtensions
    {
        public static void Close(this IHandle handle)
        {
            handle.Close(null);
        }
    }
}
