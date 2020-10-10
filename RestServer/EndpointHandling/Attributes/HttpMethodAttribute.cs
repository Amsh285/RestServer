using RestServer.Infrastructure;
using System;

namespace RestServer.EndpointHandling.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class HttpMethodAttribute : Attribute
    {
        public string Template { get; }

        public HttpMethodAttribute()
            : this(string.Empty)
        {
        }

        public HttpMethodAttribute(string template)
        {
            Assert.NotNull(template, nameof(template));

            Template = template;
        }

        public string[] GetTemplatePathSegments()
        {
            if (string.IsNullOrWhiteSpace(Template) || Template.Equals(".") || Template.Equals("./") || Template.Equals("/"))
                return new string[0];

            return Template.Split("/");
        }
    }
}
