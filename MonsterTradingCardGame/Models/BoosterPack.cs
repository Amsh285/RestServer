using System.Collections.Generic;

namespace MonsterTradingCardGame.Models
{
    public sealed class BoosterPack
    {
        public int BoosterPackID { get; set; }

        public int UserID { get; set; }

        public int CardCount { get; set; }

        public IReadOnlyCollection<BoosterPackCard> AssignedCards { get; set; }
    }
}
