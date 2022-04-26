using System;

namespace Tubumu.Mediasoup
{
    public class DisconnectedException : Exception
    {
        public DisconnectedException(string message) : base(message)
        {
        }

        public DisconnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
