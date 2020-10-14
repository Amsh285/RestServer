using System;

namespace RestServer.WebServer.EndpointHandling
{
    public sealed class RequestBodyExtractorException : Exception
    {
        public RequestBodyExtractorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
