using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace System.Net
{
    /// <summary>
    /// IPAddress 扩展方法
    /// </summary>
    public static class IPAddressExtensions
    {
        private static readonly Regex IPV4Regex = new(@"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$", RegexOptions.Compiled);

        /// <summary>
        /// IPAddress 转 Int32
        /// <para>NOTE: 可能产生负数</para>
        /// </summary>
        public static int ToInt32(this IPAddress ip)
        {
            var x = 3;
            var bytes = ip.GetAddressBytes();

            return bytes.Sum(f => f << (8 * x--));
        }

        /// <summary>
        /// IPAddress 转 Int64
        /// </summary>
        public static long ToInt64(this IPAddress ip)
        {
            var x = 3;
            var bytes = ip.GetAddressBytes();

            return bytes.Sum(f => (long)f << (8 * x--));
        }

        /// <summary>
        /// Int32 转 IPAddress
        /// </summary>
        public static IPAddress ToIPAddress(this int ip)
        {
            var bytes = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                bytes[3 - i] = (byte)((ip >> (8 * i)) & 255);
            }

            return new IPAddress(bytes);
        }

        /// <summary>
        /// Int64 转 IPAddress
        /// </summary>
        public static IPAddress ToIPAddress(this long ip)
        {
            var bytes = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                bytes[3 - i] = (byte)((ip >> (8 * i)) & 255);
            }

            return new IPAddress(bytes);
        }

        /// <summary>
        /// 是否 IPv4 格式
        /// </summary>
        public static bool IsIPv4(this string ip)
        {
            return !ip.IsNullOrWhiteSpace() && ip.Length is >= 7 and <= 15 && IPV4Regex.IsMatch(ip);
        }

        /// <summary>
        /// 获取本机 IP 地址
        /// </summary>
        public static IEnumerable<IPAddress> GetLocalIPAddresses(AddressFamily? addressFamily = null)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.Where(m => addressFamily == null || m.AddressFamily == addressFamily);
        }

        /// <summary>
        /// 获取一个本机的 IPv4 地址
        /// </summary>
        public static IPAddress? GetLocalIPv4IPAddress()
        {
            return GetLocalIPAddresses(AddressFamily.InterNetwork).FirstOrDefault(m => !IPAddress.IsLoopback(m));
        }
    }
}
