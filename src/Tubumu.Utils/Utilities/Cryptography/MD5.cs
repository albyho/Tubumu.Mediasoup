using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Tubumu.Utils.Extensions;

namespace Tubumu.Utils.Utilities.Cryptography
{
    /// <summary>
    /// MD5加密算法
    /// </summary>
    public static class MD5
    {
        /// <summary>
        /// EncryptFromByteArrayToByteArray
        /// </summary>
        /// <param name="inputByteArray"></param>
        /// <returns></returns>
        public static byte[] EncryptFromByteArrayToByteArray(Byte[] inputByteArray)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            return md5.ComputeHash(inputByteArray);
        }

        /// <summary>
        /// EncryptFromStringToByteArray
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static byte[]? EncryptFromStringToByteArray(string encryptString)
        {
            if (encryptString.IsNullOrWhiteSpace())
            {
                return null;
            }

            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            return EncryptFromByteArrayToByteArray(inputByteArray);
        }

        /// <summary>
        /// EncryptFromByteArrayToBase64
        /// </summary>
        /// <param name="inputByteArray"></param>
        /// <returns></returns>
        public static string EncryptFromByteArrayToBase64(Byte[] inputByteArray)
        {
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);
            return Convert.ToBase64String(encryptBuffer);
        }

        /// <summary>
        /// EncryptFromStringToBase64
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string? EncryptFromStringToBase64(string encryptString)
        {
            if (encryptString.IsNullOrWhiteSpace())
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
        /// <param name="inputByteArray"></param>
        /// <returns></returns>
        public static string EncryptFromByteArrayToHex(Byte[] inputByteArray)
        {
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);
            return ByteArrayToHex(encryptBuffer);
        }

        /// <summary>
        /// EncryptFromStringToHex
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string EncryptFromStringToHex(string encryptString)
        {
            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);
            return ByteArrayToHex(encryptBuffer);
        }

        /// <summary>
        /// EncryptFromStringToLowerHex
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string EncryptFromStringToLowerHex(string encryptString)
        {
            var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            var encryptBuffer = EncryptFromByteArrayToByteArray(inputByteArray);
            return ByteArrayToLowerHex(encryptBuffer);
        }

        private static string ByteArrayToHex(IEnumerable<byte> inputByteArray)
        {
            var sb = new StringBuilder();
            foreach (var item in inputByteArray)
            {
                sb.AppendFormat("{0:X2}", item);
            }
            return sb.ToString();
        }

        private static string ByteArrayToLowerHex(IEnumerable<byte> inputByteArray)
        {
            var sb = new StringBuilder();
            foreach (var item in inputByteArray)
            {
                sb.AppendFormat("{0:x2}", item);
            }
            return sb.ToString();
        }
    }
}
