using System;

namespace Tubumu.Mediasoup
{
    public class DoesNotExistException : Exception
    {
        public DoesNotExistException()
        { }

        public DoesNotExistException(string message)
            : base(message) { }

        public DoesNotExistException(string message, Exception inner)
            : base(message, inner) { }
    }
}
