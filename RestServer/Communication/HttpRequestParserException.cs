using System;

namespace RestServer.Communication
{
    public sealed class HttpRequestParserException : Exception
    {
        public HttpRequestParserException(string message) : base(message)
        {
        }
    }
}
