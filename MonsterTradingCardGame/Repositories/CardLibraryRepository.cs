using MasterTradingCardGame.Database;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class CardLibraryRepository
    {
        public CardLibraryRepository(PostgreSqlDatabase database)
        {
            Assert.NotNull(database, nameof(database));

            this.database = database;
        }

        public bool CardExists(int cardID, int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT EXISTS(SELECT * FROM public.""CardLibrary"" WHERE ""User_ID"" = @userID AND ""Card_ID"" = @cardID );";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("userID", userID),
                new NpgsqlParameter("cardID", cardID)
            };

            return database.ExecuteScalar<bool>(statement, transaction, parameters);
        }

        public void AddCardToLibrary(int cardID, int userID, NpgsqlTransaction transaction = null)
        {
            if (CardExists(cardID, userID, transaction))
                IncrementQuantity(cardID, userID, transaction);
            else
                InsertCard(cardID, userID, transaction);
        }

        private void IncrementQuantity(int cardID, int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"UPDATE public.""CardLibrary""
                SET ""Quantity"" = ""Quantity""  + 1
                WHERE ""User_ID"" = @userID AND ""Card_ID"" = @cardID;";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("userID", userID),
                new NpgsqlParameter("cardID", cardID)
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        private void InsertCard(int cardID, int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""CardLibrary""(
                ""User_ID"", ""Card_ID"", ""Quantity"")
                VALUES(@userID, @cardId, 1);";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("userID", userID),
                new NpgsqlParameter("cardID", cardID)
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        private readonly PostgreSqlDatabase database;
    }
}
