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
        public string ValidIssuer { get; set; }

        /// <summary>
        /// ValidAudience
        /// </summary>
        public string ValidAudience { get; set; }

        /// <summary>
        /// IssuerSigningKey
        /// </summary>
        public string IssuerSigningKey { get; set; }

        /// <summary>
        /// ValidateLifetime
        /// </summary>
        public bool ValidateLifetime { get; set; }

        /// <summary>
        /// ClockSkewSeconds
        /// </summary>
        public int ClockSkewSeconds { get; set; }

        /// <summary>
        /// ExpiresSeconds
        /// </summary>
        public int ExpiresSeconds { get; set; }

        /// <summary>
        /// RefreshTokenExpiresSeconds
        /// </summary>
        public int RefreshTokenExpiresSeconds { get; set; }

        /// <summary>
        /// LoginUrl
        /// </summary>
        public string LoginUrl { get; set; }
    }
}
