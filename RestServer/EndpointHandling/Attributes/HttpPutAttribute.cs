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
