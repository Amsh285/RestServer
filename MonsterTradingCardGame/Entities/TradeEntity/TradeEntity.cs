using MasterTradingCardGame.Database;
using MonsterTradingCardGame.Infrastructure.Authentication;
using MonsterTradingCardGame.Models;
using MonsterTradingCardGame.Repositories;
using Npgsql;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterTradingCardGame.Entities.TradeEntity
{
    public sealed class TradeEntity
    {
        public Card GetFirstAvailableCardForTrade(RequestContext requestContext)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = CookieAuthenticationModule.GetUserSessionFromRequest(requestContext, transaction);
                return cardRepository.GetCardsWithoutDeck(session.UserID, transaction)
                    .FirstOrDefault();
            }
        }

        public void CreateTrade(Guid tradeID, Card card, RequestContext requestContext)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = CookieAuthenticationModule.GetUserSessionFromRequest(requestContext, transaction);

                ValidateCardForTrade(card.CardID, session.UserID, transaction);

                if (tradeRepository.TradeUIDExists(tradeID, transaction))
                    throw new UniqueConstraintViolationException($"Cannot Insert new Trade with ID:{tradeID}. This ID already exists.");

                tradeRepository.InsertTrade(tradeID, session.UserID, card.CardID, transaction);
                transaction.Commit();
            }
        }

        public void CreateTradeOffer(Guid tradeOfferID, Guid tradeID, Card offer, RequestContext requestContext)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = CookieAuthenticationModule.GetUserSessionFromRequest(requestContext, transaction);
                Trade target = tradeRepository.GetTrade(tradeID, transaction);

                ValidateCardForTrade(offer.CardID, session.UserID, transaction);

                if (target.User_ID == session.UserID)
                    throw new InvalidTradeException($"Session UserID: {session.UserID}, you cannot Trade with yourself.");

                if (tradeRepository.TradeOfferUIDExists(tradeOfferID, transaction))
                    throw new UniqueConstraintViolationException($"Cannot Insert new Offer with ID:{tradeOfferID}. This ID already exists.");

                tradeRepository.InsertTradeOffer(tradeOfferID, tradeID, session.UserID, offer.CardID, transaction);
                transaction.Commit();
            }
        }

        public void AcceptTrade(Guid tradeID, Guid tradeOfferID, RequestContext requestContext)
        {
            using (NpgsqlConnection connection = database.CreateAndOpenConnection())
            using (NpgsqlTransaction transaction = connection.BeginTransaction())
            {
                UserSession session = CookieAuthenticationModule.GetUserSessionFromRequest(requestContext, transaction);

                Trade trade = tradeRepository.GetTrade(tradeID, transaction);
                TradeOffer offer = tradeRepository.GetTradeOffer(tradeOfferID, transaction);

                if (trade == null)
                    throw new NotFoundException($"Trade with ID:{trade} could not be found.");

                if (offer == null)
                    throw new NotFoundException($"TradeOffer with ID:{tradeOfferID} could not be found.");

                if (trade.User_ID != session.UserID)
                    throw new InvalidAcceptorException($"Cannot Accept Trade with ID: {trade.Trade_ID}. Invalid User.");

                ValidateCardForTrade(trade.Card_ID, trade.User_ID, transaction);
                ValidateCardForTrade(offer.Card_ID, offer.User_ID, transaction);

                cardLibraryRepository.AddCardToLibrary(trade.Card_ID, offer.User_ID, transaction);

                CardLibraryItem tradeItem = cardLibraryRepository.GetCardLibraryItem(trade.Card_ID, trade.User_ID, transaction);
                cardLibraryRepository.RemoveCardFromLibrary(tradeItem, transaction);

                cardLibraryRepository.AddCardToLibrary(offer.Card_ID, trade.User_ID, transaction);

                CardLibraryItem offerItem = cardLibraryRepository.GetCardLibraryItem(offer.Card_ID, offer.User_ID, transaction);
                cardLibraryRepository.RemoveCardFromLibrary(offerItem, transaction);

                tradeRepository.DeleteTradeOffer(tradeOfferID, transaction);
                tradeRepository.DeleteTrade(tradeID, transaction);

                transaction.Commit();
            }
        }

        private void ValidateCardForTrade(int cardID, int userID, NpgsqlTransaction transaction)
        {
            if (!cardLibraryRepository.CardExists(cardID, userID, transaction))
                throw new CardUnvailableForTradeException(
                    $"Card: [UserID:{userID}, CardID:{cardID}] cannot be traded, the specified Card isn´t located in the specified Users CardLibrary."
                );

            if (!cardLibraryRepository.CardIsAvailableForTrading(cardID, userID, transaction))
                throw new CardUnvailableForTradeException(
                    $"Card: [UserID:{userID}, CardID:{cardID}] cannot be traded, There is one or more Deck/s that use all Quantities of the specified Card."
                );
        }

        private readonly TradeRepository tradeRepository = new TradeRepository(database);
        private readonly CardLibraryRepository cardLibraryRepository = new CardLibraryRepository(database);
        private readonly CardRepository cardRepository = new CardRepository(database);

        private static readonly PostgreSqlDatabase database =
            new PostgreSqlDatabase("Host=localhost;Port=5433;Username=postgres;Password=Badger123!;Database=MonsterTradingCardGame");
    }
}
