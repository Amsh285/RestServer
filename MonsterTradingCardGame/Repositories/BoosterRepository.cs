using MasterTradingCardGame.Database;
using Npgsql;
using RestServer.WebServer.Infrastructure;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class BoosterRepository
    {
        public BoosterRepository(PostgreSqlDatabase database)
        {
            Assert.NotNull(database, nameof(database));

            this.database = database;
        }

        public int InsertBooster(int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""BoosterPack"" (""User_ID"", ""CardCount"")
	            VALUES(@userID, 0) RETURNING ""BoosterPack_ID"";";

            return database.ExecuteScalar<int>(statement, transaction, new NpgsqlParameter("userID", userID));
        }

        public int GetCardIDFromCardNumber(int cardNumber, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT ""Card_ID"", cardNumber 
                FROM(SELECT ""Card_ID"", ROW_NUMBER() OVER() AS cardNumber FROM public.""Card"") AS rowNumberIDMapping
                WHERE cardNumber = @cardNumber;";

            return database.ExecuteScalar<int>(statement, transaction, new NpgsqlParameter("cardNumber", cardNumber));
        }

        public void AssignCardToBooser(int cardID, int BoosterPackID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""BoosterPack_Cards""(""BoosterPack_ID"", ""Card_ID"")
                VALUES(@boosterPackID, @cardID);

                UPDATE public.""BoosterPack""
                SET ""CardCount"" = ""CardCount"" + 1
                WHERE ""BoosterPack_ID"" = @boosterPackID;";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("boosterPackID", BoosterPackID),
                new NpgsqlParameter("cardID", cardID)
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        private readonly PostgreSqlDatabase database;
    }
}
