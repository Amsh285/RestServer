using RestServer.WebServer.Infrastructure;
using System.Collections.Generic;

namespace RestServer.WebServer.CommunicationObjects
{
    public partial class RequestContext
    {
        public sealed class RequestParameters
        {
            public IReadOnlyDictionary<string, string> QueryStringValues { get; private set; }

            public Multimap<string, string> HeaderEntries { get; private set; }

            public RequestParameters(Multimap<string, string> headerEntries, IReadOnlyDictionary<string, string> queryStringValues)
            {
                HeaderEntries = headerEntries;
                QueryStringValues = queryStringValues;
            }
        }
    }
}
