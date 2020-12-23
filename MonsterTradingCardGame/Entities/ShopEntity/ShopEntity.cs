using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MasterTradingCardGame.Repositories;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;

namespace MonsterTradingCardGame.Entities.ShopEntity
{
    public sealed class ShopEntity
    {
        public void BuyBoosterPackage(Guid sessionToken)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = userSessionRepository.GetUserSessionByToken(sessionToken, transaction);

                if (session == null)
                    throw new NotFoundException($"User Session ID:{sessionToken} could not be found.");

                if (session.IsExpired)
                    throw new SessionExpiredException($"User Session ID:{sessionToken} is expired.");

                User customer = userRepository.GetUser(session.UserID, transaction);
                Assert.NotNull(customer, nameof(customer));

                if (customer.Coins < 4)
                    throw new InsufficientFundsException($"Booster package cannot be bought. Customer: {customer.UserName} doesn´t have enough Coins.");

                int boosterID = boosterRepository.InsertBooster(customer.UserID, transaction);
                int cardCount = cardRepository.GetCardCount(transaction);

                Random rnd = new Random((int)DateTime.Now.Ticks);
                
                for(int i = 0;i < 4;++i)
                {
                    int cardNumber = rnd.Next() % cardCount;


                }
            }
        }

        private readonly CardRepository cardRepository = new CardRepository(database);
        private readonly UserSessionRepository userSessionRepository = new UserSessionRepository(database);
        private readonly UserRepository userRepository = new UserRepository();
        private readonly BoosterRepository boosterRepository = new BoosterRepository(database);

        private static readonly PostgreSqlDatabase database =
            new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
