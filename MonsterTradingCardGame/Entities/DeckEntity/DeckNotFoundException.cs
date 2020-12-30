using System;

namespace MonsterTradingCardGame.Entities.DeckEntity
{
    public sealed class DeckNotFoundException : Exception
    {
        public DeckNotFoundException(string message)
            : base(message)
        {
        }
    }
}
