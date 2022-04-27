using System;
using System.Text;

namespace Tubumu.Utils.Utilities.Cryptography
{
    /// <summary>
    /// SHA256加密算法
    /// </summary>
    public static class SHA256
    {
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="rawString"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string Encrypt(string rawString, string salt)
        {
            if (rawString == null)
            {
                throw new ArgumentNullException(nameof(rawString));
            }
            if (salt == null)
            {
                throw new ArgumentNullException(nameof(salt));
            }
            return Convert.ToBase64String(EncryptToByteArray(rawString, salt));
        }

        /// <summary>
        /// EncryptToByteArray
        /// </summary>
        /// <param name="rawString"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] EncryptToByteArray(string rawString, string salt)
        {
            if (rawString == null)
            {
                throw new ArgumentNullException(nameof(rawString));
            }
            if (salt == null)
            {
                throw new ArgumentNullException(nameof(salt));
            }
            var salted = Encoding.UTF8.GetBytes(string.Concat(rawString, salt));
            var hasher = System.Security.Cryptography.SHA256.Create();
            var hashed = hasher.ComputeHash(salted);
            return hashed;
        }
    }
}
