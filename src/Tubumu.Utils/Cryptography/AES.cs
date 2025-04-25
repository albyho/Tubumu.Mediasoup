using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tubumu.Utils.Cryptography
{
    /// <summary>
    /// AES 加密解密算法
    /// </summary>
    public static class AES
    {
        private const string DefaultKey = "$uqn.atko@5!7%8*"; // 16 Bytes

        private static readonly byte[] DefaultIV =
        [
            0x96,
            0x47,
            0x22,
            0x18,
            0x69,
            0xCB,
            0xDA,
            0xFE,
            0xAC,
            0xBE,
            0x85,
            0x71,
            0x23,
            0x18,
            0x39,
            0x67
        ];

        #region 加密

        /// <summary>
        /// 字节数组 - 字节数组
        /// </summary>
        public static byte[] EncryptFromByteArrayToByteArray(
            byte[] inputByteArray,
            PaddingMode padding,
            CipherMode mode,
            string? key = null
        )
        {
            //分组加密算法
            var aes = Aes.Create();
            aes.Padding = padding;
            aes.Mode = mode;
            aes.Key = EnsureKey(key);
            aes.IV = DefaultIV;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        /// <summary>
        /// 字节数组 - 字节数组
        /// </summary>
        public static byte[] EncryptFromByteArrayToByteArray(byte[] inputByteArray, string? key = null)
        {
            //分组加密算法
            var aes = Aes.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = EnsureKey(key);
            aes.IV = DefaultIV;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }

        /// <summary>
        /// 字符串 - 字节数组
        /// </summary>
        public static byte[] EncryptFromStringToByteArray(string encryptString, string? key = null)
        {
            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            return EncryptFromByteArrayToByteArray(inputByteArray, key);
        }

        /// <summary>
        /// 字符串 - 16进制
        /// </summary>
        public static string EncryptFromStringToHex(string encryptString, string? key = null, bool lower = false)
        {
            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray, key);
            var sb = new StringBuilder();
            foreach (var item in encryptBuffer)
            {
                sb.AppendFormat(lower ? "{0:x2}" : "{0:X2}", item);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 字符串 - Base64
        /// </summary>
        public static string EncryptFromStringToBase64(string encryptString, string? key = null)
        {
            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray, key);

            return Convert.ToBase64String(encryptBuffer);
        }

        #endregion 加密

        #region 解密

        /// <summary>
        /// 字节数组 - 字节数组
        /// </summary>
        public static byte[] DecryptFromByteArrayToByteArray(byte[] inputByteArray, string? key = null)
        {
            var aes = Aes.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = EnsureKey(key);
            aes.IV = DefaultIV;

            var decryptBytes = new byte[inputByteArray.Length];
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            _ = cs.Read(decryptBytes, 0, decryptBytes.Length);

            return decryptBytes;
        }

        /// <summary>
        /// 字符串 - 字节数组
        /// </summary>
        public static byte[] DecryptFromHexToByteArray(string decryptString, string? key = null)
        {
            var buffer = ByteArrayFromHexString(decryptString);

            return DecryptFromByteArrayToByteArray(buffer, key);
        }

        /// <summary>
        /// 字符串 - 字符串
        /// </summary>
        public static string DecryptFromHexToString(string decryptString, string? key = null)
        {
            var buffer = ByteArrayFromHexString(decryptString);
            return Encoding.UTF8.GetString(DecryptFromByteArrayToByteArray(buffer, key)).Replace("\0", "");
        }

        /// <summary>
        /// Base64字符串 - 字节数组
        /// </summary>
        public static byte[] DecryptFromBase64ToByteArray(string decryptString, string? key = null)
        {
            var buffer = Convert.FromBase64String(decryptString);

            return DecryptFromByteArrayToByteArray(buffer, key);
        }

        #endregion 解密

        #region Private Methods

        private static byte[] ByteArrayFromHexString(string hexString)
        {
            var buffer = new byte[hexString.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return buffer;
        }

        private static byte[] EnsureKey(string? key)
        {
            if (key != null)
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);
                return keyBytes.Length < 16
                        ? throw new ArgumentOutOfRangeException(nameof(key), "key 经过 UTF8 编码后的长度至少需要 16 个字节")
                    : keyBytes.Length == 16 ? keyBytes
                    : keyBytes[0..16];
            }

            return Encoding.UTF8.GetBytes(DefaultKey);
        }

        #endregion Private Methods
    }
}
