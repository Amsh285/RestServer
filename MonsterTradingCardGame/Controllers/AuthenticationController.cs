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
            IActionResult result = Ok("Authentication Successful.");
            AddAuthenticationCookie(result, authenticationToken, authenticationTokenExpirationDate);
            //AddTestCookie(result, DateTime.Now.AddDays(1));

            return result;
        }

        private static void AddAuthenticationCookie(IActionResult result, Guid authenticationToken, DateTime authenticationTokenExpirationDate)
        {
            string epirationDate = $"{authenticationTokenExpirationDate.ToString("ddd, dd MMM yyy HH:mm:ss", new CultureInfo("En-en"))} GMT";

            StringBuilder authenticationCookieContent = new StringBuilder();
            authenticationCookieContent.Append($"{AuthenticationTokenKey}={authenticationToken};");
            //authenticationCookieContent.Append($"Domain=http://DoriansBadgerDen.at;");
            authenticationCookieContent.Append($"expires={epirationDate}");
            result.AddHeaderEntry("Set-Cookie", authenticationCookieContent.ToString());
        }

        private static void AddTestCookie(IActionResult result, DateTime expirationDate)
        {
            string epirationDateString = $"{expirationDate.ToString("ddd, dd MMM yyy HH:mm:ss", new CultureInfo("En-en"))} GMT";

            StringBuilder cookieContent = new StringBuilder();
            cookieContent.Append($"Foo=Bar;");
            //authenticationCookieContent.Append($"Domain=http://DoriansBadgerDen.at;");
            cookieContent.Append($"expires={epirationDateString}");
            result.AddHeaderEntry("Set-Cookie", cookieContent.ToString());
        }

        private const string AuthenticationTokenKey = "AuthToken";
        private readonly RequestContext requestContext;
        private static readonly UserEntity userEntity = new UserEntity();
    }
}
