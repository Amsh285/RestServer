using System;

namespace MonsterTradingCardGame.Entities.DeckEntity
{
    public sealed class NotEnoughCardsInLibraryException : Exception
    {
        public NotEnoughCardsInLibraryException(string message)
            : base(message)
        {
        }
    }
}
