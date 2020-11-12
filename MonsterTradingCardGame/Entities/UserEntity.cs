using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MasterTradingCardGame.Repositories;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;

namespace MonsterTradingCardGame.Entities
{
    public sealed class UserEntity
    {
        public bool IsLoggedIn(User user, NpgsqlTransaction transaction = null)
        {
            Assert.NotNull(user, nameof(user));

            UserSession latestUserSession = userSessionRepository.GetLatestUserSession(user.UserID);
            return latestUserSession != null && !latestUserSession.IsExpired;
        }

        public LoginResult Login(string userName, byte[] password)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                AuthenticationResult result = Authenticate(userName, password, transaction);

                //Todo: Handle numberOfFailedLoginAttempts
                if (result == AuthenticationResult.Failed)
                    return LoginResult.LoginFailed(numberOfFailedLoginAttempts: 0);
                else if (result == AuthenticationResult.AlreadyLoggedIn)
                    return LoginResult.AlreadyLoggedIn();
                else
                {
                    Guid authenticationToken = Guid.NewGuid();
                    StoreAuthenticationSessionToken(authenticationToken, userName, transaction);

                    return LoginResult.Success(authenticationToken);
                }
            }
        }

        public AuthenticationResult Authenticate(string userName, byte[] password, NpgsqlTransaction transaction)
        {
            Assert.NotNull(userName, nameof(userName));
            Assert.NotNull(password, nameof(password));

            User match = userRepository.GetUser(userName, transaction);

            if (match == null)
                return AuthenticationResult.Failed;

            if (IsLoggedIn(match))
                return AuthenticationResult.AlreadyLoggedIn;

            byte[] compareHash = SHA256PasswordService.GenerateHash(password, match.Salt);

            if (compareHash.Length != match.Password.Length)
                return AuthenticationResult.Failed;

            for (int i = 0; i < compareHash.Length; i++)
                if (compareHash[i] != match.Password[i])
                    return AuthenticationResult.Failed;

            return AuthenticationResult.Success;
        }

        public void StoreAuthenticationSessionToken(Guid token, string userName, NpgsqlTransaction transaction = null)
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

            userSessionRepository.InsertNew(match.UserID, token, DateTime.Now.AddDays(1), transaction);
        }

        private static readonly UserRepository userRepository = new UserRepository();
        private static readonly UserSessionRepository userSessionRepository = new UserSessionRepository();

        private static readonly PostgreSqlDatabase database =
            new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
