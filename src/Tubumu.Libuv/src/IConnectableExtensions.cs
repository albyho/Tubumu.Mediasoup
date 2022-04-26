using System;
using System.Net;

namespace Tubumu.Libuv
{
    public static class IConnectableExtensions
    {
        #region IP Extensions

        public static void Connect<TType>(this IConnectable<TType, IPEndPoint> client, IPAddress ipAddress, int port, Action<Exception?>? callback)
        {
            Ensure.ArgumentNotNull(ipAddress, "ipAddress");

            client.Connect(new IPEndPoint(ipAddress, port), callback);
        }

        public static void Connect<TType>(this IConnectable<TType, IPEndPoint> client, string ipAddress, int port, Action<Exception?>? callback)
        {
            client.Connect(IPAddress.Parse(ipAddress), port, callback);
        }

        #endregion IP Extensions

        public static void Connect<TType, TEndPoint>(this IConnectable<TType, TEndPoint> client, ILocalAddress<TEndPoint> remote, Action<Exception?>? callback)
        {
            client.Connect(remote.LocalAddress, callback);
        }
    }
}
