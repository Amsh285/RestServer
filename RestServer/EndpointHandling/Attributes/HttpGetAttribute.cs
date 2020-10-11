namespace RestServer.EndpointHandling.Attributes
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
