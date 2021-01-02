using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Infrastructure.Extensions;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

        public bool CardIsAvailableForTrading(int cardID, int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT (SELECT COALESCE(MAX(c), 0) FROM
                (SELECT COUNT(*) AS c
                FROM public.""Deck""
                JOIN public.""Deck_Cards"" ON ""Deck_Cards"".""Deck_ID"" = ""Deck"".""Deck_ID""
                WHERE ""Deck"".""User_ID"" = @userID AND ""Deck_Cards"".""Card_ID"" = @cardID
                GROUP BY ""Deck_Cards"".""Deck_ID"") AS asd) <

                (SELECT COALESCE(SUM(""Quantity""), 0)
                FROM public.""CardLibrary""
                WHERE ""CardLibrary"".""User_ID"" = @userID AND ""CardLibrary"".""Card_ID"" = @cardID);";

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

        public void RemoveCardFromLibrary(CardLibraryItem card, NpgsqlTransaction transaction = null)
        {
            if (card.Quantity > 1)
            {
                DecrementQuantity(card.Card_ID, card.User_ID, transaction);
                --card.Quantity;
            }
            else
                DeleteCard(card.Card_ID, card.User_ID, transaction);
        }

        private void IncrementQuantity(int cardID, int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"UPDATE public.""CardLibrary""
                SET ""Quantity"" = ""Quantity"" + 1
                WHERE ""User_ID"" = @userID AND ""Card_ID"" = @cardID;";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("userID", userID),
                new NpgsqlParameter("cardID", cardID)
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        private void DecrementQuantity(int cardID, int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"UPDATE public.""CardLibrary""
                SET ""Quantity"" = ""Quantity"" - 1
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

        private void DeleteCard(int cardID, int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"DELETE FROM public.""CardLibrary""
                WHERE ""Card_ID"" = @cardID AND ""User_ID"" = @userID;";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("userID", userID),
                new NpgsqlParameter("cardID", cardID)
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        public CardLibraryItem GetCardLibraryItem(int cardID, int userID, NpgsqlTransaction transaction = null)
        {
            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("cardID", cardID),
                new NpgsqlParameter("userID", userID)
            };

            return GetCardLibraryItemsWhere("\"Card\".\"Card_ID\" = @cardID AND \"User_ID\" = @userId", transaction, parameters)
                .FirstOrDefault();
        }

        public IEnumerable<CardLibraryItem> GetCardLibraryItemsByUserID(int userID, NpgsqlTransaction transaction = null)
        {
            return GetCardLibraryItemsWhere("\"User_ID\" = @userID", transaction, new NpgsqlParameter("userID", userID));
        }

        private IEnumerable<CardLibraryItem> GetCardLibraryItemsWhere(string whereCondition,
            NpgsqlTransaction transaction, params NpgsqlParameter[] parameters)
        {
            const string statement = @"SELECT ""CardLibrary_ID"", ""User_ID"", ""CardLibrary"".""Card_ID"", ""Name"", ""Description"",
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
                Card_ID = reader.GetValue<int>("Card_ID"),
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
