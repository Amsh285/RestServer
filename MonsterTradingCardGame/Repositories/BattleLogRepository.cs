using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Modules;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using MonsterTradingCardGame.Infrastructure.Extensions;
using System.Linq;
using MasterTradingCardGame.Repositories;
using MasterTradingCardGame.Models;

namespace MonsterTradingCardGame.Repositories
{
    public sealed class BattleLogRepository
    {
        public BattleLogRepository(PostgreSqlDatabase database)
        {
            Assert.NotNull(database, nameof(database));

            this.database = database;

            deckRepository = new DeckRepository(database);
            userRepository = new UserRepository();
        }

        public int InsertBattleLog(InsertBattleLogModel battleLog, NpgsqlTransaction transaction = null)
        {
            const string statement = @"INSERT INTO public.""BattleLog""(
                ""Match_ID"", ""Deck_ID_1"", ""Deck_ID_2"", ""User_ID_Winner"", ""BattleResult"", ""Turns"", ""CreationDate"")
	            VALUES(@matchID, @deckID1, @deckID2, @userIDWinner, @battleResult, @turns, @creationDate)
                RETURNING ""BattleLog_ID"";";

            List<NpgsqlParameter> parameters = new List<NpgsqlParameter>()
            {
                new NpgsqlParameter("matchID", battleLog.Match_ID),
                new NpgsqlParameter("deckID1", battleLog.Deck1.Deck_ID),
                new NpgsqlParameter("deckID2", battleLog.Deck2.Deck_ID),
                new NpgsqlParameter("battleResult", Enum.GetName(typeof(AutomaticDuelResult), battleLog.BattleResult)),
                new NpgsqlParameter("turns", battleLog.Turns),
                new NpgsqlParameter("creationDate", battleLog.CreationDate)
            };

            if (battleLog.Winner == null)
                parameters.Add(new NpgsqlParameter("userIDWinner", DBNull.Value));
            else
                parameters.Add(new NpgsqlParameter("userIDWinner", battleLog.Winner.UserID));

            return database.ExecuteScalar<int>(statement, transaction, parameters.ToArray());
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

        public BattleLog GetBattleLogFromMatchID(Guid matchID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT ""BattleLog_ID"", ""Match_ID"", ""Deck_ID_1"",
                ""Deck_ID_2"", ""User_ID_Winner"", ""BattleResult"", ""Turns"", ""CreationDate""
                FROM public.""BattleLog""
                WHERE ""Match_ID"" = @matchID;";

            (BattleLog Log, int deckID1, int deckID2, int? winnerID) ReadBattleLogRow(NpgsqlDataReader reader)
            {
                BattleLog log = new BattleLog()
                {
                    BattleLog_ID = reader.GetValue<int>("BattleLog_ID"),
                    Match_ID = reader.GetValue<Guid>("Match_ID"),
                    BattleResult = Enum.Parse<AutomaticDuelResult>(reader.GetValue<string>("BattleResult")),
                    Turns = reader.GetValue<int>("Turns"),
                    CreationDate = reader.GetValue<DateTime>("CreationDate")
                };

                int? winnerID = null;
                int index = reader.GetOrdinal("User_ID_Winner");
                object winnerColumnValue = reader.GetValue(index);

                if (winnerColumnValue != DBNull.Value)
                    winnerID = (int)winnerColumnValue;

                return (log, reader.GetValue<int>("Deck_ID_1"), reader.GetValue<int>("Deck_ID_2"), winnerID);
            }

            (BattleLog Log, int deckID1, int deckID2, int? winnerID) result = database.Execute(statement, transaction, ReadBattleLogRow, new NpgsqlParameter("matchID", matchID))
                .FirstOrDefault();

            BattleLog log = result.Log;

            if (log != null)
            {
                User user1 = userRepository.GetUserFromDeckID(result.deckID1, transaction);
                Deck deck1 = deckRepository.GetDeck(user1, result.deckID1, transaction);
                log.Deck1 = deck1;

                User user2 = userRepository.GetUserFromDeckID(result.deckID2, transaction);
                Deck deck2 = deckRepository.GetDeck(user2, result.deckID2, transaction);
                log.Deck2 = deck2;

                if (result.winnerID.HasValue)
                {
                    if (user1.UserID == result.winnerID)
                        log.Winner = user1;
                    else
                        log.Winner = user2;
                }

                log.LogEntries = GetBattleLogEntries(log.BattleLog_ID, transaction)
                    .ToArray();
            }

            return log;
        }

