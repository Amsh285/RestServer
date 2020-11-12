using MonsterTradingCardGame.Entities;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using RestServer.WebServer.Infrastructure;
using System;
using System.Globalization;
using System.Text;

namespace MonsterTradingCardGame.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(RequestContext requestContext)
        {
            Assert.NotNull(requestContext, nameof(requestContext));

            this.requestContext = requestContext;
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if(requestContext.Parameters.Headers.ContainsKey(AuthenticationTokenKey))
                return Ok("Already logged in.");

            if (userEntity.Authenticate(username, Encoding.UTF8.GetBytes(password)))
            {
                return AuthenticationSuccess();
            }

            return Unauthorized("Authentication Failed.");
        }

        private IActionResult AuthenticationSuccess()
        {
            string epirationDate = $"{DateTime.Now.AddMinutes(2).ToString("ddd, dd MMM yyy HH:mm:ss", new CultureInfo("En-en"))} GMT";

            StringBuilder authenticationCookieContent = new StringBuilder();
            authenticationCookieContent.Append($"{AuthenticationTokenKey}=Test12345;");
            authenticationCookieContent.Append($"expires={epirationDate}");

            IActionResult result = Ok("Authentication Successful.");
            result.AddHeaderEntry("Set-Cookie", authenticationCookieContent.ToString());
            return result;
        }

        private const string AuthenticationTokenKey = "AuthToken";
        private readonly RequestContext requestContext;
        private static readonly UserEntity userEntity = new UserEntity();
    }
}
