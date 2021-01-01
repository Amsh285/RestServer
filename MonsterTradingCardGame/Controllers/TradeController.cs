using MonsterTradingCardGame.Entities.TradeEntity;
using MonsterTradingCardGame.Infrastructure;
using MonsterTradingCardGame.Infrastructure.Authentication;
using MonsterTradingCardGame.Models;
using RestServer.WebServer.CommunicationObjects;
using RestServer.WebServer.EndpointHandling;
using RestServer.WebServer.EndpointHandling.Attributes;
using RestServer.WebServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Controllers
{
    public sealed class TradeController : ControllerBase
    {
        public TradeController(RequestContext requestContext)
        {
            Assert.NotNull(requestContext, nameof(requestContext));

            this.requestContext = requestContext;
        }

        [HttpGet("GetTradeAvailableCard")]
        public IActionResult GetTradeAvailableCard()
        {
            try
            {
                Card result = tradeEntity.GetFirstAvailableCardForTrade(requestContext);
                return Json(result);
            }
            catch (SessionTokenNotFoundException nfEx)
            {
                return Unauthorized(nfEx.Message);
            }
            catch (Exception ex) when (ex is InvalidSessionTokenFormatException || ex is SessionExpiredException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{tradeID}")]
        public IActionResult CreateTrade(Guid tradeID, Card cardToBeTraded)
        {
            try
            {
                tradeEntity.CreateTrade(tradeID, cardToBeTraded, requestContext);
                return Created("Trade Successfully created.");
            }
            catch (SessionTokenNotFoundException nfEx)
            {
                return Unauthorized(nfEx.Message);
            }
            catch (Exception ex) when (ex is InvalidSessionTokenFormatException || ex is SessionExpiredException
                || ex is CardUnvailableForTradeException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{tradeID}/offer/{tradeOfferID}")]
        public IActionResult CreateTradeOffer(Guid tradeOfferID, Guid tradeID, Card offer)
        {
            try
            {
                tradeEntity.CreateTradeOffer(tradeOfferID, tradeID, offer, requestContext);
                return Created("Trade- Offer Successfully created.");
            }
            catch (SessionTokenNotFoundException nfEx)
            {
                return Unauthorized(nfEx.Message);
            }
            catch (Exception ex) when (ex is InvalidSessionTokenFormatException || ex is SessionExpiredException
                || ex is InvalidTradeException || ex is CardUnvailableForTradeException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{tradeID}/offer/{tradeOfferID}/accept")]
        public IActionResult AcceptOffer(Guid tradeID, Guid tradeOfferID)
        {
            try
            {
                tradeEntity.AcceptTrade(tradeID, tradeOfferID, requestContext);
                return Ok("Trade executed.");
            }
            catch (SessionTokenNotFoundException nfEx)
            {
                return Unauthorized(nfEx.Message);
            }
            catch (Exception ex) when (ex is InvalidSessionTokenFormatException || ex is SessionExpiredException
                || ex is CardUnvailableForTradeException)
            {
                return BadRequest(ex.Message);
            }
        }

        private readonly RequestContext requestContext;
        private static readonly TradeEntity tradeEntity = new TradeEntity();
    }
}
