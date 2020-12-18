using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MasterTradingCardGame.Repositories;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.Infrastructure;
using System;

namespace MonsterTradingCardGame.Entities.UserEntity
{
    public sealed class UserEntity
    {
        public bool IsLoggedIn(User user, NpgsqlTransaction transaction = null)
        {
            Assert.NotNull(user, nameof(user));

            UserSession latestUserSession = userSessionRepository.GetLatestUserSession(user.UserID, transaction);
            return latestUserSession != null && !latestUserSession.IsExpired;
        }

        public LoginResult Login(string userName, byte[] password, RequestContext context)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    UserSession session = GetSessionFromCookie(context, transaction);

                    if (session != null && !session.IsExpired)
                        return LoginResult.AlreadyLoggedIn();

                    AuthenticationResult result = Authenticate(userName, password, transaction);

                    if (result == AuthenticationResult.Failed)
                        return LoginResult.LoginFailed(numberOfFailedLoginAttempts: 0);
                    else
                    {
                        User match = userRepository.GetUser(userName, transaction);
                        UserSession latestUserSession = userSessionRepository.GetLatestUserSession(match.UserID, transaction);

                        Guid authenticationToken = Guid.NewGuid();
                        DateTime authenticationTokenExpirationDate = DateTime.Now.AddDays(1);

                        if(latestUserSession != null && !latestUserSession.IsExpired)
                        {
                            authenticationToken = latestUserSession.Token;
                            authenticationTokenExpirationDate = latestUserSession.ExpirationDate;
                        }
                        else
                            StoreAuthenticationSessionToken(authenticationToken, authenticationTokenExpirationDate, userName, transaction);

                        return LoginResult.Success(authenticationToken, authenticationTokenExpirationDate);
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    transaction.Commit();
                }
            }
        }

        public LogoutResult Logout(RequestContext context)
        {
            Assert.NotNull(context, nameof(context));

            if (!context.Cookies.Exists(ProjectConstants.AuthenticationTokenKey) ||
                string.IsNullOrWhiteSpace(context.Cookies[ProjectConstants.AuthenticationTokenKey]))
                return LogoutResult.NotLoggedIn;

            if (Guid.TryParse(context.Cookies[ProjectConstants.AuthenticationTokenKey], out Guid sessionToken))
            {
                using (NpgsqlConnection connection = database.CreateAndOpenConnection())
                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        UserSession session = userSessionRepository.GetUserSessionByToken(sessionToken, transaction);

                        if (session == null || session.IsExpired)
                            return LogoutResult.NotLoggedIn;

                        userSessionRepository.CancelSession(session, transaction);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            else
                return LogoutResult.InvalidAuthenticationTokenFormat;

            return LogoutResult.Success;
        }

        public UserSession GetSessionFromCookie(RequestContext context, NpgsqlTransaction transaction = null)
        {
            if (!context.Cookies.Exists(ProjectConstants.AuthenticationTokenKey))
                return null;

            string authenticationToken = context.Cookies[ProjectConstants.AuthenticationTokenKey];

            if (Guid.TryParse(authenticationToken, out Guid token))
                return userSessionRepository.GetUserSessionByToken(token, transaction);

            return null;
        }

        public AuthenticationResult Authenticate(string userName, byte[] password, NpgsqlTransaction transaction)
        {
            Assert.NotNull(userName, nameof(userName));
            Assert.NotNull(password, nameof(password));

            User match = userRepository.GetUser(userName, transaction);

            if (match == null)
                return AuthenticationResult.Failed;

            byte[] compareHash = SHA256PasswordService.GenerateHash(password, match.Salt);

            if (compareHash.Length != match.Password.Length)
                return AuthenticationResult.Failed;

            for (int i = 0; i < compareHash.Length; i++)
                if (compareHash[i] != match.Password[i])
                    return AuthenticationResult.Failed;

            return AuthenticationResult.Success;
        }

        public void StoreAuthenticationSessionToken(Guid token, DateTime expirationDate, string userName, NpgsqlTransaction transaction = null)
        {
            Assert.NotNull(userName, nameof(userName));

            User match = userRepository.GetUser(userName, transaction);

            if (match == null)
                throw new NotFoundException($"Unable to find User: {userName}.");

            UserSession latestSession = userSessionRepository.GetLatestUserSession(match.UserID, transaction);

            if (latestSession != null && !latestSession.IsExpired)
                throw new InvalidOperationException(
                    $"New Session cannot be created. Session: Id - {latestSession.UserSessionID}, Guid - {latestSession.Token} is still active."
                );

            userSessionRepository.InsertNew(match.UserID, token, expirationDate, transaction);
        }

        private readonly UserRepository userRepository = new UserRepository();
        private readonly UserSessionRepository userSessionRepository = new UserSessionRepository(database);

        private static readonly PostgreSqlDatabase database =
            new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
