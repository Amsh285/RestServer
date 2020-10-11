using System;

namespace RestServer.EndpointHandling
{
    public sealed class EndPointHandlerException : Exception
    {
        public EndPointHandlerException(string message)
            : base(message)
        {
        }

        public EndPointHandlerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
