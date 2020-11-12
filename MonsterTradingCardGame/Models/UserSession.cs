using System;

namespace MonsterTradingCardGame.Models
{
    public sealed class UserSession
    {
        public int UserSessionID { get; set; }

        public int UserID { get; set; }

        public Guid Token { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool IsExpired { get { return ExpirationDate < DateTime.Now; } }
    }
}
