using MonsterTradingCardGame.Entities.UserEntity;
using MonsterTradingCardGame.Infrastructure;
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
            // Todo: neu einloggen wenn kein cookie gesendet wird aber in der datenbank ein nicht abgelaufenes existiert.
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
                return Unauthorized("Logout failed not logged in.");
            else if (result == LogoutResult.InvalidAuthenticationTokenFormat)
                return BadRequest("Logout failed, invalid Authenticationtoken- Format.");

            IActionResult actionResult = Ok("Logout Successful.");
            SetCookie(actionResult, ProjectConstants.AuthenticationTokenKey, string.Empty, null, DateTime.Now);
            return actionResult;
        }

        private IActionResult AuthenticationSuccess(Guid authenticationToken, DateTime authenticationTokenExpirationDate)
        {
            IActionResult result = Ok("Authentication Successful.");
            AddAuthenticationCookie(result, authenticationToken, authenticationTokenExpirationDate);

            return result;
        }

        private static void AddAuthenticationCookie(IActionResult result, Guid authenticationToken, DateTime authenticationTokenExpirationDate)
        {
            SetCookie(result, ProjectConstants.AuthenticationTokenKey, authenticationToken.ToString(), null, authenticationTokenExpirationDate);
        }

        private static void SetCookie(IActionResult result, string token, string value, string path, DateTime expirationDate)
        {
            string formattedExpirationDate = $"{expirationDate.ToString("ddd, dd MMM yyy HH:mm:ss", new CultureInfo("En-en"))} GMT";

            StringBuilder cookieContent = new StringBuilder();
            cookieContent.Append($"{token}={value}; ");

            if(string.IsNullOrWhiteSpace(path))
                cookieContent.Append("Path=/; ");
            else
                cookieContent.Append($"Path={path}; ");

            cookieContent.Append($"Expires={formattedExpirationDate}");
            result.AddHeaderEntry("Set-Cookie", cookieContent.ToString());
        }

        private readonly RequestContext requestContext;
        private static readonly UserEntity userEntity = new UserEntity();
    }
}
