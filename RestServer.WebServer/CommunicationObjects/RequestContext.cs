using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace RestServer.WebServer.CommunicationObjects
{
    public partial class RequestContext
    {
        public RequestLine Request { get; private set; }

        public RequestParameters Parameters { get; private set; }

        public RequestContent Content { get; private set; }

        public CookieCollection Cookies { get; }

        private RequestContext(RequestLine request, RequestParameters parameters, RequestContent content, CookieCollection cookies)
        {
            Assert.NotNull(request, nameof(request));
            Assert.NotNull(parameters, nameof(parameters));
            Assert.NotNull(content, nameof(content));
            Assert.NotNull(cookies, nameof(cookies));

            Request = request;
            Parameters = parameters;
            Content = content;
            Cookies = cookies;
        }

        public static RequestContext Build(RequestLine request, IReadOnlyDictionary<string, string> queryString, Multimap<string, string> headerEntries, NetworkStream inputStream)
        {
            RequestParameters parameters = new RequestParameters(headerEntries, queryString);

            int contentLength = 0;

            if (headerEntries.ContainsKey("Content-Length"))
                int.TryParse(headerEntries["Content-Length"].FirstOrDefault(), out contentLength);

            string contentType = null;

            if (headerEntries.ContainsKey("Content-Type"))
                contentType = headerEntries["Content-Type"].FirstOrDefault();

            RequestContent content = new RequestContent(contentType, contentLength, inputStream);

            return new RequestContext(request, parameters, content, CookieCollection.Build(headerEntries));
        }
    }
}
