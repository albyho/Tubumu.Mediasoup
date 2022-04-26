using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Tubumu.Meeting.Server.Authorization
{
    /// <summary>
    /// Token Service
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// 生成 Access Token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        string GenerateAccessToken(IEnumerable<Claim> claims);

        /// <summary>
        /// 生成 Refresh Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GenerateRefreshTokenAsync(int userId);

        /// <summary>
        /// 获取 Refresh Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetRefreshTokenAsync(int userId);

        /// <summary>
        /// 废弃 Refresh Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task RevokeRefreshTokenAsync(int userId);

        /// <summary>
        /// 通过过期 Token 获取 ClaimsPrincipal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
