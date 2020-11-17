namespace RestServer.WebServer.CommunicationObjects
{
    public partial class RequestContext
    {
        public sealed class RequestLine
        {
            public string HttpVersion { get; private set; }

            public string Path { get; private set; }

            public string FullPath { get; }

            public HttpVerb Method { get; private set; }

            public RequestLine(string httpVersion, string path, string fullPath, HttpVerb method)
            {
                HttpVersion = httpVersion;
                Path = path;
                FullPath = fullPath;
                Method = method;
            }
        }
    }
}
