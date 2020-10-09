using NUnit.Framework;
using RestServer.CommunicationObjects;
using RestServer.EndpointHandling;
using RestServer.EndpointHandling.Attributes;
using RestServer.Tests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestServer.UnitTests.EndpointHandling
{
    [TestFixture]
    public sealed class EndpointControllerReflectorTests
    {
        [TestCase("", HttpVerb.GET)]
        [TestCase("SubResources", HttpVerb.GET)]
        [TestCase("1", HttpVerb.GET)]
        [TestCase("1/SubResource", HttpVerb.GET)]
        [TestCase("1/SubResource/1", HttpVerb.GET)]
        public void ActionRoutingWorksAsIntended(string actionRoute, HttpVerb method)
        {
            string[] actionRouteSegments = actionRoute
                    .Split("/")
                    .Where(segment => !string.IsNullOrWhiteSpace(segment))
                    .ToArray();

            RouteActionMatch result = EndpointControllerReflector.SearchRouteActionMatch(actionRouteSegments, method, typeof(TestController));

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.HttpMethod);

            Assert.AreEqual(typeof(TestController), result.Controller);
            Assert.AreEqual(HttpVerbAttributeTypeMapping[method], result.HttpMethod.GetType());
            RouteTemplateIsValidForActionRoute(actionRouteSegments, result.HttpMethod);
        }

        private static void RouteTemplateIsValidForActionRoute(string[] actionRouteSegments, HttpMethodAttribute actual)
        {
            Assert.IsNotNull(actual);

            string[] templatePathSegments = actual.GetTemplatePathSegments();
            Assert.AreEqual(actionRouteSegments.Length, templatePathSegments.Length);

            for (int i = 0; i < templatePathSegments.Length; ++i)
            {
                if (!templatePathSegments[i].StartsWith('{') && !templatePathSegments[i].EndsWith('}'))
                    StringAssert.AreEqualIgnoringCase(actionRouteSegments[i], templatePathSegments[i]);
            }
        }

        private static readonly IReadOnlyDictionary<HttpVerb, Type> HttpVerbAttributeTypeMapping =
            new Dictionary<HttpVerb, Type>() {
                { HttpVerb.GET, typeof(HttpGetAttribute) }, { HttpVerb.POST, typeof(HttpPostAttribute) },
                { HttpVerb.PUT, typeof(HttpPutAttribute) }, { HttpVerb.DELETE, typeof(HttpDeleteAttribute) }
            };
    }
}
