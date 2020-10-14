using System;

namespace RestServer.WebServer.Infrastructure
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
