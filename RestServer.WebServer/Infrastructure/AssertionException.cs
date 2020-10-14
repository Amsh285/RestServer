using System;

namespace RestServer.WebServer.Infrastructure
{
    public class AssertionException : Exception
    {
        public AssertionException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
