using System;

namespace RestServer.WebServer.EndpointHandling
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
