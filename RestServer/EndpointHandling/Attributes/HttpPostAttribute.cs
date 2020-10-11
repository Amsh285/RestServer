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
