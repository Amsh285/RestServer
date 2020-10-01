using System;
using System.Collections.Generic;
using System.Text;

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
