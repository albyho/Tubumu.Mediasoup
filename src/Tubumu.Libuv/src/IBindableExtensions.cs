using System.Net;

namespace Tubumu.Libuv
{
    internal static class IBindableExtensions
    {
        public static void Bind<T>(this IBindable<T, IPEndPoint> bindable, IPAddress ipAddress, int port)
        {
            Ensure.ArgumentNotNull(ipAddress, "ipAddress");
            bindable.Bind(new IPEndPoint(ipAddress, port));
        }

        public static void Bind<T>(this IBindable<T, IPEndPoint> bindable, string ipAddress, int port)
        {
            Ensure.ArgumentNotNull(ipAddress, "ipAddress");
            bindable.Bind(IPAddress.Parse(ipAddress), port);
        }
    }
}
