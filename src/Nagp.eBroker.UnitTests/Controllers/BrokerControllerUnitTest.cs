using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Nagp.eBroker.Controllers;
using Nagp.eBroker.Helper;
using Nagp.eBroker.Service;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nagp.eBroker.UnitTests.Controllers
{
    public class BrokerControllerUnitTest
    {
        //SUT
        readonly BrokerController _brokerController;

        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
        private readonly Mock<IBrokerService> _mockBrokerService;
        private readonly Mock<IEquityService> _mockEquityService;

        public BrokerControllerUnitTest()
        {
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockBrokerService = new Mock<IBrokerService>();
            _mockEquityService = new Mock<IEquityService>();
            _brokerController = new BrokerController(Mock.Of<ILogger<BrokerController>>(), _mockDateTimeProvider.Object, _mockBrokerService.Object, _mockEquityService.Object);
        }

        [Fact]
        public async Task AddFound_Test_InValidBroker_Return_ForbidResult()
        {
            //Arrange
            _mockBrokerService.Setup(x => x.IsBrokerIdValid(It.IsAny<int>())).ReturnsAsync(false);

            //Act
            var result = await _brokerController.AddFound(5, 5);

            //Assert
            Assert.IsType<ForbidResult>(result);

            //verify each call
            _mockBrokerService.Verify(mock => mock.IsBrokerIdValid(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task AddFound_Test_Return_OkObjectResult()
        {
            //Arrange
            _mockBrokerService.Setup(x => x.IsBrokerIdValid(It.IsAny<int>())).ReturnsAsync(true);
            _mockBrokerService.Setup(x => x.AddFound(It.IsAny<int>(), It.IsAny<decimal>())).ReturnsAsync(true);
            //Act
            var result = await _brokerController.AddFound(5, 5);

            //Assert
            Assert.IsType<OkObjectResult>(result);

            //verify each call
            _mockBrokerService.Verify(mock => mock.IsBrokerIdValid(It.IsAny<int>()), Times.Once());
            _mockBrokerService.Verify(mock => mock.AddFound(It.IsAny<int>(), It.IsAny<decimal>()), Times.Once());
        }

        [Fact]
        public async Task BuyEquityUnits_Test_InValidDateTime_Return_BadRequestResult()
        {
            //Arrange
            _mockDateTimeProvider.Setup(x => x.GetNow()).Returns(new DateTime(2021, 12, 25));

            //Act
            var result = await _brokerController.BuyEquityUnits(5, 5, 1);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task BuyEquityUnits_Test_InValidTime_Return_BadRequestResult()
        {
            //Arrange
            _mockDateTimeProvider.Setup(x => x.GetNow()).Returns(new DateTime(2021, 12, 24, 8, 0, 0));

            //Act
            var result = await _brokerController.BuyEquityUnits(5, 5, 1);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task BuyEquityUnits_Test_InValidBroker_Return_ForbidResult()
        {
            //Arrange
            _mockDateTimeProvider.Setup(x => x.GetNow()).Returns(new DateTime(2021, 12, 24, 10, 0, 0));
            _mockBrokerService.Setup(x => x.IsBrokerIdValid(It.IsAny<int>())).ReturnsAsync(false);

            //Act
            var result = await _brokerController.BuyEquityUnits(5, 5, 1);

            //Assert
            Assert.IsType<ForbidResult>(result);
            _mockBrokerService.Verify(mock => mock.IsBrokerIdValid(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task BuyEquityUnits_Test_Return_OkObjectResult()
        {
            //Arrange
            _mockDateTimeProvider.Setup(x => x.GetNow()).Returns(new DateTime(2021, 12, 24, 10, 0, 0));
            _mockBrokerService.Setup(x => x.IsBrokerIdValid(It.IsAny<int>())).ReturnsAsync(true);

            //Act
            var result = await _brokerController.BuyEquityUnits(5, 5, 1);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            _mockBrokerService.Verify(mock => mock.IsBrokerIdValid(It.IsAny<int>()), Times.Once());
            _mockEquityService.Verify(mock => mock.BuyEquityUnits(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task SellEquityUnits_Test_InValidTime_Return_BadRequestResult()
        {
            //Arrange
            _mockDateTimeProvider.Setup(x => x.GetNow()).Returns(new DateTime(2021, 12, 25));

            //Act
            var result = await _brokerController.SellEquityUnits(5, 5, 1);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SellEquityUnits_Test_InValidBroker_Return_ForbidResult()
        {
            //Arrange
            _mockDateTimeProvider.Setup(x => x.GetNow()).Returns(new DateTime(2021, 12, 24, 10, 0, 0));
            _mockBrokerService.Setup(x => x.IsBrokerIdValid(It.IsAny<int>())).ReturnsAsync(false);

            //Act
            var result = await _brokerController.SellEquityUnits(5, 5, 1);

            //Assert
            Assert.IsType<ForbidResult>(result);
            _mockBrokerService.Verify(mock => mock.IsBrokerIdValid(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task SellEquityUnits_Test_Return_OkObjectResult()
        {
            //Arrange
            _mockDateTimeProvider.Setup(x => x.GetNow()).Returns(new DateTime(2021, 12, 24, 10, 0, 0));
            _mockBrokerService.Setup(x => x.IsBrokerIdValid(It.IsAny<int>())).ReturnsAsync(true);

            //Act
            var result = await _brokerController.SellEquityUnits(5, 5, 1);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            _mockBrokerService.Verify(mock => mock.IsBrokerIdValid(It.IsAny<int>()), Times.Once());
            _mockEquityService.Verify(mock => mock.SellEquityUnits(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }
    }
}
