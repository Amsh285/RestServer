using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Models;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using MonsterTradingCardGame.Infrastructure.Extensions;
using System.Linq;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class TradeRepository
    {
        public TradeRepository(PostgreSqlDatabase database)
        {
            Assert.NotNull(database, nameof(database));

            this.database = database;
        }

        public bool TradeUIDExists(Guid tradeID, NpgsqlTransaction transaction)
        {
            const string statement = @"SELECT EXISTS(SELECT ""Trade_ID"" FROM public.""Trade"" WHERE ""Trade_ID"" = @tradeID);";

            return database.ExecuteScalar<bool>(statement, transaction, new NpgsqlParameter("tradeID", tradeID));
        }

        public bool TradeOfferUIDExists(Guid tradeOfferID, NpgsqlTransaction transaction)
        {
            const string statement = @"SELECT EXISTS(SELECT ""Trade_Offer_ID"" FROM public.""Trade_Offer"" WHERE ""Trade_Offer_ID"" = @tradeOfferID);";

            return database.ExecuteScalar<bool>(statement, transaction, new NpgsqlParameter("tradeOfferID", tradeOfferID));
        }

        public void InsertTrade(Guid tradeID, int userID, int cardID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""Trade""(
                ""Trade_ID"", ""User_ID"", ""Card_ID"", ""CreationDate"")
	            VALUES(@tradeID, @userID, @cardID, @creationDate); ";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("tradeID", tradeID),
                new NpgsqlParameter("userID", userID),
                new NpgsqlParameter("cardID", cardID),
                new NpgsqlParameter("creationDate", DateTime.Now),
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        public void InsertTradeOffer(Guid tradeOfferID, Guid tradeID, int userID, int offerCardID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""Trade_Offer""(
                ""Trade_Offer_ID"", ""Trade_ID"", ""User_ID"", ""Card_ID"", ""CreationDate"")
                VALUES(@tradeOfferID, @tradeID, @userID, @offerCardID, @creationDate); ";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("tradeOfferID", tradeOfferID),
                new NpgsqlParameter("tradeID", tradeID),
                new NpgsqlParameter("userID", userID),
                new NpgsqlParameter("offerCardID", offerCardID),
                new NpgsqlParameter("creationDate", DateTime.Now),
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        public void DeleteTrade(Guid tradeID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"DELETE FROM public.""Trade""
                WHERE ""Trade_ID"" = @tradeID;";

            database.ExecuteNonQuery(statement, transaction, new NpgsqlParameter("tradeID", tradeID));
        }

        public void DeleteTradeOffer(Guid tradeOfferId, NpgsqlTransaction transaction = null)
        {
            const string statement = @"DELETE FROM public.""Trade_Offer""
                WHERE ""Trade_Offer_ID"" = @tradeOfferId;";

            database.ExecuteNonQuery(statement, transaction, new NpgsqlParameter("tradeOfferId", tradeOfferId));
        }

        public Trade GetTrade(Guid tradeID, NpgsqlTransaction transaction = null)
        {
            return GetTradesWhere("\"Trade_ID\" = @tradeID", transaction, new NpgsqlParameter("tradeID", tradeID))
                .FirstOrDefault();
        }

        private IEnumerable<Trade> GetTradesWhere(string condition, NpgsqlTransaction transaction, params NpgsqlParameter[] parameters)
        {
            const string statement = @"SELECT ""Trade_ID"", ""User_ID"", ""Card_ID"", ""CreationDate""
                FROM public.""Trade""";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(statement);

            if (!string.IsNullOrWhiteSpace(condition))
                builder.AppendLine($"WHERE {condition}");

            return database.Execute(statement, transaction, ReadTradeRow, parameters);
        }

        public TradeOffer GetTradeOffer(Guid tradeOfferID, NpgsqlTransaction transaction = null)
        {
            return GetTradeOffersWhere("\"Trade_Offer_ID\" = @tradeOfferID", transaction, new NpgsqlParameter("tradeOfferID", tradeOfferID))
                .FirstOrDefault();
        }

        private IEnumerable<TradeOffer> GetTradeOffersWhere(string condition, NpgsqlTransaction transaction, params NpgsqlParameter[] parameters)
        {
            const string statement = @"SELECT ""Trade_Offer_ID"", ""Trade_ID"", ""User_ID"", ""Card_ID"", ""CreationDate""
                FROM public.""Trade_Offer""";
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(statement);

            if (!string.IsNullOrWhiteSpace(condition))
                builder.AppendLine($"WHERE {condition}");

            return database.Execute(statement, transaction, ReadTradeOfferRow, parameters);
        }

        private static TradeOffer ReadTradeOfferRow(NpgsqlDataReader reader)
        {
            return new TradeOffer()
            {
                Trade_Offer_ID = reader.GetValue<Guid>("Trade_Offer_ID"),
                Trade_ID = reader.GetValue<Guid>("Trade_ID"),
                User_ID = reader.GetValue<int>("User_ID"),
                Card_ID = reader.GetValue<int>("Card_ID"),
                CreationDate = reader.GetValue<DateTime>("CreationDate")
            };
        }

        private static Trade ReadTradeRow(NpgsqlDataReader reader)
        {
            return new Trade()
            {
                Trade_ID = reader.GetValue<Guid>("Trade_ID"),
                User_ID = reader.GetValue<int>("User_ID"),
                Card_ID = reader.GetValue<int>("Card_ID"),
                CreationDate = reader.GetValue<DateTime>("CreationDate")
            };
        }

        private readonly PostgreSqlDatabase database;
    }
}
