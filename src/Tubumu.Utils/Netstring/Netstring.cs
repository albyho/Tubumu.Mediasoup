using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tubumu.Utils
{
    public struct Payload
    {
        public ArraySegment<byte> Data { get; }

        public int NetstringLength { get; }

        public Payload(ArraySegment<byte> data, int netstringLength)
        {
            Data = data;
            NetstringLength = netstringLength;
        }
    }

    public sealed class Netstring : IEnumerator<Payload>, IEnumerable<Payload>
    {
        private readonly ArraySegment<byte> _buffer;
        private int _offset;

        public Netstring(ArraySegment<byte> buffer, int offset = 0)
        {
            _buffer = buffer;
            _offset = offset;
        }

        /// <summary>
        /// Emits the specified string as a netstring.
        /// </summary>
        /// <param name="value">The string to encode as a netstring.</param>
        /// <returns>A netstring.</returns>
        public static byte[] Encode(string value)
        {
            return Encode(Encoding.UTF8.GetBytes(value));
        }

        public static byte[] Encode(byte[] value)
        {
            var payloadLengthBuffer = Encoding.UTF8.GetBytes(value.Length.ToString());
            var netstringLenth = payloadLengthBuffer.Length + 1 + value.Length + 1;
            var result = new byte[netstringLenth];
            Array.Copy(payloadLengthBuffer, 0, result, 0, payloadLengthBuffer.Length);
            result[payloadLengthBuffer.Length] = (byte)':';
            Array.Copy(value, 0, result, payloadLengthBuffer.Length + 1, value.Length);
            result[result.Length - 1] = (byte)',';
            return result;
        }

        #region IEnumerator, IEnumerable

        public bool MoveNext()
        {
            if (_offset > _buffer.Count - 3)
            {
                return false;
            }

            Current = ExtractPayload();

            return true;
        }

        public Payload Current { get; private set; }

        object IEnumerator.Current => Current;

#pragma warning disable CA1063 // Implement IDisposable Correctly

        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Payload> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion IEnumerator, IEnumerable

        private static int ExtractPayloadLength(ArraySegment<byte> buffer, int offset)
        {
            var len = 0;
            int i;
            for (i = offset; i < buffer.Count; i++)
            {
                var cc = buffer.Array![buffer.Offset + i];

                if (cc == ':'/*0x3a*/)
                {
                    if (i == offset)
                    {
                        throw new Exception("Invalid netstring with leading ':'");
                    }

                    return len;
                }

                if (cc < '0'/*0x30*/ || cc > '9'/*0x39*/)
                {
                    throw new Exception($"Unexpected character {cc} found at offset");
                }

                if (len == 0 && i > offset)
                {
                    throw new Exception("Invalid netstring with leading 0");
                }

                len = len * 10 + cc - '0'/*0x30*/;
            }

            // We didn't get a complete length specification
            if (i == buffer.Count)
            {
                return -1;
            }

            return len;
        }

        private static int ComputeNetstringLength(int payloadLength)
        {
            // Negative values are special (see nsPayloadLength()); just return it
            if (payloadLength < 0)
            {
                return payloadLength;
            }

            // Compute the number of digits in the length specifier. Stop at
            // any value < 10 and just add 1 later (this catches the case where
            // '0' requires a digit.
            var nslen = payloadLength;
            while (payloadLength >= 10)
            {
                nslen += 1;
                payloadLength /= 10;
            }

            // nslen + 1 (last digit) + 1 (:) + 1 (,)
            return nslen + 3;
        }

        private Payload ExtractPayload()
        {
            var payloadLength = ExtractPayloadLength(_buffer, _offset);
            if (payloadLength < 0)
            {
                throw new InvalidDataException("Illegal size field");
            }

            var netstringLength = ComputeNetstringLength(payloadLength);

            // We don't have the entire buffer yet
            if (_buffer.Count - _offset - netstringLength < 0)
            {
                throw new InvalidDataException("Don't have the entire buffer yet");
            }

            var start = _offset + (netstringLength - payloadLength - 1);
            _offset += netstringLength;
            return new Payload(new ArraySegment<byte>(_buffer.Array!, start, payloadLength), netstringLength);
        }
    }
}
