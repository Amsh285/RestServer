﻿using MasterTradingCardGame.Database;
using MasterTradingCardGame.Models;
using MasterTradingCardGame.Repositories;
using MonsterTradingCardGame.Infrastructure.Authentication;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterTradingCardGame.Entities.DeckEntity
{
    public sealed class DeckEntity
    {
        public void AutoGenerateDeck(RequestContext requestContext, string name, int cardCountPerDeck)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = CookieAuthenticationModule.GetUserSessionFromRequest(requestContext, transaction);

                if (deckRepository.Exists(name, session.UserID, transaction))
                    throw new UniqueConstraintViolationException($"Cannot create Deck with Name: {name}. A Deck with that Name already exists.");

                int createdDeckId = deckRepository.InsertDeck(session.UserID, name, transaction);

                IEnumerable<CardLibraryDeckAssignment> cardLibrary = cardLibraryRepository
                    .GetCardLibraryItemsByUserID(session.UserID, transaction)
                    .Select(i => new CardLibraryDeckAssignment(i));

                int cardsInLibrary = cardLibrary.Sum(l => l.CardLibraryEntry.Quantity);

                if (cardsInLibrary < cardCountPerDeck)
                    throw new NotEnoughCardsInLibraryException(
                        $"Cannot Build Deck. Library is to small. Number of Cards in Library: {cardsInLibrary}. " +
                        $"Number Cards needed for Decks: {cardCountPerDeck}.");

                IEnumerable<int> bestPicks = GetBestPicks(cardCountPerDeck, cardLibrary.ToList());

                Assert.That(
                    bestPicks.Count() == cardCountPerDeck,
                    $"Cards in {nameof(bestPicks)}: {bestPicks.Count()}. " +
                    $"{cardCountPerDeck} are needed to Build a valid Deck."
                );

                foreach (int cardID in bestPicks)
                    deckRepository.AssignCardToDeck(createdDeckId, cardID, transaction);

                transaction.Commit();
            }
        }

        private IEnumerable<int> GetBestPicks(int cardCountPerDeck, List<CardLibraryDeckAssignment> cardLibrary)
        {
            List<int> bestPicks = new List<int>();
            int maxPossibleCardsWithDifferentElement = cardCountPerDeck / Enum.GetNames(typeof(ElementType)).Length;

            // Try to get the maximum number of strongest card from each possible element.
            IEnumerable<IGrouping<ElementType, CardLibraryDeckAssignment>> elementGroupings = cardLibrary
                .GroupBy(l => l.CardLibraryEntry.Element);

            for (int i = 0; i < maxPossibleCardsWithDifferentElement; i++)
            {
                foreach (IGrouping<ElementType, CardLibraryDeckAssignment> grouping in elementGroupings)
                {
                    CardLibraryDeckAssignment strongestCardForCurrentElement = grouping
                        .OrderByDescending(g => g.CardLibraryEntry.AttackPoints)
                        .First();

                    if (strongestCardForCurrentElement.PossibleAssignments > strongestCardForCurrentElement.ExecutedAssignments)
                    {
                        ++strongestCardForCurrentElement.ExecutedAssignments;
                        bestPicks.Add(strongestCardForCurrentElement.CardLibraryEntry.Card_ID);
                    }
                }
            }

            // For the remaining places just get the strongest cards possible
            int remainingPlaces = cardCountPerDeck - bestPicks.Count;

            for (int i = 0; i < remainingPlaces; i++)
            {
                IEnumerable<CardLibraryDeckAssignment> bestPossiblePicks = cardLibrary
                    .Where(l => l.PossibleAssignments > l.ExecutedAssignments);

                Assert.That(bestPossiblePicks.Count() >= remainingPlaces, $"bestPossiblePicks.Count() must be larger or equal to {nameof(remainingPlaces)}");

                CardLibraryDeckAssignment bestPossiblePick = bestPossiblePicks
                    .OrderByDescending(l => l.CardLibraryEntry.AttackPoints)
                    .First();

                ++bestPossiblePick.ExecutedAssignments;
                bestPicks.Add(bestPossiblePick.CardLibraryEntry.Card_ID);
            }

            return bestPicks;
        }

        public Deck GetDeck(RequestContext requestContext, string name)
        {
            using(NpgsqlConnection connection = database.CreateAndOpenConnection())
                using(NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = CookieAuthenticationModule.GetUserSessionFromRequest(requestContext, transaction);
                User user = userRepository.GetUser(session.UserID, transaction);
                Assert.NotNull(user, nameof(user));

                Deck deck = deckRepository.GetDeck(user, name, transaction);

                if (deck == null)
                    throw new DeckNotFoundException($"Deck: Name: {name} - User: {user.UserName} could not be found.");

                return deck;
            }
        }

        private readonly UserRepository userRepository = new UserRepository();
        private readonly DeckRepository deckRepository = new DeckRepository(database);
        private readonly CardLibraryRepository cardLibraryRepository = new CardLibraryRepository(database);

        private static readonly PostgreSqlDatabase database =
            new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
