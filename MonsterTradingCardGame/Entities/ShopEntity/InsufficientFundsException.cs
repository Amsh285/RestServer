using System;

namespace MonsterTradingCardGame.Entities.ShopEntity
{
    public sealed class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(string message)
            : base(message)
        {
        }
    }
}
