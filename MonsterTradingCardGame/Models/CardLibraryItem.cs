namespace MonsterTradingCardGame.Models
{
    public sealed class CardLibraryItem
    {
        public int CardLibrary_ID { get; set; }

        public int User_ID { get; set; }

        public int Card_ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ElementType Element { get; set; }

        public CardType Type { get; set; }

        public int AttackPoints { get; set; }

        public int Quantity { get; set; }
    }
}
