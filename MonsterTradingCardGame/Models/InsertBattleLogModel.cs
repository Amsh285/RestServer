using MasterTradingCardGame.Models;
using MonsterTradingCardGame.Modules;
using System;

namespace MonsterTradingCardGame.Models
{
    public sealed class InsertBattleLogModel
    {
        public Guid Match_ID { get; set; }

        public Deck Deck1 { get; set; }

        public Deck Deck2 { get; set; }

        public User Winner { get; set; }

        public AutomaticDuelResult BattleResult { get; set; }

        public int Turns { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
