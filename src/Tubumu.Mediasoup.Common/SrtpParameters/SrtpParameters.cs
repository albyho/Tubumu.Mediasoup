namespace Tubumu.Mediasoup
{
    /// <summary>
    /// SRTP parameters.
    /// </summary>
    public class SrtpParameters
    {
        /// <summary>
        /// Encryption and authentication transforms to be used.
        /// </summary>
        public SrtpCryptoSuite CryptoSuite { get; set; }

        /// <summary>
        /// SRTP keying material (master key and salt) in Base64.
        /// </summary>
        public string KeyBase64 { get; set; }
    }
}
