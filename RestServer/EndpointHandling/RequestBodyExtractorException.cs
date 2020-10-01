using System;
using System.Collections.Generic;
using System.Text;

namespace RestServer.EndpointHandling
{
    public sealed class RequestBodyExtractorException : Exception
    {
        public RequestBodyExtractorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
