namespace Tubumu.Mediasoup
{
    /// <summary>
    /// UDP/TCP socket flags.
    /// </summary>
    public class TransportSocketFlags
    {
        /// <summary>
        /// Disable dual-stack support so only IPv6 is used (only if ip is IPv6).
        /// </summary>
        public bool? Ipv6Only { get; set; }

        /// <summary>
        /// Make different transports bind to the same ip and port (only for UDP).
        /// Useful for multicast scenarios with plain transport.Use with caution.
        /// </summary>
        public bool? UdpReusePort { get; set; }
    }
}
