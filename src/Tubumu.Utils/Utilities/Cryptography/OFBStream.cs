using System;
using System.IO;
using System.Security.Cryptography;

namespace Tubumu.Utils.Utilities.Cryptography
{
    /// <summary>
    /// OFBStream
    /// </summary>
    public class OFBStream : Stream
    {
        private const int Blocks = 16;
        private const int EOS = 0; // the goddess of dawn is found at the end of the stream

        private readonly Stream _parent;
        private readonly CryptoStream _cbcStream;
        private readonly CryptoStreamMode _mode;
        private readonly byte[] _keyStreamBuffer;
        private int _keyStreamBufferOffset;
        private readonly byte[] _readWriteBuffer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="algo"></param>
        /// <param name="mode"></param>
        public OFBStream(Stream parent, SymmetricAlgorithm algo, CryptoStreamMode mode)
        {
            //if (algo.Mode != CipherMode.CBC)
            algo.Mode = CipherMode.CBC;
            //if (algo.Padding != PaddingMode.None)
            algo.Padding = PaddingMode.None;
            this._parent = parent;
            this._cbcStream = new CryptoStream(new ZeroStream(), algo.CreateEncryptor(), CryptoStreamMode.Read);
            this._mode = mode;
            _keyStreamBuffer = new byte[algo.BlockSize * Blocks];
            _readWriteBuffer = new byte[_keyStreamBuffer.Length];
        }

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
            {
                throw new NotSupportedException("Cannot read.");
            }

            int toRead = Math.Min(count, _readWriteBuffer.Length);
            int read = _parent.Read(_readWriteBuffer, 0, toRead);
            if (read == EOS)
            {
                return EOS;
            }

            for (int i = 0; i < read; i++)
            {
                // NOTE could be optimized (branches for each byte)
                if (_keyStreamBufferOffset % _keyStreamBuffer.Length == 0)
                {
                    FillKeyStreamBuffer();
                    _keyStreamBufferOffset = 0;
                }

                buffer[offset + i] = (byte)(_readWriteBuffer[i]
                    ^ _keyStreamBuffer[_keyStreamBufferOffset++]);
            }

            return read;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
            {
                throw new NotSupportedException("Cannot write.");
            }

            int readWriteBufferOffset = 0;
            for (int i = 0; i < count; i++)
            {
                if (_keyStreamBufferOffset % _keyStreamBuffer.Length == 0)
                {
                    FillKeyStreamBuffer();
                    _keyStreamBufferOffset = 0;
                }

                if (readWriteBufferOffset % _readWriteBuffer.Length == 0)
                {
                    _parent.Write(_readWriteBuffer, 0, readWriteBufferOffset);
                    readWriteBufferOffset = 0;
                }

                _readWriteBuffer[readWriteBufferOffset++] = (byte)(buffer[offset + i]
                    ^ _keyStreamBuffer[_keyStreamBufferOffset++]);
            }

            _parent.Write(_readWriteBuffer, 0, readWriteBufferOffset);
        }

        private void FillKeyStreamBuffer()
        {
            int read = _cbcStream.Read(_keyStreamBuffer, 0, _keyStreamBuffer.Length);
            // NOTE undocumented feature
            // only works if keyStreamBuffer.Length % blockSize == 0
            if (read != _keyStreamBuffer.Length)
            {
                throw new InvalidOperationException("Implementation error: could not read all bytes from CBC stream");
            }
        }

        /// <summary>
        /// CanRead
        /// </summary>
        public override bool CanRead
        {
            get { return _mode == CryptoStreamMode.Read; }
        }

        /// <summary>
        /// CanWrite
        /// </summary>
        public override bool CanWrite
        {
            get { return _mode == CryptoStreamMode.Write; }
        }

        /// <summary>
        /// Flush
        /// </summary>
        public override void Flush()
        {
            // should never have to be flushed, implementation empty
        }

        /// <summary>
        /// CanSeek
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Seek
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(nameof(Seek));
        }

        /// <summary>
        /// Position
        /// </summary>
        public override long Position
        {
            get { throw new NotSupportedException(nameof(Position)); }
            set { throw new NotSupportedException(nameof(Position)); }
        }

        /// <summary>
        /// Length
        /// </summary>
        public override long Length
        {
            get { throw new NotSupportedException(nameof(Length)); }
        }

        /// <summary>
        /// SetLength
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException(nameof(SetLength));
        }
    }

    internal class ZeroStream : Stream
    {
        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = 0;
            }

            return count;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        // ... the rest is not implemented
        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}

/*

// NIST CAVP test vector F.4.1: OFB-AES128.Encrypt from NIST SP 800-38A

RijndaelManaged aes = new RijndaelManaged ();
aes.Key = FromHex("2b7e151628aed2a6abf7158809cf4f3c");
aes.IV = FromHex("000102030405060708090A0B0C0D0E0F");
MemoryStream testVectorStream = new MemoryStream (FromHex (
    "6bc1bee22e409f96e93d7e117393172aae2d8a571e03ac9c9eb76fac45af8e5130c81c46a35ce411e5fbc1191a0a52eff69f2445df4f9b17ad2b417be66c3710"));
OFBStream testOFBStream = new OFBStream (testVectorStream, aes, CryptoStreamMode.Read);
MemoryStream cipherTextStream = new MemoryStream ();
testOFBStream.CopyTo (cipherTextStream);
Console.WriteLine (ToHex (cipherTextStream.ToArray ()));

 */
