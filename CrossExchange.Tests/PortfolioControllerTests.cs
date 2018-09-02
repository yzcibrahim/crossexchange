using System;
using System.Threading.Tasks;
using CrossExchange.Controller;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace CrossExchange.Tests
{
    public class PortfolioControllerTests
    {
        private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock = new Mock<IPortfolioRepository>();

        private readonly Mock<ITradeRepository> _tradeRepositoryMock = new Mock<ITradeRepository>();

        private readonly Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();

        private readonly PortfolioController _portfolioController;

        public PortfolioControllerTests()
        {
            _portfolioController = new PortfolioController(_shareRepositoryMock.Object, _tradeRepositoryMock.Object, _portfolioRepositoryMock.Object);
        }

        [Test]
        public async Task Post_ShouldInsertPortfolio()
        {
         
           Portfolio p=new Portfolio()
            {
                Id = 1,
                Name = "test"
            };
          
            // Arrange

            // Act
            var result = await _portfolioController.Post(p);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [Test]
        public async Task Get_ShouldGetPortfolioById()
        {
            List<Portfolio> portfolios = new List<Portfolio>();
            portfolios.Add(new Portfolio()
            {
                Id = 1,
                Name = "test"
            });
            _portfolioRepositoryMock.Setup(mr => mr.Query()).Returns(portfolios.AsQueryable());

            // Arrange

            // Act
            var result = await _portfolioController.GetPortfolioInfo(1);

            // Assert
            Assert.NotNull(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var porfRes = okResult.Value as Portfolio;
            Assert.AreEqual("test", porfRes.Name);
        }

    }
}
