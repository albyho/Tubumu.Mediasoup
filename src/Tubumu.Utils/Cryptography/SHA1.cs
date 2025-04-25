using System;
using System.Text;

namespace Tubumu.Utils.Cryptography
{
    /// <summary>
    /// SHA1 加密算法
    /// </summary>
    public static class SHA1
    {
        /// <summary>
        /// Encrypt
        /// </summary>
        public static string Encrypt(string rawString)
        {
            return Convert.ToBase64String(EncryptToByteArray(rawString));
        }

        /// <summary>
        /// EncryptToByteArray
        /// </summary>
        public static byte[] EncryptToByteArray(string rawString)
        {
            var salted = Encoding.UTF8.GetBytes(rawString);
            var hasher = System.Security.Cryptography.SHA1.Create();
            var hashed = hasher.ComputeHash(salted);

            return hashed;
        }
    }
}
