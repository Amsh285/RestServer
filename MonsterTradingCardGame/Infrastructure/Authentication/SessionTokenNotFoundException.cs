using System;

namespace MonsterTradingCardGame.Infrastructure.Authentication
{
    public sealed class SessionTokenNotFoundException : Exception
    {
        public SessionTokenNotFoundException(string message)
            : base(message)
        {
        }
    }
}
