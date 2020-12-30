using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MonsterTradingCardGame.Modules
{
    public sealed class AutomaticDuelModule
    {
        public void ExecuteDuel(MatchmakingEntry match, int maxNumberOfRounds)
        {
            Assert.NotNull(match, nameof(match));
            Assert.NotNull(match.Adversary, "match.Adversary");
            Assert.NotNull(match.AdversaryRequestedDeck, "match.AdversaryRequestedDeck");
            Assert.NotNull(match.Self, "match.Self");
            Assert.NotNull(match.SelfRequestedDeck, "match.SelfRequestedDeck");

            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                User winner = null;
                List<InsertBattleLogEntryModel> battleLogEntries = new List<InsertBattleLogEntryModel>();

                Deck deck1 = deckRepository.GetDeck(match.Self, match.SelfRequestedDeck, transaction);
                Deck deck2 = deckRepository.GetDeck(match.Adversary, match.AdversaryRequestedDeck, transaction);

                Assert.That(deck1.Cards.Count > 0, $"deck1 CardCount must be larger than 0.");
                Assert.That(deck2.Cards.Count > 0, $"deck2 CardCount must be larger than 0.");
                Assert.That(deck1.Cards.Count == deck2.Cards.Count, $"deck1 CardCount must be equal to deck2 CardCount.");

                int currentRound;

                for (currentRound = 0; currentRound < maxNumberOfRounds; currentRound++)
                {
                    if (deck1.Cards.Count == 0)
                    {
                        winner = deck2.Player;
                        break;
                    }
                    else if (deck2.Cards.Count == 0)
                    {
                        winner = deck1.Player;
                        break;
                    }

                    int cardNumber_Deck1 = randomNumberGenerator.Next() % deck1.Cards.Count;
                    int cardNumber_Deck2 = randomNumberGenerator.Next() % deck2.Cards.Count;

                    Card card1 = deck1.Cards[cardNumber_Deck1];
                    Card card2 = deck2.Cards[cardNumber_Deck2];

                    int effectiveAttackPointsCard1 = GetEffectiveAttackPoints(card1, card2);
                    int effectiveAttackPointsCard2 = GetEffectiveAttackPoints(card2, card1);

                    InsertBattleLogEntryModel logEntry = new InsertBattleLogEntryModel()
                    {
                        Card1 = card1,
                        Card2 = card2,
                        Order = currentRound
                    };

                    StringBuilder roundDescriptionBuilder = new StringBuilder();
                    roundDescriptionBuilder.Append(BuildPlayerCardSummary(deck1.Player, card1, effectiveAttackPointsCard1));
                    roundDescriptionBuilder.Append(" vs. ");
                    roundDescriptionBuilder.AppendLine(BuildPlayerCardSummary(deck2.Player, card2, effectiveAttackPointsCard2));
                    roundDescriptionBuilder.Append("Result: ");

                    if (effectiveAttackPointsCard1 == effectiveAttackPointsCard2)
                        roundDescriptionBuilder.Append("Tie.");
                    else if (effectiveAttackPointsCard1 > effectiveAttackPointsCard2)
                    {
                        roundDescriptionBuilder.Append(BuildWinnerSummary(deck1.Player, currentRound));
                        deck1.Cards.Add(card2);
                        deck2.Cards.Remove(card2);
                    }
                    else
                    {
                        roundDescriptionBuilder.Append(BuildWinnerSummary(deck2.Player, currentRound));
                        deck2.Cards.Add(card2);
                        deck1.Cards.Remove(card2);
                    }

                    logEntry.DeckState1 = JsonSerializer.Serialize(deck1, typeof(Deck));
                    logEntry.DeckState2 = JsonSerializer.Serialize(deck2, typeof(Deck));
                    logEntry.RoundDescription = roundDescriptionBuilder.ToString();

                    battleLogEntries.Add(logEntry);
                }

                InsertBattleLogModel battleLog = new InsertBattleLogModel()
                {
                    Match_ID = match.MatchID,
                    Winner = winner,
                    BattleResult = GetBattleResult(winner, deck1.Player, deck2.Player),
                    Deck1 = deck1,
                    Deck2 = deck2,
                    CreationDate = DateTime.Now,
                    Turns = currentRound
                };

                int battleLogID = battleLogRepository.InsertBattleLog(battleLog, transaction);

                foreach (InsertBattleLogEntryModel logEntryModel in battleLogEntries)
                    battleLogRepository.InsertBattleLogEntry(battleLogID, logEntryModel, transaction);

                transaction.Commit();
            }
        }

        private static AutomaticDuelResult GetBattleResult(User winner, User user1, User user2)
        {
            Assert.NotNull(user1, nameof(user1));
            Assert.NotNull(user2, nameof(user2));

            if (winner == null)
                return AutomaticDuelResult.Draw;
            else if (winner == user1)
                return AutomaticDuelResult.Deck1;
            else if (winner == user2)
                return AutomaticDuelResult.Deck2;
            else
                throw new InvalidOperationException("BattleResult could not be determined.");
        }

        private string BuildPlayerCardSummary(User player, Card playedCard, int effectiveAttackPoints)
        {
            return $"Player: {player.UserName} used Card: [Name: {playedCard.Name}, Type: {Enum.GetName(typeof(CardType), playedCard.Type)}, " +
                $"Element: {Enum.GetName(typeof(ElementType), playedCard.Element)}, AttackPoints: {playedCard.AttackPoints}, " +
                $"EffectiveAttackPoints: {effectiveAttackPoints}]";
        }

        private string BuildWinnerSummary(User winner, int currentRound)
        {
            return $"Player: {winner.UserName} wins Round: {currentRound}.";
        }

        private static int GetEffectiveAttackPoints(Card self, Card opponent)
        {
            Assert.NotNull(self, nameof(self));
            Assert.NotNull(opponent, nameof(opponent));

            if (!(self.Type == CardType.Monster && opponent.Type == CardType.Monster))
            {
                if (ElementIsEffective(self.Element, opponent.Element))
                    return self.AttackPoints * 2;
                else if (ElementIsInEffective(self.Element, opponent.Element))
                    return self.AttackPoints / 2;
            }

            return self.AttackPoints;
        }

        private static bool ElementIsEffective(ElementType self, ElementType opponent)
        {
            return (self == ElementType.Fire && opponent == ElementType.Normal) ||
                (self == ElementType.Water && opponent == ElementType.Fire) ||
                (self == ElementType.Normal && opponent == ElementType.Water);
        }

        private static bool ElementIsInEffective(ElementType self, ElementType opponent)
        {
            return (self == ElementType.Normal && opponent == ElementType.Fire) ||
                (self == ElementType.Fire && opponent == ElementType.Water) ||
                (self == ElementType.Water && opponent == ElementType.Normal);
        }

        private readonly Random randomNumberGenerator = new Random((int)DateTime.Now.Ticks);

        private readonly BattleLogRepository battleLogRepository = new BattleLogRepository(database);
        private readonly DeckRepository deckRepository = new DeckRepository(database);

        private static readonly PostgreSqlDatabase database =
            new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
