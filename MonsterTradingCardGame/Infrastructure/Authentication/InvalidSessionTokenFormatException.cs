using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Infrastructure.Authentication
{
    public sealed class InvalidSessionTokenFormatException : Exception
    {
        public InvalidSessionTokenFormatException(string message)
            : base(message)
        {
        }
    }
}
