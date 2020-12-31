using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MonsterTradingCardGame.Models;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using MonsterTradingCardGame.Infrastructure.Extensions;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class DeckRepository
    {
        public DeckRepository(PostgreSqlDatabase database)
        {
            Assert.NotNull(database, nameof(database));

            this.database = database;
        }

        public bool Exists(string name, int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT EXISTS(SELECT * FROM public.""Deck"" WHERE ""Name"" = @name AND ""User_ID"" = @userID)";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("name", name),
                new NpgsqlParameter("userID", userID)
            };

            return database.ExecuteScalar<bool>(statement, transaction, parameters);
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

        public Deck GetDeck(User user, int deckID, NpgsqlTransaction transaction = null)
        {
            Assert.NotNull(user, nameof(user));

            const string statement = @"SELECT ""Deck_ID"", ""Name""
                FROM public.""Deck""
                WHERE ""Deck_ID"" = @deckId;";

            Deck ReadDeckRow(NpgsqlDataReader reader)
            {
                return new Deck()
                {
                    Deck_ID = reader.GetValue<int>("Deck_ID"),
                    Name = reader.GetValue<string>("Name"),
                    Player = user
                };
            }

            Deck result = database.Execute(statement, transaction, ReadDeckRow, new NpgsqlParameter("deckId", deckID))
                .FirstOrDefault();

            if (result != null)
                result.Cards = GetCardsFromDeckID(result.Deck_ID, transaction)
                    .ToList();

            return result;
        }

        public Deck GetDeck(User user, string name, NpgsqlTransaction transaction = null)
        {
            Assert.NotNull(user, nameof(user));
            Assert.NotNull(name, nameof(name));

            const string statement = @"SELECT ""Deck_ID"", ""Name""
                FROM public.""Deck""
                WHERE ""User_ID"" = @userID AND ""Name"" = @name;";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("userID", user.UserID),
                new NpgsqlParameter("name", name)
            };

            Deck ReadDeckRow(NpgsqlDataReader reader)
            {
                return new Deck()
                {
                    Deck_ID = reader.GetValue<int>("Deck_ID"),
                    Name = reader.GetValue<string>("Name"),
                    Player = user
                };
            }

            Deck result = database.Execute(statement, transaction, ReadDeckRow, parameters)
                .FirstOrDefault();

            if (result != null)
                result.Cards = GetCardsFromDeckID(result.Deck_ID, transaction)
                    .ToList();

            return result;
        }

        public IEnumerable<Card> GetCardsFromDeckID(int deckID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT ""Card"".""Card_ID"", ""ElementType"", ""CardType"", ""Name"", ""Description"", ""AttackPoints""
                FROM public.""Deck_Cards""
                JOIN public.""Card"" ON ""Card"".""Card_ID"" = ""Deck_Cards"".""Card_ID""
                WHERE ""Deck_ID"" = @deckID";

            Card ReadCardRow(NpgsqlDataReader reader)
            {
                return new Card()
                {
                    CardID = reader.GetValue<int>("Card_ID"),
                    Element = Enum.Parse<ElementType>(reader.GetValue<string>("ElementType")),
                    Type = Enum.Parse<CardType>(reader.GetValue<string>("CardType")),
                    Name = reader.GetValue<string>("Name"),
                    Description = reader.GetValue<string>("Description"),
                    AttackPoints = reader.GetValue<int>("AttackPoints")
                };
            }

            return database.Execute(statement, transaction, ReadCardRow, new NpgsqlParameter("deckID", deckID));
        }

        private readonly PostgreSqlDatabase database;
    }
}
