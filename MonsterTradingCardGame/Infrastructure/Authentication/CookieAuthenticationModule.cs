using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.Infrastructure;
using System;

namespace MonsterTradingCardGame.Infrastructure.Authentication
{
    public static class CookieAuthenticationModule
    {
        public static UserSession GetUserSessionFromRequest(RequestContext context, NpgsqlTransaction transaction = null)
        {
            Assert.NotNull(context, nameof(context));

            if (!context.Cookies.Exists(ProjectConstants.AuthenticationTokenKey))
                throw new SessionTokenNotFoundException("Authentication- Cookie could not be found.");

            string cookieValue = context.Cookies[ProjectConstants.AuthenticationTokenKey];

            if (Guid.TryParse(cookieValue, out Guid sessionToken))
            {
                return GetValidUserSession(sessionToken, transaction);
            }
            else
                throw new InvalidSessionTokenFormatException($"Invalid Sessiontoken- Format: {cookieValue}");
        }

        private static UserSession GetValidUserSession(Guid sessionToken, NpgsqlTransaction transaction)
        {
            UserSession session = userSessionRepository.GetUserSessionByToken(sessionToken, transaction);

            if (session == null)
                throw new SessionTokenNotFoundException($"User Session ID:{sessionToken} could not be found in the Database.");

            if (session.IsExpired)
                throw new SessionExpiredException($"User Session ID:{sessionToken} is expired.");

            return session;
        }

        private static readonly UserSessionRepository userSessionRepository = new UserSessionRepository(database);

        private static readonly PostgreSqlDatabase database = new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
