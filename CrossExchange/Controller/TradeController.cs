﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CrossExchange.Controller
{
    [Route("api/Trade")]
    public class TradeController : ControllerBase
    {
        private IShareRepository _shareRepository { get; set; }
        private ITradeRepository _tradeRepository { get; set; }
        private IPortfolioRepository _portfolioRepository { get; set; }

        public TradeController(IShareRepository shareRepository, ITradeRepository tradeRepository, IPortfolioRepository portfolioRepository)
        {
            _shareRepository = shareRepository;
            _tradeRepository = tradeRepository;
            _portfolioRepository = portfolioRepository;
			
        }


        [HttpGet("{portfolioid}")]
        public async Task<IActionResult> GetAllTradings([FromRoute]int portFolioid)
        {
            var trade = _tradeRepository.Query().Where(x => x.PortfolioId.Equals(portFolioid));
            return Ok(trade);
        }



        /*************************************************************************************************************************************
        For a given portfolio, with all the registered shares you need to do a trade which could be either a BUY or SELL trade.
		For a particular trade keep following conditions in mind:
		BUY:
        a) The rate at which the shares will be bought will be the latest price in the database.
		b) The share specified should be a registered one otherwise it should be considered a bad request. 
		c) The Portfolio of the user should also be registered otherwise it should be considered a bad request. 
                
        SELL:
        a) The share should be there in the portfolio of the customer.
		b) The Portfolio of the user should be registered otherwise it should be considered a bad request. 
		c) The rate at which the shares will be sold will be the latest price in the database.
        d) The number of shares should be sufficient so that it can be sold. 
        Hint: You need to group the total shares bought and sold of a particular share and see the difference to figure out if there are sufficient quantities available for SELL. 

        *************************************************************************************************************************************/

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TradeModel model)
        {

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var trade = CastTrade(model);

			var share =  _shareRepository.Query().Where(x => x.Symbol.Equals(trade.Symbol)).OrderByDescending(c => c.TimeStamp).FirstOrDefault();
			
			if (share == null)
				return BadRequest("The Share could not be found");

			var portfolio = _portfolioRepository.Query().Where(x => x.Id.Equals(trade.PortfolioId)).FirstOrDefault();

			if (portfolio == null)
				return BadRequest("The portfolio should be registerd");

			trade.Price = share.Rate;

			if (trade.Action == "SELL")
			{
				var trades=_tradeRepository.Query().Where(x => x.PortfolioId == trade.PortfolioId && x.Symbol == trade.Symbol).ToList();

				int totalBuy = trades.Where(c=>c.Action=="BUY").Sum(x=>x.NoOfShares);
				int totalSell = trades.Where(c => c.Action == "SELL").Sum(x=>x.NoOfShares);

				if (totalBuy < totalSell + trade.NoOfShares)
				{
					int avaliableForSell = totalBuy - totalSell;
					return BadRequest($"The number of shares is not sufficent. Avaliable share is  {avaliableForSell}");
				}
			}
			
			await _tradeRepository.InsertAsync(trade);
			return Created("Trade", trade);
        }

		private Trade CastTrade(TradeModel model)
		{
			var res = new Trade();
			
			res.Action = model.Action;
			res.NoOfShares = model.NoOfShares;
			res.Price = 0;
			res.Symbol = model.Symbol;
			res.PortfolioId = model.PortfolioId;
			return res;
		}
		
	}
}
