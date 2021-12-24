using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nagp.eBroker.Data;
using Nagp.eBroker.Data.Entities;
using Nagp.eBroker.Data.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nagp.eBroker.UnitTests.Repositories
{
    public class BrokerAccountRepositoryUnitTest
    {
        //SUT
        readonly IBrokerAccountRepository _brokerAccountRepository;
        readonly EBrokerContext context;

        public BrokerAccountRepositoryUnitTest()
        {
            context = MockContextHelper.Instance.GetMockContextPostgres();
            _brokerAccountRepository = new BrokerAccountRepository(context);
        }

        [Fact]
        public async Task<int> Insert_Test_Success()
        {
            //Arrange
            var brokerAccount = new BrokerAccount
            {
                AccountName = "Test Account",
                Funds = 100M
            };

            //Act
            await _brokerAccountRepository.Insert(brokerAccount);
            await _brokerAccountRepository.Save();
            //Assert
            brokerAccount.Id.Should().BeGreaterThan(0);
            return brokerAccount.Id;
        }

        [Fact]
        public async Task Update_Test_Success()
        {
            //Arrange
            var brokerId = await Insert_Test_Success();
            var brokerAccount = await _brokerAccountRepository.GetByID(brokerId);

            //Act
            brokerAccount.AccountName = "Testing";
            _brokerAccountRepository.Update(brokerAccount);
            var affectedRow = await _brokerAccountRepository.Save();

            //Assert
            affectedRow.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Delete_By_Id_Test_Success()
        {
            //Arrange
            var brokerId = await Insert_Test_Success();

            //Act
            await _brokerAccountRepository.Delete(brokerId);
            var affectedRow = await _brokerAccountRepository.Save();

            //Assert
            affectedRow.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Delete_Test_Success()
        {
            //Arrange
            var brokerId = await Insert_Test_Success();
            var brokerAccount = await _brokerAccountRepository.GetByID(brokerId);
            context.Entry(brokerAccount).State = EntityState.Detached;
            //Act

            _brokerAccountRepository.Delete(brokerAccount);
            var affectedRow = await _brokerAccountRepository.Save();

            //Assert
            affectedRow.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_No_Filter_Test_Success()
        {
            //Arrange
            await Insert_Test_Success();
            //Act

            var brokerAccounts = await _brokerAccountRepository.Get();

            //Assert
            brokerAccounts.Should().NotBeNull();
            brokerAccounts.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_No_IncludeProperties_Test_Success()
        {
            //Arrange
            await Insert_Test_Success();
            //Act

            var brokerAccounts = await _brokerAccountRepository.Get(includeProperties: nameof(BrokerAccount.EquitieHolds));

            //Assert
            brokerAccounts.Should().NotBeNull();
            brokerAccounts.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_No_Filter_OrderBy_Test_Success()
        {
            //Arrange
            await Insert_Test_Success();
            //Act

            var brokerAccounts = await _brokerAccountRepository.Get(orderBy: b => b.OrderBy(x => x.Id));

            //Assert
            brokerAccounts.Should().NotBeNull();
            brokerAccounts.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_Filter_Test_Success()
        {
            //Arrange
            var brokerId = await Insert_Test_Success();
            //Act

            var brokerAccounts = await _brokerAccountRepository.Get(f => f.Id == brokerId);

            //Assert
            brokerAccounts.Should().NotBeNull();
            brokerAccounts.Count.Should().BeGreaterThan(0);
        }
    }
}
