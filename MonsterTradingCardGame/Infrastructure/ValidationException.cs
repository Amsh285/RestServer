﻿using System;

namespace MonsterTradingCardGame.Infrastructure
{
    public sealed class ValidationException : Exception
    {
        public ValidationException(string message)
           : base(message)
        {
        }
    }
}
