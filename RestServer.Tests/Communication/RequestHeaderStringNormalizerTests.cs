using NUnit.Framework;
using RestServer.WebServer.Communication;
using System;

namespace RestServer.UnitTests.Communication
{
    [TestFixture]
    public sealed class RequestHeaderStringNormalizerTests
    {
        [TestCase("Hallo\r\nWelt\r\n")]
        [TestCase("Hallo\nWelt\n")]
        public void NormalizesAsExpected(string value)
        {
            string result = RequestHeaderStringNormalizer.NormalizeForApplication(value);
            string expected = $"Hallo{Environment.NewLine}Welt{Environment.NewLine}";

            StringAssert.AreEqualIgnoringCase(expected, result);
        }
    }
}
