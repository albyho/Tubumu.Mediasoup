using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tubumu.Utils.Utilities.Cryptography
{
    /// <summary>
    /// DES 加密解密算法(默认采用的是ECB模式)
    /// <para>字节数组 -> 字节数组</para>
    /// <para>字节数组 -> Base64</para>
    /// <para>字符串   -> 字节数组</para>
    /// <para>字符串   -> Base64</para>
    /// <para>字符串   -> Base64(指定填充模式)</para>
    /// <para>字符串   -> Hex</para>
    /// </summary>
    public static class DES
    {
        private const string DefaultKey = "$uo@5%8*";

        #region 加密

        /// <summary>
        /// 核心方法 EncryptFromByteArrayToByteArray
        /// </summary>
        public static byte[] EncryptFromByteArrayToByteArray(
            byte[] inputByteArray,
            CipherMode mode,
            PaddingMode paddingMode,
            string? key = null
        )
        {
            if (inputByteArray == null)
            {
                throw new ArgumentNullException(nameof(inputByteArray));
            }

            var keyBytes = EnsureKey(key);
            var keyIV = keyBytes;
            var des = TripleDES.Create();
            des.Mode = mode;
            des.Padding = paddingMode;
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, des.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();

            return mStream.ToArray();
        }

        /// <summary>
        /// EncryptFromByteArrayToByteArray
        /// </summary>
        public static byte[] EncryptFromByteArrayToByteArray(byte[] inputByteArray, string? key = null)
        {
            return EncryptFromByteArrayToByteArray(inputByteArray, CipherMode.ECB, PaddingMode.Zeros, key);
        }

        /// <summary>
        /// EncryptFromByteArrayToBase64String
        /// </summary>
        public static string EncryptFromByteArrayToBase64String(byte[] inputByteArray, string? key = null)
        {
            return Convert.ToBase64String(EncryptFromByteArrayToByteArray(inputByteArray, key));
        }

        /// <summary>
        /// EncryptFromStringToByteArray
        /// </summary>
        public static byte[] EncryptFromStringToByteArray(string encryptString, string? key = null)
        {
            if (encryptString == null)
            {
                throw new ArgumentNullException(nameof(encryptString));
            }

            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            return EncryptFromByteArrayToByteArray(inputByteArray, key);
        }

        /// <summary>
        /// EncryptFromStringToBase64String
        /// </summary>
        public static string EncryptFromStringToBase64String(string encryptString, string? key = null)
        {
            if (encryptString == null)
            {
                throw new ArgumentNullException(nameof(encryptString));
            }

            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            return EncryptFromByteArrayToBase64String(inputByteArray, key);
        }

        /// <summary>
        /// EncryptFromStringToBase64String
        /// </summary>
        public static string EncryptFromStringToBase64String(string encryptString, PaddingMode paddingMode, string? key = null)
        {
            if (encryptString == null)
            {
                throw new ArgumentNullException(nameof(encryptString));
            }

            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            var keyBytes = EnsureKey(key);
            var keyIV = keyBytes;

            var des = TripleDES.Create();
            des.Mode = CipherMode.ECB;
            des.Padding = paddingMode;
            using var mStream = new MemoryStream();
            using var cStream = new CryptoStream(mStream, des.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();

            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// EncryptFromStringToHex
        /// </summary>
        public static string EncryptFromStringToHex(string encryptString, string? key = null)
        {
            var encryptBuffer = EncryptFromStringToByteArray(encryptString, key);

            return HexFromByteArray(encryptBuffer);
        }

        #endregion 加密

        #region 解密

        /// <summary>
        /// 核心方法 DecryptFromByteArrayToByteArray
        /// </summary>
        public static byte[] DecryptFromByteArrayToByteArray(
            byte[] inputByteArray,
            CipherMode mode,
            PaddingMode paddingMode,
            string? key = null
        )
        {
            if (inputByteArray == null)
            {
                throw new ArgumentNullException(nameof(inputByteArray));
            }

            var keyBytes = EnsureKey(key);
            var keyIV = keyBytes;
            var des = TripleDES.Create();
            des.Mode = mode;
            des.Padding = paddingMode;
            using var mStream = new MemoryStream();
            using var cStream = new CryptoStream(mStream, des.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();

            return mStream.ToArray();
        }

        /// <summary>
        /// DecryptFromByteArrayToByteArray
        /// </summary>
        public static byte[] DecryptFromByteArrayToByteArray(byte[] inputByteArray, string? key = null)
        {
            return DecryptFromByteArrayToByteArray(inputByteArray, CipherMode.ECB, PaddingMode.Zeros, key);
        }

        /// <summary>
        /// DecryptFromByteArrayToString
        /// </summary>
        public static string DecryptFromByteArrayToString(byte[] inputByteArray, string? key = null)
        {
            return inputByteArray == null
                ? throw new ArgumentNullException(nameof(inputByteArray))
                : Encoding.UTF8.GetString(DecryptFromByteArrayToByteArray(inputByteArray, key));
        }

        /// <summary>
        /// DecryptFromBase64StringToByteArray
        /// </summary>
        public static byte[] DecryptFromBase64StringToByteArray(string decryptBase64String, string? key = null)
        {
            if (decryptBase64String == null)
            {
                throw new ArgumentNullException(nameof(decryptBase64String));
            }

            var inputByteArray = Convert.FromBase64String(decryptBase64String);

            return DecryptFromByteArrayToByteArray(inputByteArray, key);
        }

        /// <summary>
        /// DecryptFromBase64StringToString
        /// </summary>
        public static string DecryptFromBase64StringToString(string decryptBase64String, PaddingMode paddingMode, string key)
        {
            if (decryptBase64String == null)
            {
                throw new ArgumentNullException(nameof(decryptBase64String));
            }

            var inputByteArray = Convert.FromBase64String(decryptBase64String);
            var keyBytes = EnsureKey(key);
            var keyIV = keyBytes;
            var des = TripleDES.Create();
            des.Mode = CipherMode.ECB;
            des.Padding = paddingMode;
            using var mStream = new MemoryStream();
            using var cStream = new CryptoStream(mStream, des.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        /// <summary>
        /// DecryptFromHexToString
        /// </summary>
        public static string DecryptFromHexToString(string decryptString, string? key = null)
        {
            var decryptBuffer = new byte[decryptString.Length / 2];
            for (var i = 0; i < decryptBuffer.Length; i++)
            {
                decryptBuffer[i] = Convert.ToByte(decryptString.Substring(i * 2, 2), 16);
            }

            return Encoding.UTF8.GetString(DecryptFromByteArrayToByteArray(decryptBuffer, key)).Replace("\0", "");
        }

        #endregion 解密

        /// <summary>
        /// HexFromByteArray
        /// </summary>
        public static string HexFromByteArray(byte[]? value)
        {
            if (value == null || value.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var item in value)
            {
                sb.AppendFormat("{0:X2}", item);
            }

            return sb.ToString();
        }

        #region Private Methods

        private static byte[] EnsureKey(string? key)
        {
            if (key != null)
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);
                return keyBytes.Length < 8
                        ? throw new ArgumentOutOfRangeException(nameof(key), "key 经过 UTF8 编码后的长度至少需要 8 个字节")
                    : keyBytes.Length == 8 ? keyBytes
                    : keyBytes[0..8];
            }

            return Encoding.UTF8.GetBytes(DefaultKey);
        }

        #endregion Private Methods
    }
}
