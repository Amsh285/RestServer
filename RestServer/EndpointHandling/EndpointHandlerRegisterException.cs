using System;

namespace RestServer.EndpointHandling
{
    public sealed class EndpointHandlerRegisterException : Exception
    {
        public EndpointHandlerRegisterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
