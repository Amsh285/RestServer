

namespace RestServer.EndpointHandling
{
    using RestServer.CommunicationObjects;
    using RestServer.EndpointHandling.Attributes;
    using RestServer.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class EndpointControllerReflector
    {
        public static RouteActionMatch SearchRouteActionMatch(string[] actionPathSegments, HttpVerb method, Type controllerType)
        {
            Assert.NotNull(actionPathSegments, nameof(actionPathSegments));
            Assert.NotNull(controllerType, nameof(controllerType));
            Assert.ForEach(actionPathSegments, s => !string.IsNullOrWhiteSpace(s), "actionPathSegment cannot be null empty or whitespace.");

            Type methodAttributeType = supportedMethodTypeMapping[method];

            var methodMatches = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.IsDefined(methodAttributeType, false) && m.ReturnType == typeof(IActionResult))
                .Select(m => new { Method = m, Attr = (HttpMethodAttribute)m.GetCustomAttribute(methodAttributeType) })
                .ToArray();

            var match = methodMatches
                .FirstOrDefault(match => RouteActionMatch.RequestPathMatchesRouteTemplate(match.Attr.GetTemplatePathSegments(), actionPathSegments));

            if (match != null)
                return new RouteActionMatch(controllerType, match.Method, match.Attr);
            else
                throw new NotFoundException($"Couldn´t find an appropriate Action for ActionPath:{string.Join('/', actionPathSegments)} and HttpVerb: {method}");
        }

        private static readonly IReadOnlyDictionary<HttpVerb, Type> supportedMethodTypeMapping =
            new Dictionary<HttpVerb, Type>() {
                { HttpVerb.GET, typeof(HttpGetAttribute) }, { HttpVerb.POST, typeof(HttpPostAttribute) },
                { HttpVerb.PUT, typeof(HttpPutAttribute) }, { HttpVerb.DELETE, typeof(HttpDeleteAttribute) }
            };
    }
}
