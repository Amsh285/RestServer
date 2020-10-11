using System;

namespace RestServer.Infrastructure
{
    public class AssertionException : Exception
    {
        public AssertionException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
