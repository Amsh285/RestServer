namespace MonsterTradingCardGame.Models
{
    public sealed class HighScoreEntry
    {
        public string UserName { get; set; }

        public int GamesWon { get; set; }

        public int GamesLost { get; set; }

        public int Score { get; set; }
    }
}
