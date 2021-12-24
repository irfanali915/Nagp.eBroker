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
    public class BrokerServiceUnitTest
    {
        //SUT
        readonly IBrokerService _brokerService;

        readonly Mock<IBrokerAccountRepository> _mockBrokerAccountRepository;
        public BrokerServiceUnitTest()
        {
            _mockBrokerAccountRepository = new Mock<IBrokerAccountRepository>();
            _brokerService = new BrokerService(Mock.Of<ILogger<BrokerService>>(), _mockBrokerAccountRepository.Object);
        }

        [Fact]
        public async Task IsBrokerIdValid_Test_Success_Return_True()
        {
            //Arrange
            var brokerAccount = new BrokerAccount
            {
                Id = 1,
                AccountName = "Test Brker",
                Funds = 100M,
                EquitieHolds = new List<EquitieHold>()
            };
            _mockBrokerAccountRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(brokerAccount);

            //Act
            var result = await _brokerService.IsBrokerIdValid(brokerAccount.Id);

            //Assert
            Assert.True(result);

            //verify each call
            _mockBrokerAccountRepository.Verify(mock => mock.GetByID(brokerAccount.Id), Times.Once());
        }

        [Fact]
        public async Task IsBrokerIdValid_Test_Fail_Return_False()
        {
            //Arrange
            _mockBrokerAccountRepository.Setup(x => x.GetByID(It.IsAny<int>()));

            //Act
            var result = await _brokerService.IsBrokerIdValid(5);

            //Assert
            Assert.False(result);

            //verify each call
            _mockBrokerAccountRepository.Verify(mock => mock.GetByID(It.IsAny<int>()), Times.Once());
        }

        [Theory]
        [InlineData(100, 100, 200)]
        [InlineData(100, 200000, 200000)]
        public async Task AddFound_Test_Success_Return_True(decimal initialFund, decimal addFund, decimal expected)
        {
            //Arrange
            var brokerAccount = new BrokerAccount
            {
                Id = 1,
                AccountName = "Test Brker",
                Funds = initialFund,
                EquitieHolds = new List<EquitieHold>()
            };
            _mockBrokerAccountRepository.Setup(x => x.GetByID(It.IsAny<int>())).ReturnsAsync(brokerAccount);
            _mockBrokerAccountRepository.Setup(x => x.Save()).ReturnsAsync(1);
            //Act
            var result = await _brokerService.AddFound(brokerAccount.Id, addFund);

            //Assert
            Assert.True(result);
            Assert.Equal(brokerAccount.Funds, expected);

            //verify each call
            _mockBrokerAccountRepository.Verify(mock => mock.GetByID(brokerAccount.Id), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.Update(brokerAccount), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.Save(), Times.Once());
        }

        [Fact]
        public async Task AddFound_Test_Fail_Return_False()
        {
            //Arrange
            _mockBrokerAccountRepository.Setup(x => x.GetByID(It.IsAny<int>()));
            //Act
            var result = await _brokerService.AddFound(1, 100);

            //Assert
            Assert.False(result);

            //verify each call
            _mockBrokerAccountRepository.Verify(mock => mock.GetByID(1), Times.Once());
            _mockBrokerAccountRepository.Verify(mock => mock.Update(It.IsAny<BrokerAccount>()), Times.Never());
            _mockBrokerAccountRepository.Verify(mock => mock.Save(), Times.Never());
        }
    }
}
