namespace Tubumu.Meeting.Server.Authorization
{
    /// <summary>
    /// TokenValidation settings
    /// </summary>
    public class TokenValidationSettings
    {
        /// <summary>
        /// ValidIssuer
        /// </summary>
        public string ValidIssuer { get; init; }

        /// <summary>
        /// ValidAudience
        /// </summary>
        public string ValidAudience { get; init; }

        /// <summary>
        /// IssuerSigningKey
        /// </summary>
        public string IssuerSigningKey { get; init; }

        /// <summary>
        /// ValidateLifetime
        /// </summary>
        public bool ValidateLifetime { get; init; }

        /// <summary>
        /// ClockSkewSeconds
        /// </summary>
        public int ClockSkewSeconds { get; init; }

        /// <summary>
        /// ExpiresSeconds
        /// </summary>
        public int ExpiresSeconds { get; init; }

        /// <summary>
        /// RefreshTokenExpiresSeconds
        /// </summary>
        public int RefreshTokenExpiresSeconds { get; init; }

        /// <summary>
        /// LoginUrl
        /// </summary>
        public string LoginUrl { get; init; }
    }
}
