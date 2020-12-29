using System;

namespace MonsterTradingCardGame.Entities.PlayerEntity
{
    public sealed class NoBoosterPackageAssignedException : Exception
    {
        public NoBoosterPackageAssignedException(string message)
            : base(message)
        {
        }
    }
}
