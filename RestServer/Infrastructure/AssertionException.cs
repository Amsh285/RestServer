using System;
using System.Collections.Generic;
using System.Text;

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
