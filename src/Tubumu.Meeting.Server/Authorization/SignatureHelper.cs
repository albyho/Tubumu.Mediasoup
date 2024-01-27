using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Tubumu.Meeting.Server.Authorization
{
    /// <summary>
    /// 签名帮助类
    /// </summary>
    public static class SignatureHelper
    {
        /// <summary>
        /// 生成签名凭据
        /// </summary>
        public static SigningCredentials GenerateSigningCredentials(string secretKey)
        {
            var signingKey = GenerateSymmetricSecurityKey(secretKey);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            return signingCredentials;
        }

        /// <summary>
        /// 生成签名 Key
        /// </summary>
        public static SymmetricSecurityKey GenerateSymmetricSecurityKey(string secretKey)
        {
            var keyByteArray = Encoding.UTF8.GetBytes(secretKey);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            return signingKey;
        }
    }
}
