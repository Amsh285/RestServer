using System;
using System.Collections.Generic;
using System.Text;

namespace MasterTradingCardGame.Models
{
    public class User
    {
        public int UserID{ get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public byte[] Password { get; set; }

        public byte[] Salt { get; set; }

        public string HashAlgorithm { get; set; }

        public int Coins { get; set; }
    }
}
