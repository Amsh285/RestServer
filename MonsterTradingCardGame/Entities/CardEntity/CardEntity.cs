using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.Infrastructure;

namespace MonsterTradingCardGame.Entities.CardEntity
{
    public sealed class CardEntity
    {
        public void CreateCard(Card value)
        {
            Assert.NotNull(value, nameof(value));
            Validate.NotNullOrWhiteSpace(value.Name, "Der Name darf nicht lerr sein.");

            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                if(cardRepository.CardNameExists(value.Name))
                    throw new UniqueConstraintViolationException(
                       $"Die Karte konnte nicht erstellt werden der Name:{value.Name} ist bereits vergeben."
                   );

                cardRepository.InsertCard(value, transaction);

                transaction.Commit();
            }
        }

        private readonly CardRepository cardRepository = new CardRepository(database);

        private static readonly PostgreSqlDatabase database =
            new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
