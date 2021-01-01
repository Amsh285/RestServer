using System;

namespace MonsterTradingCardGame.Entities.TradeEntity
{
    public sealed class CardUnvailableForTradeException : Exception
    {
        public CardUnvailableForTradeException(string message)
            : base(message)
        {
        }
    }
}
