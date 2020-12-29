using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Infrastructure.Extensions;
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

        public IEnumerable<CardLibraryItem> GetCardLibraryItemsByUserID(int userID, NpgsqlTransaction transaction = null)
        {
            return GetCardLibraryItemsWhere("\"User_ID\" = @userID", transaction, new NpgsqlParameter("userID", userID));
        }

        private IEnumerable<CardLibraryItem> GetCardLibraryItemsWhere(string whereCondition,
            NpgsqlTransaction transaction, params NpgsqlParameter[] parameters)
        {
            const string statement = @"SELECT ""CardLibrary_ID"", ""User_ID"", ""Name"", ""Description"",
                ""ElementType"", ""CardType"", ""AttackPoints"", ""Quantity""
                FROM public.""CardLibrary""
                JOIN public.""Card"" ON ""Card"".""Card_ID"" = ""CardLibrary"".""Card_ID""";

            StringBuilder sql = new StringBuilder();
            sql.AppendLine(statement);

            if (!string.IsNullOrWhiteSpace(whereCondition))
            {
                sql.Append("WHERE ");
                sql.AppendLine(whereCondition);
            }

            return database.Execute(sql.ToString(), transaction, ReadCardLibraryRow, parameters);
        }

        private CardLibraryItem ReadCardLibraryRow(NpgsqlDataReader reader)
        {
            return new CardLibraryItem()
            {
                CardLibrary_ID = reader.GetValue<int>("CardLibrary_ID"),
                User_ID = reader.GetValue<int>("User_ID"),
                Name = reader.GetValue<string>("Name"),
                Description = reader.GetValue<string>("Description"),
                Element = Enum.Parse<ElementType>(reader.GetValue<string>("ElementType")),
                Type = Enum.Parse<CardType>(reader.GetValue<string>("CardType")),
                AttackPoints = reader.GetValue<int>("AttackPoints"),
                Quantity = reader.GetValue<int>("Quantity")
            };
        }

        private readonly PostgreSqlDatabase database;
    }
}
