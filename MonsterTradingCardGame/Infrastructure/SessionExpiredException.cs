using System;

namespace MonsterTradingCardGame.Infrastructure
{
    public sealed class SessionExpiredException : Exception
    {
        public SessionExpiredException(string message)
            : base(message)
        {
        }
    }
}
