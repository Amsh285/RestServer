﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RestServer.EndpointHandling.Attributes
{
    public sealed class HttpPostAttribute : HttpMethodAttribute
    {
        public HttpPostAttribute()
        {
        }

        public HttpPostAttribute(string template)
            : base(template)
        {
        }
    }
}
