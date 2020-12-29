using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Models;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using MonsterTradingCardGame.Infrastructure.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class BoosterRepository
    {
        public BoosterRepository(PostgreSqlDatabase database)
        {
            Assert.NotNull(database, nameof(database));

            this.database = database;
        }

        public bool HasBoosterPackages(int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT EXISTS(SELECT * FROM public.""BoosterPack""
                WHERE ""User_ID"" = @userID);";

            return database.ExecuteScalar<bool>(statement, transaction, new NpgsqlParameter("userID", userID));
        }

        public IEnumerable<BoosterPackCard> GetAssignedCards(int boosterPackID, NpgsqlTransaction transaction)
        {
            const string statement = @"SELECT ""BoosterPack_Cards_ID"", ""BoosterPack_ID"", ""Card_ID""
                FROM public.""BoosterPack_Cards""
                WHERE ""BoosterPack_ID"" = @boosterPackID;";

            BoosterPackCard ReadBoosterPackCard(NpgsqlDataReader reader)
            {
                return new BoosterPackCard()
                {
                    BoosterPackCardID = reader.GetValue<int>("BoosterPack_Cards_ID"),
                    BoosterPackID = reader.GetValue<int>("BoosterPack_ID"),
                    CardID = reader.GetValue<int>("Card_ID")
                };
            };

            return database.Execute(statement, transaction, ReadBoosterPackCard, new NpgsqlParameter("boosterPackID", boosterPackID));
        }

        public BoosterPack GetFirstBooserPackage(int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT ""BoosterPack_ID"", ""User_ID"", ""CardCount""
                FROM public.""BoosterPack""
                WHERE ""User_ID"" = @userID
                LIMIT 1;";

            BoosterPack ReadBoosterPackRow(NpgsqlDataReader reader)
            {
                return new BoosterPack()
                {
                    BoosterPackID = reader.GetValue<int>("BoosterPack_ID"),
                    UserID = reader.GetValue<int>("User_ID"),
                    CardCount = reader.GetValue<int>("CardCount")
                };
            };

            BoosterPack result = database.Execute(statement, transaction, ReadBoosterPackRow, new NpgsqlParameter("userID", userID))
                .First();

            List<BoosterPackCard> assignedCards = GetAssignedCards(result.BoosterPackID, transaction)
                .ToList();

            result.AssignedCards = assignedCards;
            return result;
        }

        public int InsertBooster(int userID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""BoosterPack"" (""User_ID"", ""CardCount"")
	            VALUES(@userID, 0) RETURNING ""BoosterPack_ID"";";

            return database.ExecuteScalar<int>(statement, transaction, new NpgsqlParameter("userID", userID));
        }

        public void DeleteBooster(BoosterPack booster, NpgsqlTransaction transaction = null)
        {
            const string statement = @"DELETE FROM public.""BoosterPack_Cards"" WHERE ""BoosterPack_ID"" = @boosterPackID;
                DELETE FROM public.""BoosterPack"" WHERE ""BoosterPack_ID"" = @boosterPackID;";

            database.ExecuteNonQuery(statement, transaction, new NpgsqlParameter("boosterPackID", booster.BoosterPackID));
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
