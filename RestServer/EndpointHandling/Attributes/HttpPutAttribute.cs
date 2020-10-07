using System;
using System.Collections.Generic;
using System.Text;

namespace RestServer.EndpointHandling.Attributes
{
    public sealed class HttpPutAttribute : HttpMethodAttribute
    {
        public HttpPutAttribute()
        {
        }

        public HttpPutAttribute(string template)
            : base(template)
        {
        }
    }
}
