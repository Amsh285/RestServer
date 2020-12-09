using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestServer.WebServer.CommunicationObjects
{
    public sealed class CookieCollection
    {
        public IReadOnlyDictionary<string, string> CookieContainer { get { return cookieContainer; } }

        private CookieCollection(Dictionary<string, string> cookieContainer)
        {
            Assert.NotNull(cookieContainer, nameof(cookieContainer));

            this.cookieContainer = cookieContainer;
        }

        public bool Exists(string name)
        {
            return cookieContainer.ContainsKey(name);
        }

        public string this[string name]
        {
            get
            {
                return cookieContainer[name];
            }
        }

        public static CookieCollection Build(Multimap<string, string> requestHeader, bool overrideKey = true)
        {
            Assert.NotNull(requestHeader, nameof(requestHeader));

            Dictionary<string, string> cookieContainer = new Dictionary<string, string>();

            foreach (KeyValuePair<string, List<string>> headerEntry in requestHeader.Where(p => p.Key.Equals("cookie", StringComparison.OrdinalIgnoreCase)))
                foreach (string cookie in headerEntry.Value)
                {
                    string[] cookieSegments = cookie.Split("=", 2);

                    if (cookieSegments.Length != 2)
                        Console.WriteLine($"Warning invalid Request- Cookie: {cookie}.");

                    string name = cookieSegments[0];
                    string value = cookieSegments[1];

                    if (!cookieContainer.ContainsKey(name))
                        cookieContainer.Add(name, value);
                    else if (overrideKey)
                        cookieContainer[name] = value;
                }

            return new CookieCollection(cookieContainer);
        }

        private readonly Dictionary<string, string> cookieContainer;
    }
}
