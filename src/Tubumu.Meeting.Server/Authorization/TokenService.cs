using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Tubumu.Meeting.Server.Authorization
{
    /// <summary>
    /// Token Service
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly TokenValidationSettings _tokenValidationSettings;
        private readonly IDistributedCache _cache;
        private readonly ILogger<TokenService> _logger;
        private const string CacheKeyFormat = "RefreshToken:{0}";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tokenValidationSettings"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        public TokenService(
            TokenValidationSettings tokenValidationSettings,
            IDistributedCache cache,
            ILogger<TokenService> logger
            )
        {
            _tokenValidationSettings = tokenValidationSettings;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// 生成 Access Token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var utcNow = DateTime.UtcNow;
            var jwtToken = new JwtSecurityToken(
                _tokenValidationSettings.ValidIssuer,
                _tokenValidationSettings.ValidAudience,
                claims,
                notBefore: utcNow,
                expires: utcNow.AddSeconds(_tokenValidationSettings.ExpiresSeconds),
                signingCredentials: SignatureHelper.GenerateSigningCredentials(_tokenValidationSettings.IssuerSigningKey)
            );

            return _tokenHandler.WriteToken(jwtToken);
        }

        /// <summary>
        /// 生成 Refresh Token
        /// </summary>
        /// <returns></returns>
        public Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);
            var cacheKey = CacheKeyFormat.FormatWith(userId);
            _cache.SetStringAsync(cacheKey, refreshToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_tokenValidationSettings.ExpiresSeconds + _tokenValidationSettings.ClockSkewSeconds + _tokenValidationSettings.RefreshTokenExpiresSeconds)
            }).ContinueWithOnFaultedHandleLog(_logger);
            return Task.FromResult(refreshToken);
        }

        /// <summary>
        /// 获取 Refresh Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<string> GetRefreshTokenAsync(int userId)
        {
            var cacheKey = CacheKeyFormat.FormatWith(userId);
            return _cache.GetStringAsync(cacheKey);
        }

        /// <summary>
        /// 废弃 Refresh Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task RevokeRefreshTokenAsync(int userId)
        {
            var cacheKey = CacheKeyFormat.FormatWith(userId);
            return _cache.RemoveAsync(cacheKey);
        }

        /// <summary>
        /// 通过过期 Token 获取 ClaimsPrincipal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SignatureHelper.GenerateSymmetricSecurityKey(_tokenValidationSettings.IssuerSigningKey),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            return securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
                ? throw new SecurityTokenException("Invalid token")
                : principal;
        }
    }
}
