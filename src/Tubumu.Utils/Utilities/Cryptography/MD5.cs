using System;
using System.Collections.Generic;
using System.Text;

namespace Tubumu.Utils.Utilities.Cryptography
{
    /// <summary>
    /// MD5 加密算法
    /// </summary>
    public static class MD5
    {
        /// <summary>
        /// EncryptFromByteArrayToByteArray
        /// </summary>
        public static byte[] EncryptFromByteArrayToByteArray(byte[] inputByteArray)
        {
            var md5 = System.Security.Cryptography.MD5.Create();

            return md5.ComputeHash(inputByteArray);
        }

        /// <summary>
        /// EncryptFromStringToByteArray
        /// </summary>
        public static byte[]? EncryptFromStringToByteArray(string encryptString)
        {
            if(encryptString.IsNullOrWhiteSpace())
            {
                return null;
            }

            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);

            return EncryptFromByteArrayToByteArray(inputByteArray);
        }

        /// <summary>
        /// EncryptFromByteArrayToBase64
        /// </summary>
        public static string EncryptFromByteArrayToBase64(byte[] inputByteArray)
        {
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);

            return Convert.ToBase64String(encryptBuffer);
        }

        /// <summary>
        /// EncryptFromStringToBase64
        /// </summary>
        public static string? EncryptFromStringToBase64(string encryptString)
        {
            if(encryptString.IsNullOrWhiteSpace())
            {
                return null;
            }

            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);

            return Convert.ToBase64String(encryptBuffer);
        }

        /// <summary>
        /// EncryptFromByteArrayToHex
        /// </summary>
        public static string EncryptFromByteArrayToHex(byte[] inputByteArray, bool lower = false)
        {
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);

            return ByteArrayToHex(encryptBuffer, lower);
        }

        /// <summary>
        /// EncryptFromStringToHex
        /// </summary>
        public static string EncryptFromStringToHex(string encryptString, bool lower = false)
        {
            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);

            return ByteArrayToHex(encryptBuffer, lower);
        }

        private static string ByteArrayToHex(IEnumerable<byte> inputByteArray, bool lower)
        {
            var sb = new StringBuilder();
            foreach(var item in inputByteArray)
            {
                sb.AppendFormat(lower ? "{0:x2}" : "{0:X2}", item);
            }

            return sb.ToString();
        }
    }
}
