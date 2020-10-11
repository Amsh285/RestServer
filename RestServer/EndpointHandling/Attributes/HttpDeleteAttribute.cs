namespace RestServer.EndpointHandling.Attributes
{
    public sealed class HttpDeleteAttribute : HttpMethodAttribute
    {
        public HttpDeleteAttribute()
        {
        }

        public HttpDeleteAttribute(string template)
            : base(template)
        {
        }
    }
}
