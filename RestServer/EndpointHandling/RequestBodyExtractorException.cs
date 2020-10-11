using System;

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
