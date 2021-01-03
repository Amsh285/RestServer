using System.Text.Json.Serialization;

namespace MasterTradingCardGame.Models
{
    public class User
    {
        public int UserID{ get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public byte[] Password { get; set; }

        [JsonIgnore]
        public byte[] Salt { get; set; }

        [JsonIgnore]
        public string HashAlgorithm { get; set; }

        public int Coins { get; set; }

        public int Rating { get; set; }

        public int GamesPlayed { get; set; }

        public int GamesWon { get; set; }

        public int GamesLost { get; set; }

        public int Winrate { get; set; }
    }
}
