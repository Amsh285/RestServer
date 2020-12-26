using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MasterTradingCardGame.Repositories;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterTradingCardGame.Entities.PlayerEntity
{
    public sealed class PlayerEntity
    {
        public void OpenFirstBoosterPackage(Guid sessionToken)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                //Todo: remove duplicate Code
                UserSession session = userSessionRepository.GetUserSessionByToken(sessionToken, transaction);

                if (session == null)
                    throw new NotFoundException($"User Session ID:{sessionToken} could not be found.");

                if (session.IsExpired)
                    throw new SessionExpiredException($"User Session ID:{sessionToken} is expired.");

                User user = userRepository.GetUser(session.UserID, transaction);
                Assert.NotNull(user, nameof(user));

                BoosterPack firstBoosterPack = boosterRepository.GetFirstBooserpackage(user.UserID, transaction);
                Assert.NotNull(firstBoosterPack, nameof(firstBoosterPack));

                foreach (BoosterPackCard assignedCard in firstBoosterPack.AssignedCards)
                    cardLibraryRepository.AddCardToLibrary(assignedCard.CardID, firstBoosterPack.UserID);

                boosterRepository.DeleteBooster(firstBoosterPack, transaction);
                transaction.Commit();
            }
        }

        private readonly UserRepository userRepository = new UserRepository();
        private readonly UserSessionRepository userSessionRepository = new UserSessionRepository(database);
        private readonly CardLibraryRepository cardLibraryRepository = new CardLibraryRepository(database);
        private readonly BoosterRepository boosterRepository = new BoosterRepository(database);

        private static readonly PostgreSqlDatabase database = new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
