using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Modules;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class BattleLogRepository
    {
        public BattleLogRepository(PostgreSqlDatabase database)
        {
            Assert.NotNull(database, nameof(database));

            this.database = database;
        }

        public int InsertBattleLog(InsertBattleLogModel battleLog, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""BattleLog""(
                ""Match_ID"", ""Deck_ID_1"", ""Deck_ID_2"", ""User_ID_Winner"", ""BattleResult"", ""Turns"", ""CreationDate"")
	            VALUES(@matchID, @deckID1, @deckID2, @userIDWinner, @battleResult, @turns, @creationDate)
                RETURNING ""BattleLog_ID"";";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("matchID", battleLog.Match_ID),
                new NpgsqlParameter("deckID1", battleLog.Deck1.Deck_ID),
                new NpgsqlParameter("deckID2", battleLog.Deck2.Deck_ID),
                new NpgsqlParameter("battleResult", Enum.GetName(typeof(AutomaticDuelResult), battleLog.BattleResult)),
                new NpgsqlParameter("turns", battleLog.Turns),
                new NpgsqlParameter("creationDate", battleLog.CreationDate)
            };

            if (battleLog.Winner == null)
                new NpgsqlParameter("userIDWinner", DBNull.Value);
            else
                new NpgsqlParameter("userIDWinner", battleLog.Winner.UserID);

            return database.ExecuteScalar<int>(statement, transaction, parameters);
        }

        public void InsertBattleLogEntry(int battleLogID, InsertBattleLogEntryModel battleLogEntry, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""BattleLogEntry""(
                ""BattleLog_ID"", ""Card_ID_1"", ""Card_ID_2"", ""DeckState_1"", ""DeckState_2"", ""RoundDescription"", ""Order"")
	            VALUES(@battleLogID, @cardID1, @cardID2, @deckState1, @deckState2, @roundDescription, @order);";

            NpgsqlParameter[] parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("battleLogID", battleLogID),
                new NpgsqlParameter("cardID1", battleLogEntry.Card1.CardID),
                new NpgsqlParameter("cardID2", battleLogEntry.Card2.CardID),
                new NpgsqlParameter("deckState1", battleLogEntry.DeckState1),
                new NpgsqlParameter("deckState2", battleLogEntry.DeckState2),
                new NpgsqlParameter("roundDescription", battleLogEntry.RoundDescription),
                new NpgsqlParameter("order", battleLogEntry.Order),
            };

            database.ExecuteNonQuery(statement, transaction, parameters);
        }

        private readonly PostgreSqlDatabase database;
    }
}
