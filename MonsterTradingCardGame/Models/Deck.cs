using MasterTradingCardGame.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Models
{
    public sealed class Deck
    {
        public int Deck_ID { get; set; }

        public string Name { get; set; }

        public User Player { get; set; }

        public List<Card> Cards { get; set; }

        public string GenerateDeckStateString()
        {
            StringBuilder deckStateBuilder = new StringBuilder();
            deckStateBuilder.AppendLine($"Deck_ID: {Deck_ID}, DeckName: {Name}");
            deckStateBuilder.AppendLine($"User_ID: {Player.UserID}, UserName: {Player.UserName}");
            deckStateBuilder.AppendLine($"CardCount: {Cards.Count}");

            foreach (Card card in Cards)
                deckStateBuilder.AppendLine(
                    $"Card_ID: {card.CardID}, CardName: {card.Name}, Element: {Enum.GetName(typeof(ElementType), card.Element)}, " +
                    $"Type: {Enum.GetName(typeof(CardType), card.Type)}, AttackPoints: {card.AttackPoints}");

            return deckStateBuilder.ToString();
        }
    }
}
