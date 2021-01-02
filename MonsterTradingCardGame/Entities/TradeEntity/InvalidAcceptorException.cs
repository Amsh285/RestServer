using System;

namespace MonsterTradingCardGame.Entities.TradeEntity
{
    public sealed class InvalidAcceptorException : Exception
    {
        public InvalidAcceptorException(string message)
            : base(message)
        {
        }
    }
}
