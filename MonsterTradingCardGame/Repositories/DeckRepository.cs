using MasterTradingCardGame.Database;
using Npgsql;
using RestServer.WebServer.Infrastructure;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class DeckRepository
    {
        public DeckRepository(PostgreSqlDatabase database)
        {
            Assert.NotNull(database, nameof(database));

            this.database = database;
        }

        public int InsertDeck(int userID, string deckName, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""Deck""(""User_ID"", ""Name"")
                VALUES(@userID, @name) RETURNING ""Deck_ID"";";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("userID", userID),
                new NpgsqlParameter("name", deckName)
            };

            return database.ExecuteScalar<int>(statement, transaction, parameters);
        }

        public void AssignCardToDeck(int deckID, int cardID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""Deck_Cards""(
                ""Deck_ID"", ""Card_ID"")
                VALUES(@deckID, @cardID);";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("deckID", deckID),
                new NpgsqlParameter("cardID", cardID)
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        private readonly PostgreSqlDatabase database;
    }
}
