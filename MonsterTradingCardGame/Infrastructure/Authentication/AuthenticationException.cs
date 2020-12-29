using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Infrastructure.Authentication
{
    public sealed class AuthenticationException : Exception
    {
        public AuthenticationException(string message)
            : base(message)
        {
        }
    }
}