        public IEnumerable<BattleLogEntry> GetBattleLogEntries(int battleLogID, NpgsqlTransaction transaction = null)
        {
            const string statement = @"SELECT ""BattleLogEntry_ID"", ""BattleLog_ID"", ""Card_ID_1"", ""Card_ID_2"", 
                ""DeckState_1"", ""DeckState_2"", ""RoundDescription"", ""Order"",

                Card1.""Card_ID"" AS Card1_Card_ID, Card1.""ElementType"" AS Card1_ElementType, Card1.""CardType"" AS Card1_CardType,
                Card1.""Name"" AS Card1_Name, Card1.""AttackPoints"" AS Card1_AttackPoints,

                Card2.""Card_ID"" AS Card2_Card_ID, Card2.""ElementType"" AS Card2_ElementType, Card2.""CardType"" AS Card2_CardType,
                Card2.""Name"" AS Card2_Name, Card2.""AttackPoints"" AS Card2_AttackPoints

                FROM public.""BattleLogEntry""
                JOIN public.""Card"" Card1 ON Card1.""Card_ID"" = ""BattleLogEntry"".""Card_ID_1""
                JOIN public.""Card"" Card2 ON Card2.""Card_ID"" = ""BattleLogEntry"".""Card_ID_2""
                WHERE ""BattleLog_ID"" = @battleLogID
                ORDER BY ""Order"";";

            BattleLogEntry ReadBattleLogEntryRow(NpgsqlDataReader reader)
            {
                BattleLogCard card1 = new BattleLogCard()
                {
                    CardID = reader.GetValue<int>("Card1_Card_ID"),
                    Element = Enum.Parse<ElementType>(reader.GetValue<string>("Card1_ElementType")),
                    Type = Enum.Parse<CardType>(reader.GetValue<string>("Card1_CardType")),
                    Name = reader.GetValue<string>("Card1_Name"),
                    AttackPoints = reader.GetValue<int>("Card1_AttackPoints")
                };

                BattleLogCard card2 = new BattleLogCard()
                {
                    CardID = reader.GetValue<int>("Card2_Card_ID"),
                    Element = Enum.Parse<ElementType>(reader.GetValue<string>("Card2_ElementType")),
                    Type = Enum.Parse<CardType>(reader.GetValue<string>("Card2_CardType")),
                    Name = reader.GetValue<string>("Card2_Name"),
                    AttackPoints = reader.GetValue<int>("Card2_AttackPoints")
                };

                return new BattleLogEntry()
                {
                    BattleLogEntry_ID = reader.GetValue<int>("BattleLogEntry_ID"),
                    BattleLog_ID = reader.GetValue<int>("BattleLog_ID"),
                    Card1 = card1,
                    Card2 = card2,
                    DeckState1 = reader.GetValue<string>("DeckState_1"),
                    DeckState2 = reader.GetValue<string>("DeckState_2"),
                    RoundDescription = reader.GetValue<string>("RoundDescription"),
                    Order = reader.GetValue<int>("Order")
                };
            }

            return database.Execute(statement, transaction, ReadBattleLogEntryRow, new NpgsqlParameter("battleLogID", battleLogID));
        }

        private readonly UserRepository userRepository;
        private readonly DeckRepository deckRepository;

        private readonly PostgreSqlDatabase database;
    }
}
