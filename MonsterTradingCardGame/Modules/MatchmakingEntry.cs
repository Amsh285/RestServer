using MasterTradingCardGame.Models;
using RestServer.WebServer.Infrastructure;
using System;

namespace MonsterTradingCardGame.Modules
{
    public sealed class MatchmakingEntry
    {
        public User Self { get; set; }

        public string SelfRequestedDeck { get; set; }

        public User Adversary { get; set; }

        public string AdversaryRequestedDeck { get; set; }

        public bool IsMatched { get; set; }

        public bool ShouldInitiate { get; set; }

        public Guid MatchID { get; set; }

        public MatchmakingEntry()
        {
        }

        public MatchmakingEntry(User self, string selfRequestedDeck)
        {
            Assert.NotNull(self, nameof(self));
            Assert.NotNull(selfRequestedDeck, nameof(selfRequestedDeck));

            this.Self = self;
            this.SelfRequestedDeck = selfRequestedDeck;
            this.IsMatched = false;
            this.ShouldInitiate = false;
        }
    }
}
