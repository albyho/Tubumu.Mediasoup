using System;
using System.IO;
using System.Net;

namespace Tubumu.Libuv
{
    internal class Ensure
    {
        internal static Exception? Map(int systemErrorCode, string? name = null)
        {
            // no error, just return null
            if (!(systemErrorCode < 0))
            {
                return null;
            }

            // map some error codes
            var errorCode = UVException.Map(systemErrorCode);
            switch (errorCode)
            {
                case UVErrorCode.EINVAL:
                    return new ArgumentException(UVException.StringError(systemErrorCode));

                case UVErrorCode.ENOENT:
                    var path =
                        name == null ? Directory.GetCurrentDirectory() : Path.Combine(Directory.GetCurrentDirectory(), name);
                    return new FileNotFoundException("Could not find file", path);

                case UVErrorCode.ENOTSUP:
                    return new NotSupportedException("UVErrorCode.ENOTSUP");

                default:
                    break;
            }

            // everything else is a UVException
            return new UVException(systemErrorCode);
        }

        public static void Success(int errorCode)
        {
            if (errorCode < 0)
            {
                var e = Map(errorCode);
                if (e != null)
                {
                    throw e;
                }
            }
        }

        internal static void Success(int errorCode, Action<Exception?>? callback, string? name = null)
        {
            callback?.Invoke(Map(errorCode));
        }

        internal static void Success<T>(Exception? ex, Action<Exception?, T>? callback, T arg)
        {
            callback?.Invoke(ex, arg);
        }

        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void AddressFamily(IPAddress ipAddress)
        {
            if (
                ipAddress.AddressFamily
                is not System.Net.Sockets.AddressFamily.InterNetwork
                    and not System.Net.Sockets.AddressFamily.InterNetworkV6
            )
            {
                throw new ArgumentException("ipAddress has to be of AddressFamily InterNetwork or InterNetworkV6");
            }
        }
    }
}
