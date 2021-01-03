using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MasterTradingCardGame.Repositories;
using MonsterTradingCardGame.Infrastructure.Authentication;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.Infrastructure;
using System.Collections.Generic;

namespace MonsterTradingCardGame.Entities.PlayerEntity
{
    public sealed class PlayerEntity
    {
        public void OpenFirstBoosterPackage(RequestContext requestContext)
        {
            Assert.NotNull(requestContext, nameof(requestContext));

            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = CookieAuthenticationModule.GetUserSessionFromRequest(requestContext, transaction);
                User user = userRepository.GetUser(session.UserID, transaction);
                Assert.NotNull(user, nameof(user));

                if (!boosterRepository.HasBoosterPackages(user.UserID, transaction))
                    throw new NoBoosterPackageAssignedException($"Cannot Open Booster- package. No Booster- Packages found for User: {user.UserName}.");

                BoosterPack firstBoosterPack = boosterRepository.GetFirstBooserPackage(user.UserID, transaction);
                Assert.NotNull(firstBoosterPack, nameof(firstBoosterPack));

                foreach (BoosterPackCard assignedCard in firstBoosterPack.AssignedCards)
                    cardLibraryRepository.AddCardToLibrary(assignedCard.CardID, firstBoosterPack.UserID);

                boosterRepository.DeleteBooster(firstBoosterPack, transaction);
                transaction.Commit();
            }
        }

        public IEnumerable<CardLibraryItem> GetCardLibrary(RequestContext requestContext)
        {
            Assert.NotNull(requestContext, nameof(requestContext));

            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = CookieAuthenticationModule.GetUserSessionFromRequest(requestContext, transaction);
                return cardLibraryRepository.GetCardLibraryItemsByUserID(session.UserID, transaction);
            }
        }

        public IEnumerable<HighScoreEntry> GetHighScore()
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                return userRepository.GetHighScore(transaction);
            }
        }

        private readonly UserRepository userRepository = new UserRepository();
        private readonly CardLibraryRepository cardLibraryRepository = new CardLibraryRepository(database);
        private readonly BoosterRepository boosterRepository = new BoosterRepository(database);

        private static readonly PostgreSqlDatabase database = new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
