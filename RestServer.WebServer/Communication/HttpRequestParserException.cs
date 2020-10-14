using System;

namespace RestServer.WebServer.Communication
{
    public sealed class HttpRequestParserException : Exception
    {
        public HttpRequestParserException(string message) : base(message)
        {
        }
    }
}
