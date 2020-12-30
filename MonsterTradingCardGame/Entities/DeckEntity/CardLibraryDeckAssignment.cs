using MonsterTradingCardGame.Models;
using RestServer.WebServer.Infrastructure;

namespace MonsterTradingCardGame.Entities.DeckEntity
{
    public sealed class CardLibraryDeckAssignment
    {
        public CardLibraryItem CardLibraryEntry { get; private set; }

        public int PossibleAssignments { get { return CardLibraryEntry.Quantity; } }

        public int ExecutedAssignments { get; set; }

        public CardLibraryDeckAssignment(CardLibraryItem cardLibraryEntry)
        {
            Assert.NotNull(cardLibraryEntry, nameof(cardLibraryEntry));

            this.CardLibraryEntry = cardLibraryEntry;
            this.ExecutedAssignments = 0;
        }
    }
}
