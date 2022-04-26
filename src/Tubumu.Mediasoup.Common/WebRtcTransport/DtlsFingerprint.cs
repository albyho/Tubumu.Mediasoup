namespace Tubumu.Mediasoup
{
    /// <summary>
    /// The hash function algorithm (as defined in the "Hash function Textual Names"
    /// registry initially specified in RFC 4572 Section 8) and its corresponding
    /// certificate fingerprint value (in lowercase hex string as expressed utilizing
    /// the syntax of "fingerprint" in RFC 4572 Section 5).
    /// </summary>
    public class DtlsFingerprint
    {
        public string Algorithm { get; set; }

        public string Value { get; set; }
    }
}
