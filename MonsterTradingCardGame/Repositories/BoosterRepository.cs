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
	            VALUES(@userID, 0) RETURNING BoosterPack_ID;";

            return database.ExecuteScalar<int>(statement, transaction, new NpgsqlParameter("userID", userID));
        }

        private readonly PostgreSqlDatabase database;
    }
}
