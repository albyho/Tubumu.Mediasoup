using System;
using System.Text;

namespace Tubumu.Utils.Utilities.Cryptography
{
    /// <summary>
    /// SHA1 加密算法
    /// </summary>
    public static class SHA1
    {
        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public static string Encrypt(string rawString)
        {
            return rawString == null
                ? throw new ArgumentNullException(nameof(rawString))
                : Convert.ToBase64String(EncryptToByteArray(rawString));
        }

        /// <summary>
        /// EncryptToByteArray
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public static byte[] EncryptToByteArray(string rawString)
        {
            if (rawString == null)
            {
                throw new ArgumentNullException(nameof(rawString));
            }

            var salted = Encoding.UTF8.GetBytes(rawString);
            var hasher = System.Security.Cryptography.SHA1.Create();
            var hashed = hasher.ComputeHash(salted);

            return hashed;
        }
    }
}
