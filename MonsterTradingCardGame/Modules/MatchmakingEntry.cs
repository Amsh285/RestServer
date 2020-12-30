using MasterTradingCardGame.Models;
using RestServer.WebServer.Infrastructure;
using System;

namespace MonsterTradingCardGame.Modules
{
    public sealed class MatchmakingEntry
    {
        public User Self { get; set; }

        public User Adversary { get; set; }

        public bool IsMatched { get; set; }

        public bool ShouldInitiate { get; set; }

        public Guid MatchID { get; set; }

        public MatchmakingEntry(User self)
        {
            Assert.NotNull(self, nameof(self));

            this.Self = self;
            this.IsMatched = false;
            this.ShouldInitiate = false;
        }
    }
}
