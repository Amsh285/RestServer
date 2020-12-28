using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Infrastructure.Authentication
{
    public sealed class InvalidCookieFormatException : Exception
    {
        public InvalidCookieFormatException(string message)
            : base(message)
        {
        }
    }
}
