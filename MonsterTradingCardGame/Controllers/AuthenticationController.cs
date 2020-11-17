using MonsterTradingCardGame.Entities.UserEntity;
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
            LoginResult result = userEntity.Login(username, Encoding.UTF8.GetBytes(password));
            Assert.NotNull(result, nameof(result));

            if (result.AuthenticationResult == AuthenticationResult.Failed)
                return Unauthorized("Authentication Failed.");
            else if (result.AuthenticationResult == AuthenticationResult.AlreadyLoggedIn)
                return Ok("Already logged in.");
            else
            {
                Assert.NotNull(result.AuthenticationToken, nameof(result.AuthenticationToken));
                Assert.NotNull(result.AuthenticationTokenExpirationDate, nameof(result.AuthenticationTokenExpirationDate));
                return AuthenticationSuccess(result.AuthenticationToken.Value, result.AuthenticationTokenExpirationDate.Value);
            }
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            LogoutResult result = userEntity.Logout(requestContext);

            if (result == LogoutResult.NotLoggedIn)
                return Ok("Logout failed not logged in.");

            return Ok("Logout Successful.");
        }

        private IActionResult AuthenticationSuccess(Guid authenticationToken, DateTime authenticationTokenExpirationDate)
        {
            string epirationDate = $"{DateTime.Now.AddMinutes(2).ToString("ddd, dd MMM yyy HH:mm:ss", new CultureInfo("En-en"))} GMT";

            StringBuilder authenticationCookieContent = new StringBuilder();
            authenticationCookieContent.Append($"{AuthenticationTokenKey}={authenticationToken};");
            //authenticationCookieContent.Append($"Domain=http://DoriansBadgerDen.at;");
            authenticationCookieContent.Append($"expires={authenticationTokenExpirationDate}");

            IActionResult result = Ok("Authentication Successful.");
            result.AddHeaderEntry("Set-Cookie", authenticationCookieContent.ToString());

            return result;
        }

        private const string AuthenticationTokenKey = "AuthToken";
        private readonly RequestContext requestContext;
        private static readonly UserEntity userEntity = new UserEntity();
    }
}
