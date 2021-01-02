using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Models
{
    public sealed class Trade
    {
        public Guid Trade_ID { get; set; }

        public int User_ID { get; set; }

        public int Card_ID { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
