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
    public class ShareControllerTests
    {
        private readonly Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();

        private readonly ShareController _shareController;

        public ShareControllerTests()
        {
            _shareController = new ShareController(_shareRepositoryMock.Object);
        }

        [Test]
        public async Task Post_ShouldInsertHourlySharePrice()
        {
            var hourRate = new HourlyShareRate
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
            };

            // Arrange

            // Act
            var result = await _shareController.Post(hourRate);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [Test]
        public async Task Get_ShouldGetHourlySharePrices()
        {
            List<HourlyShareRate> rates = new List<HourlyShareRate>();
            rates.Add(new HourlyShareRate()
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
            });
            _shareRepositoryMock.Setup(mr => mr.Query()).Returns(rates.AsQueryable());

            // Arrange

            // Act
            var result = await _shareController.Get("CBI");

            // Assert
            Assert.NotNull(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var list = okResult.Value as List<HourlyShareRate>;
            Assert.AreEqual(330.0M, list[0].Rate);
        }

    }
}
