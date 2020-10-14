using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace RestServer.WebServer.CommunicationObjects
{
    public sealed class HttpResponseHeader
    {
        public const string Version = "HTTP/1.1";

        public HttpStatusCode Status { get; }

        public int ContentLength { get { return int.Parse(responseHeader[ContentLengthKey]); } }

        public string ContentType { get { return responseHeader[ContentTypeKey]; } }

        public HttpResponseHeader(HttpStatusCode status, int contentLength, string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentException("message", nameof(contentType));
            }

            Status = status;

            responseHeader.Add(ContentLengthKey, contentLength.ToString());
            responseHeader.Add(ContentTypeKey, contentType);

            IReadOnlyDictionary<string, string> defaultHeaderFields = GetDefaultHeaderFields();

            foreach (KeyValuePair<string, string> defaultHeader in defaultHeaderFields)
                responseHeader.Add(defaultHeader.Key, defaultHeader.Value);
        }

        public void Add(string key, string value)
        {
            responseHeader.Add(key, value);
        }

        private string BuildResponseHeaderLine()
        {
            return $"HTTP/1.1 {(int)Status} {Enum.GetName(typeof(HttpStatusCode), Status)}";
        }

        private string BuildResponseHeader()
        {
            StringBuilder responseHeaderBuilder = new StringBuilder();
            responseHeaderBuilder.AppendLine(BuildResponseHeaderLine());

            foreach (KeyValuePair<string, string> headerField in responseHeader.Where(pair => !pair.Key.Equals(ContentLengthKey, StringComparison.OrdinalIgnoreCase)))
                responseHeaderBuilder.AppendLine($"{headerField.Key}: {headerField.Value}");

            responseHeaderBuilder.AppendLine($"{ContentLengthKey}: {ContentLength}");
            responseHeaderBuilder.AppendLine(string.Empty);

            return responseHeaderBuilder.ToString();
        }

        public override string ToString()
        {
            return BuildResponseHeader();
        }

        //Todo: Config
        private IReadOnlyDictionary<string, string> GetDefaultHeaderFields()
        {
            return new Dictionary<string, string>() { { "Server", "Dorian Monster Duel Cards TM ©/ 1.3.3.7" } };
        }

        private Dictionary<string, string> responseHeader = new Dictionary<string, string>();
        private const string ContentLengthKey = "Content - length";
        private const string ContentTypeKey = "Content - type";
    }
}
