namespace MonsterTradingCardGame.Models
{
    public sealed class BattleLogEntry
    {
        public int BattleLogEntry_ID { get; set; }

        public int BattleLog_ID { get; set; }

        public BattleLogCard Card1 { get; set; }

        public BattleLogCard Card2 { get; set; }

        public string DeckState1 { get; set; }

        public string DeckState2 { get; set; }

        public string RoundDescription { get; set; }

        public int Order { get; set; }
    }
}
