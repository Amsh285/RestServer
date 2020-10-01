using System;

namespace RestServer.EndpointHandling.Attributes
{
    public abstract class HttpMethodAttribute : Attribute
    {
        public string Template { get; }

        public HttpMethodAttribute()
            : this(null)
        {
        }

        public HttpMethodAttribute(string template)
        {
            Template = template;
        }
    }
}
