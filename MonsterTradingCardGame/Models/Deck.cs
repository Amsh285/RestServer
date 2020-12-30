using MasterTradingCardGame.Models;
using System.Collections.Generic;

namespace MonsterTradingCardGame.Models
{
    public sealed class Deck
    {
        public int Deck_ID { get; set; }

        public string Name { get; set; }

        public User Player { get; set; }

        public IEnumerable<Card> Cards { get; set; }
    }
}
