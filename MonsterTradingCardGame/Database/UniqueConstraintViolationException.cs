using System;

namespace MasterTradingCardGame.Database
{
    public sealed class UniqueConstraintViolationException : Exception
    {
        public UniqueConstraintViolationException(string message)
            : base(message)
        {
        }
    }
}
