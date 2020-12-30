using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MasterTradingCardGame.Repositories;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Infrastructure.Authentication;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MonsterTradingCardGame.Modules
{
    public sealed class AutomaticDuelMatchmaker
    {
        public MatchmakingEntry FindMatch(RequestContext requestContext, string requestedDeck)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = CookieAuthenticationModule.GetUserSessionFromRequest(requestContext, transaction);
                User user = userRepository.GetUser(session.UserID, transaction);
                Assert.NotNull(user, nameof(user));

                Validate.Condition(deckRepository.Exists(requestedDeck, user.UserID, transaction), $"Requested Deck: {requestedDeck} could not be found for User: {user.UserName}");

                MatchmakingEntry self = new MatchmakingEntry(user, requestedDeck);

                lock (duelQueue)
                {
                    if (duelQueue.Any(mme => mme.Self.UserID == user.UserID || mme.Adversary?.UserID == user.UserID))
                        throw new InvalidOperationException(
                            $"Gegnersuche fehlgeschlagen. Der Benutzer: {user.UserName} " +
                            "ist dem Matchmaking bereits beigetreten.");

                    duelQueue.Add(self);
                }

                const int maxAttemptCount = 60;

                int currentAttempt = 0;
                MatchmakingEntry adversary = null;

                do
                {
                    ++currentAttempt;
                    Thread.Sleep(2000);

                    lock (duelQueue)
                    {
                        if (self.IsMatched)
                            return self;

                        adversary = duelQueue
                            .FirstOrDefault(mmEntry => mmEntry != self);

                        if (adversary != null)
                        {
                            Guid matchID = Guid.NewGuid();

                            adversary.Adversary = user;
                            adversary.AdversaryRequestedDeck = requestedDeck;
                            adversary.IsMatched = true;
                            adversary.MatchID = matchID;

                            duelQueue.Remove(adversary);

                            self.Adversary = adversary.Self;
                            self.AdversaryRequestedDeck = adversary.SelfRequestedDeck;
                            self.IsMatched = true;
                            self.ShouldInitiate = true;
                            self.MatchID = matchID;
                            duelQueue.Remove(self);

                            return self;
                        }
                    }
                } while (currentAttempt <= maxAttemptCount);

                // no Match could be found -> deregister.
                lock (duelQueue)
                {
                    duelQueue.Remove(self);
                }

                return self;
            }
        }

        private readonly DeckRepository deckRepository = new DeckRepository(database);
        private readonly UserRepository userRepository = new UserRepository();

        private static readonly List<MatchmakingEntry> duelQueue = new List<MatchmakingEntry>();

        private static readonly PostgreSqlDatabase database =
            new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
