using System.Threading.Tasks;

namespace Tubumu.Libuv.Threading.Tasks
{
    internal static class IHandleExtensions
    {
        public static Task CloseAsync(this IHandle handle)
        {
            return HelperFunctions.WrapSingle(handle.Close);
        }
    }
}
