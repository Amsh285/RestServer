using System.Text.Json.Serialization;

namespace MonsterTradingCardGame.Models
{
    public class Card
    {
        public int CardID { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ElementType Element { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CardType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int AttackPoints { get; set; }
    }
}
