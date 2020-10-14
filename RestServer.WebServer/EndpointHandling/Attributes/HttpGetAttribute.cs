namespace RestServer.WebServer.EndpointHandling.Attributes
{
    public sealed class HttpGetAttribute : HttpMethodAttribute
    {
        public HttpGetAttribute()
        {
        }

        public HttpGetAttribute(string template)
            : base(template)
        {
        }
    }
}
