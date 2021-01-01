using System;

namespace MonsterTradingCardGame.Entities.TradeEntity
{
    public sealed class InvalidTradeException : Exception
    {
        public InvalidTradeException(string message)
            : base(message)
        {
        }
    }
}
