using System;
using System.Threading.Tasks;
using CrossExchange.Controller;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace CrossExchange.Tests
{
    public class TradeControllerTest
	{
        private readonly Mock<ITradeRepository> _tradeRepositoryMock = new Mock<ITradeRepository>();

		private readonly Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();

		private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock = new Mock<IPortfolioRepository>();

		private readonly TradeController _tradeController;

		public TradeControllerTest()
        {
            _tradeController = new TradeController(_shareRepositoryMock.Object, _tradeRepositoryMock.Object, _portfolioRepositoryMock.Object);
					}

        [Test]
        public async Task Post_ShouldInsertTrade()
        {
            List<HourlyShareRate> rates = new List<HourlyShareRate>();
            rates.Add(new HourlyShareRate()
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
            });
            _shareRepositoryMock.Setup(mr => mr.Query()).Returns(rates.AsQueryable());

            List<Portfolio> portfolios = new List<Portfolio>();
            portfolios.Add(new Portfolio()
            {
                Id = 1,
                Name = "test"
            });
            _portfolioRepositoryMock.Setup(mr => mr.Query()).Returns(portfolios.AsQueryable());
            
            var tradeModel = new TradeModel
            {
                Symbol = "CBI",
                Action = "BUY",
                NoOfShares = 5,
                PortfolioId = 1
            };

            var result = await _tradeController.Post(tradeModel);
			
			// Assert
			Assert.NotNull(result);
            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
			
        }

        [Test]
        public async Task Post_ShouldNotInsertTrade()
        {
            List<HourlyShareRate> rates = new List<HourlyShareRate>();
            rates.Add(new HourlyShareRate()
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
            });
            _shareRepositoryMock.Setup(mr => mr.Query()).Returns(rates.AsQueryable());

            List<Portfolio> portfolios = new List<Portfolio>();
            portfolios.Add(new Portfolio()
            {
                Id = 1,
                Name = "test"
            });
            _portfolioRepositoryMock.Setup(mr => mr.Query()).Returns(portfolios.AsQueryable());

            var tradeModel = new TradeModel
            {
                Symbol = "CBI",
                Action = "SELL",
                NoOfShares = 5,
                PortfolioId = 1
            };

            var result = await _tradeController.Post(tradeModel);

            // Assert
            Assert.NotNull(result);
            var BadRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(BadRequestResult);
            Assert.AreEqual(400, BadRequestResult.StatusCode);

        }

        [Test]
        public async Task Get_ShouldGetAllTrades()
        {
            List<Trade> trades = new List<Trade>();
            trades.Add(new Trade()
            {
                Action = "BUY",
                NoOfShares = 5,
                PortfolioId = 1,
                Price = 300,
                Symbol = "CBI"
            });
            _tradeRepositoryMock.Setup(mr => mr.Query()).Returns(trades.AsQueryable());

            // Arrange

            // Act
            var result = await _tradeController.GetAllTradings(1);

            // Assert
            Assert.NotNull(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var list = okResult.Value as List<Trade>;
            Assert.AreEqual("CBI", list[0].Symbol);
            Assert.AreEqual("BUY", list[0].Action);
        }
    }
}
