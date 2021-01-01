using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Models;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using MonsterTradingCardGame.Infrastructure.Extensions;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class CardRepository
    {
        public CardRepository(PostgreSqlDatabase database)
        {
            Assert.NotNull(database, nameof(database));

            this.database = database;
        }

        public bool CardNameExists(string cardName, NpgsqlTransaction transaction = null)
        {
            return database.ExecuteScalar<bool>(@"SELECT EXISTS(
                SELECT ""Card_ID""
                FROM public.""Card""
                WHERE ""Name"" = @cardName);", 
                transaction,
                new NpgsqlParameter("cardName", cardName)
            );
        }

        public int GetCardCount(NpgsqlTransaction transaction = null)
        {
            const string statement = "SELECT COUNT(*) FROM public.\"Card\"";
            return (int)database.ExecuteScalar<long>(statement, transaction);
        }

        public void InsertCard(Card card, NpgsqlTransaction transaction = null)
        {
            Assert.NotNull(card, nameof(card));

            const string statement = @"INSERT INTO public.""Card"" (""ElementType"", ""CardType"", ""Name"",
                ""Description"", ""AttackPoints"")
	            VALUES(@elementType, @cardType, @name, @description, @attackPoints);";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("elementType", card.Element.ToString()),
                new NpgsqlParameter("cardType", card.Type.ToString()),
                new NpgsqlParameter("name", card.Name),
                new NpgsqlParameter("description", card.Description),
                new NpgsqlParameter("attackPoints", card.AttackPoints)
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        public IEnumerable<Card> GetCardsWithoutDeck(int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT ""Card"".""Card_ID"", ""ElementType"", ""CardType"", ""Name"", ""Description"", ""AttackPoints""
                FROM public.""CardLibrary""
                INNER JOIN public.""Card"" ON ""Card"".""Card_ID"" = ""CardLibrary"".""Card_ID""
                WHERE ""User_ID"" = @userID AND ""CardLibrary"".""Card_ID"" NOT IN (SELECT ""Card_ID""
                FROM public.""Deck""
                INNER JOIN public.""Deck_Cards"" ON ""Deck_Cards"".""Deck_ID"" = ""Deck"".""Deck_ID""
                WHERE ""User_ID"" = @userID);";

            Card ReadCardRow(NpgsqlDataReader reader)
            {
                return new Card()
                {
                    CardID = reader.GetValue<int>("Card"),
                    Element = Enum.Parse<ElementType>(reader.GetValue<string>("ElementType")),
                    Type = Enum.Parse<CardType>(reader.GetValue<string>("CardType")),
                    Name = reader.GetValue<string>("Name"),
                    Description = reader.GetValue<string>("Description"),
                    AttackPoints = reader.GetValue<int>("AttackPoints")
                };
            }

            return database.Execute(statement, transaction, ReadCardRow, new NpgsqlParameter("userID", userID));
        }

        private readonly PostgreSqlDatabase database;
    }
}
