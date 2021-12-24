using Microsoft.Extensions.Logging;
using Moq;
using Nagp.eBroker.Data.Entities;
using Nagp.eBroker.Data.Repositories;
using Nagp.eBroker.Service;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nagp.eBroker.UnitTests.Services
{
    public class EquityServiceUnitTest
    {
        //SUT
        readonly IEquityService _equityService;

        readonly Mock<IBrokerAccountRepository> _mockBrokerAccountRepository;
        readonly Mock<IEquityRepository> _mockEquityRepository;
        readonly Mock<IEquitieHoldRepository> _mockEquitieHoldRepository;

        public EquityServiceUnitTest()
        {
            _mockBrokerAccountRepository = new Mock<IBrokerAccountRepository>();
            _mockEquityRepository = new Mock<IEquityRepository>();
            _mockEquitieHoldRepository = new Mock<IEquitieHoldRepository>();
            _equityService = new EquityService(Mock.Of<ILogger<EquityService>>(), _mockBrokerAccountRepository.Object, _mockEquityRepository.Object, _mockEquitieHoldRepository.Object);
        }

        [Fact]
        public async Task BuyEquityUnits_Test_NoEquity_Fail_Return_False()
        {
            //Arrange
            _mockEquityRepository.Setup(x => x.GetByID(It.IsAny<int>()));

            //Act
            var result = await _equityService.BuyEquityUnits(5, 5, 10);

            //Assert
            Assert.False(result);

            //verify each call
            _mockEquityRepository.Verify(mock => mock.GetByID(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task BuyEquityUnits_Test_Insufficient_EquityAvl_Fail_Return_False()
        {
            //Arrange
            var equity = new Equity
            {
                Id = 1,
                Name = "Test Equity",
                PricePerUnit = 100M,
                Units = 10
            };

            _mockEquityRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(equity);

            //Act
            var result = await _equityService.BuyEquityUnits(5, equity.Id, equity.Units + 2);

            //Assert
            Assert.False(result);

            //verify each call
            _mockEquityRepository.Verify(mock => mock.GetByID(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task BuyEquityUnits_Test_Insufficient_BrokerFund_Fail_Return_False()
        {
            //Arrange
            var equity = new Equity
            {
                Id = 1,
                Name = "Test Equity",
                PricePerUnit = 100M,
                Units = 10
            };

            var brokerAccount = new BrokerAccount
            {
                Id = 1,
                AccountName = "Test Brker",
                Funds = 100M,
                EquitieHolds = new List<EquitieHold>()
            };

            _mockEquityRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(equity);
            _mockBrokerAccountRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(brokerAccount);

            //Act
            var result = await _equityService.BuyEquityUnits(5, equity.Id, equity.Units);

            //Assert
            Assert.False(result);

            //verify each call
            _mockEquityRepository.Verify(mock => mock.GetByID(It.IsAny<int>()), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.GetByID(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task BuyEquityUnits_Test_Success_NoHoldEquity_Return_True()
        {
            //Arrange
            var equity = new Equity
            {
                Id = 1,
                Name = "Test Equity",
                PricePerUnit = 100M,
                Units = 10
            };

            var brokerAccount = new BrokerAccount
            {
                Id = 1,
                AccountName = "Test Brker",
                Funds = equity.Units * equity.PricePerUnit,
                EquitieHolds = new List<EquitieHold>()
            };
            _mockEquityRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(equity);
            _mockBrokerAccountRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(brokerAccount);
            _mockEquitieHoldRepository.Setup(x => x.Save()).ReturnsAsync(1);

            //Act
            var result = await _equityService.BuyEquityUnits(brokerAccount.Id, equity.Id, equity.Units);

            //Assert
            Assert.True(result);
            Assert.Equal(0, equity.Units);
            Assert.Equal(0, brokerAccount.Funds);
            //verify each call
            _mockEquityRepository.Verify(mock => mock.GetByID(equity.Id), Times.Once());
            _mockEquityRepository.Verify(mock => mock.Update(equity), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.GetByID(brokerAccount.Id), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.Update(brokerAccount), Times.Once());
            _mockEquitieHoldRepository.Verify(mock => mock.GetByID(brokerAccount.Id, equity.Id), Times.Once());
            _mockEquitieHoldRepository.Verify(mock => mock.Insert(It.IsAny<EquitieHold>()), Times.Once());
            _mockEquitieHoldRepository.Verify(mock => mock.Save(), Times.Once());
        }

        [Theory]
        [InlineData(10, 1, 10)]
        [InlineData(15, 5, 10)]
        public async Task BuyEquityUnits_Test_Success_WithHoldEquity_Return_True(int equityUnits, int initialHoldUnits, int buyEquity)
        {
            //Arrange
            var equity = new Equity
            {
                Id = 1,
                Name = "Test Equity",
                PricePerUnit = 100M,
                Units = equityUnits
            };

            var brokerAccount = new BrokerAccount
            {
                Id = 1,
                AccountName = "Test Brker",
                Funds = equity.Units * equity.PricePerUnit,
                EquitieHolds = new List<EquitieHold>()
            };

            var equitieHold = new EquitieHold
            {
                BrokerAccount = brokerAccount,
                BrokerAccountId = brokerAccount.Id,
                Equity = equity,
                EquityId = equity.Id,
                HoldUnits = initialHoldUnits
            };

            _mockEquityRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(equity);
            _mockBrokerAccountRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(brokerAccount);
            _mockEquitieHoldRepository.Setup(x => x.GetByID(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(equitieHold);
            _mockEquitieHoldRepository.Setup(x => x.Save()).ReturnsAsync(1);

            //Act
            var result = await _equityService.BuyEquityUnits(brokerAccount.Id, equity.Id, buyEquity);

            //Assert
            Assert.True(result);
            Assert.Equal(equityUnits, equity.Units + buyEquity);
            Assert.Equal(equitieHold.HoldUnits, initialHoldUnits + buyEquity);
            Assert.Equal((equityUnits - buyEquity) * equity.PricePerUnit, brokerAccount.Funds);
            //verify each call
            _mockEquityRepository.Verify(mock => mock.GetByID(equity.Id), Times.Once());
            _mockEquityRepository.Verify(mock => mock.Update(equity), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.GetByID(brokerAccount.Id), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.Update(brokerAccount), Times.Once());
            _mockEquitieHoldRepository.Verify(mock => mock.GetByID(brokerAccount.Id, equity.Id), Times.Once());
            _mockEquitieHoldRepository.Verify(mock => mock.Update(It.IsAny<EquitieHold>()), Times.Once());
            _mockEquitieHoldRepository.Verify(mock => mock.Save(), Times.Once());
        }

        [Fact]
        public async Task SellEquityUnits_Test_Fail_NoHoldEquity_Return_False()
        {
            //Arrange
            _mockEquitieHoldRepository.Setup(x => x.GetByID(It.IsAny<int>(), It.IsAny<int>()));

            //Act
            var result = await _equityService.SellEquityUnits(1, 100, 10);

            //Assert
            Assert.False(result);

            //verify each call
            _mockEquitieHoldRepository.Verify(mock => mock.GetByID(1, 100), Times.Once());
        }

        [Fact]
        public async Task SellEquityUnits_Test_Fail_Insufficient_HoldEquity_Return_False()
        {
            //Arrange
            var equitieHold = new EquitieHold
            {
                HoldUnits = 1
            };
            _mockEquitieHoldRepository.Setup(x => x.GetByID(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(equitieHold);
            //Act
            var result = await _equityService.SellEquityUnits(1, 100, 10);

            //Assert
            Assert.False(result);

            //verify each call
            _mockEquitieHoldRepository.Verify(mock => mock.GetByID(1, 100), Times.Once());
        }

        [Theory]
        [InlineData(10, 10, 10)]
        [InlineData(15, 15, 10)]
        public async Task SellEquityUnits_Test_Success_Return_True(int equityUnits, int holdUnits, int numberOfUnitsToSell)
        {
            //Arrange
            var equity = new Equity
            {
                Id = 1,
                Name = "Test Equity",
                PricePerUnit = 100M,
                Units = equityUnits
            };

            var brokerAccount = new BrokerAccount
            {
                Id = 1,
                AccountName = "Test Brker",
                Funds = equity.Units * equity.PricePerUnit,
                EquitieHolds = new List<EquitieHold>()
            };

            var equitieHold = new EquitieHold
            {
                BrokerAccount = brokerAccount,
                BrokerAccountId = brokerAccount.Id,
                Equity = equity,
                EquityId = equity.Id,
                HoldUnits = holdUnits
            };

            _mockEquityRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(equity);
            _mockBrokerAccountRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(brokerAccount);
            _mockEquitieHoldRepository.Setup(x => x.GetByID(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(equitieHold);
            _mockEquitieHoldRepository.Setup(x => x.Save()).ReturnsAsync(1);

            //Act
            var result = await _equityService.SellEquityUnits(brokerAccount.Id, equity.Id, numberOfUnitsToSell);

            //Assert
            Assert.True(result);
            Assert.Equal(equityUnits + numberOfUnitsToSell, equity.Units);
            Assert.Equal(equitieHold.HoldUnits, holdUnits - numberOfUnitsToSell);

            //verify each call
            _mockEquityRepository.Verify(mock => mock.GetByID(equity.Id), Times.Once());
            _mockEquityRepository.Verify(mock => mock.Update(equity), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.GetByID(brokerAccount.Id), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.Update(brokerAccount), Times.Once());
            _mockEquitieHoldRepository.Verify(mock => mock.GetByID(brokerAccount.Id, equity.Id), Times.Once());
            _mockEquitieHoldRepository.Verify(mock => mock.Update(It.IsAny<EquitieHold>()), Times.Once());
            _mockEquitieHoldRepository.Verify(mock => mock.Save(), Times.Once());
        }
    }
}
