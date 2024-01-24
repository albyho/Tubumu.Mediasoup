using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tubumu.Libuv.Threading.Tasks;

namespace Tubumu.Libuv.Extensions
{
    public static class Default
    {
        public static IPEndPoint IPEndPoint = new(IPAddress.Parse("127.0.0.1"), 7000);
    }

    public static class AsyncExtensions
    {
        public static Task<string?> ReadStringAsync(this IUVStream<ArraySegment<byte>> stream)
        {
            return ReadStringAsync(stream, Encoding.UTF8);
        }

        public static async Task<string?> ReadStringAsync(this IUVStream<ArraySegment<byte>> stream, Encoding encoding)
        {
            if(encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }
            var buffer = await stream.ReadStructAsync();
            return buffer.HasValue ? encoding.GetString(buffer.Value) : null;
        }
    }

    public static class EncodingExtensions
    {
        public static string GetString(this Encoding encoding, ArraySegment<byte> segment)
        {
            return encoding.GetString(segment.Array!, segment.Offset, segment.Count);
        }

        public static string? GetString(this Encoding encoding, ArraySegment<byte>? segment)
        {
            if(!segment.HasValue)
            {
                return null;
            }
            else
            {
                var value = segment.Value;
                return encoding.GetString(value.Array!, value.Offset, value.Count);
            }
        }
    }

    internal static class TcpClientExtensions
    {
        public static async Task ConnectAsync(this TcpClient client, IPEndPoint ipEndPoint)
        {
            await client.ConnectAsync(ipEndPoint.Address, ipEndPoint.Port);
        }
    }

    public static class HexExtensions
    {
        public static string ToHex(this byte[] bytes)
        {
            return string.Join(string.Empty, Array.ConvertAll(bytes, x => x.ToString("x2")));
        }

        public static string ToHex(this ArraySegment<byte> segment)
        {
            return string.Join(string.Empty, segment.Select((x) => x.ToString("x2")));
        }
    }

    public static class HashAlgorithmExtensions
    {
        public static void TransformBlock(this HashAlgorithm hashAlgorithm, byte[] input, byte[] outputBuffer, int outputOffset)
        {
            hashAlgorithm.TransformBlock(input, 0, input.Length, outputBuffer, outputOffset);
        }

        public static void TransformBlock(this HashAlgorithm hashAlgorithm, byte[] input, byte[] outputBuffer)
        {
            hashAlgorithm.TransformBlock(input, 0, input.Length, outputBuffer, 0);
        }

        public static void TransformBlock(this HashAlgorithm hashAlgorithm, ArraySegment<byte> input, byte[]? outputBuffer, int outputOffset)
        {
            hashAlgorithm.TransformBlock(input.Array!, input.Offset, input.Count, outputBuffer, outputOffset);
        }

        public static void TransformBlock(this HashAlgorithm hashAlgorithm, ArraySegment<byte> input)
        {
            hashAlgorithm.TransformBlock(input, null, 0);
        }

        public static void TransformFinalBlock(this HashAlgorithm hashAlgorithm, ArraySegment<byte> input)
        {
            hashAlgorithm.TransformFinalBlock(input.Array!, input.Offset, input.Count);
        }

        private static readonly byte[] emptyBuffer = Array.Empty<byte>();

        public static void TransformFinalBlock(this HashAlgorithm hashAlgorithm)
        {
            hashAlgorithm.TransformFinalBlock(emptyBuffer, 0, 0);
        }

        public static void TransformFinalBlock(this HashAlgorithm hashAlgorithm, byte[] buffer)
        {
            hashAlgorithm.TransformFinalBlock(buffer, 0, buffer.Length);
        }
    }
}
