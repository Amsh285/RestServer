﻿namespace MonsterTradingCardGame.Models
{
    public sealed class InsertBattleLogEntryModel
    {
        public Card Card1 { get; set; }

        public Card Card2 { get; set; }

        public string DeckState1 { get; set; }

        public string DeckState2 { get; set; }

        public string RoundDescription { get; set; }

        public int Order { get; set; }
    }
}
