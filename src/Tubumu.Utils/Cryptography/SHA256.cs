using System;
using System.Text;

namespace Tubumu.Utils.Cryptography
{
    /// <summary>
    /// SHA256 加密算法
    /// </summary>
    public static class SHA256
    {
        /// <summary>
        /// Encrypt
        /// </summary>
        public static string Encrypt(string rawString, string salt)
        {
            return Convert.ToBase64String(EncryptToByteArray(rawString, salt));
        }

        /// <summary>
        /// EncryptToByteArray
        /// </summary>
        public static byte[] EncryptToByteArray(string rawString, string salt)
        {
            var salted = Encoding.UTF8.GetBytes(string.Concat(rawString, salt));
            var hasher = System.Security.Cryptography.SHA256.Create();
            var hashed = hasher.ComputeHash(salted);

            return hashed;
        }
    }
}
