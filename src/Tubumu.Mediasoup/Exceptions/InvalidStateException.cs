using System;

namespace Tubumu.Mediasoup
{
    public class InvalidStateException : Exception
    {
        public InvalidStateException(string message) : base(message)
        {
        }

        public InvalidStateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
