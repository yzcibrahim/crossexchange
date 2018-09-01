using System;
using System.Threading.Tasks;
using CrossExchange.Controller;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;

namespace CrossExchange.Tests
{
    public class TradeControllerTest
	{
        private readonly Mock<ITradeRepository> _tradeRepositoryMock = new Mock<ITradeRepository>();

		private readonly Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();

		private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock = new Mock<IPortfolioRepository>();

		private readonly TradeController _tradeController;

		private readonly ShareController _shareController;

		private readonly PortfolioController _portfolioController;

		public TradeControllerTest()
        {
			_tradeController = new TradeController(_shareRepositoryMock.Object, _tradeRepositoryMock.Object, _portfolioRepositoryMock.Object);
			_shareController = new ShareController(_shareRepositoryMock.Object);
			_portfolioController = new PortfolioController(_shareRepositoryMock.Object, _tradeRepositoryMock.Object, _portfolioRepositoryMock.Object);
		}

        [Test]
        public async Task Post_ShouldNotInsertTrade()
        {
			var tradeModel = new TradeModel
			{
				Symbol = "CBI",
				Action = "BUY",
				NoOfShares = 5,
				PortfolioId = 1
            };

			// Arrange

			// Act
			var hourRate = new HourlyShareRate
			{
				Id=8,
				Symbol = "CBI",
				Rate = 330.0M,
				TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
			};
			var resShare = await _shareController.Post(hourRate);

			var portfolio = new Portfolio()
			{
				Id=1,
				Name = "test"
			};

			var resPortfolio = await _portfolioController.Post(portfolio);

			var result = await _tradeController.Post(tradeModel);
			
			// Assert
			Assert.NotNull(result);
			Console.WriteLine(result.ToString());
            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
			Console.Read();
        }
        
    }
}
