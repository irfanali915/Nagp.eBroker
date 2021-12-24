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
    public class EquitieHoldRepositoryUnitTest
    {
        //SUT
        readonly IEquitieHoldRepository _equitieHoldRepository;

        readonly EBrokerContext context;

        public EquitieHoldRepositoryUnitTest()
        {
            context = MockContextHelper.Instance.GetMockContextPostgres();
            _equitieHoldRepository = new EquitieHoldRepository(context);
        }

        [Fact]
        public async Task<(int brokerId, int equityId)> Insert_Test_Success()
        {
            //Arrange
            var equityHold = new EquitieHold
            {
                BrokerAccount = new BrokerAccount
                {
                    AccountName = "Test Account",
                    Funds = 100M
                },
                Equity = new Equity
                {
                    Name = "Test Equity",
                    PricePerUnit = 100M,
                    Units = 10
                },
                HoldUnits = 2
            };

            //Act
            await _equitieHoldRepository.Insert(equityHold);
            await _equitieHoldRepository.Save();

            //Assert
            equityHold.BrokerAccountId.Should().BeGreaterThan(0);
            equityHold.EquityId.Should().BeGreaterThan(0);
            return (equityHold.BrokerAccountId, equityHold.EquityId);
        }

        [Fact]
        public async Task Update_Test_Success()
        {
            //Arrange
            (var brokerId, var equityId) = await Insert_Test_Success();
            var equityHold = await _equitieHoldRepository.GetByID(brokerId, equityId);

            //Act
            equityHold.HoldUnits = 10;
            _equitieHoldRepository.Update(equityHold);
            var affectedRow = await _equitieHoldRepository.Save();

            //Assert
            affectedRow.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Delete_By_Id_Test_Success()
        {
            //Arrange
            (var brokerId, var equityId) = await Insert_Test_Success();

            //Act
            await _equitieHoldRepository.Delete(brokerId, equityId);
            var affectedRow = await _equitieHoldRepository.Save();

            //Assert
            affectedRow.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Delete_Test_Success()
        {
            //Arrange
            (var brokerId, var equityId) = await Insert_Test_Success();
            var equitieHold = await _equitieHoldRepository.GetByID(brokerId, equityId);
            context.Entry(equitieHold).State = EntityState.Detached;
            //Act

            _equitieHoldRepository.Delete(equitieHold);
            var affectedRow = await _equitieHoldRepository.Save();

            //Assert
            affectedRow.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_No_Filter_Test_Success()
        {
            //Arrange
            await Insert_Test_Success();
            //Act

            var equitieHolds = await _equitieHoldRepository.Get();

            //Assert
            equitieHolds.Should().NotBeNull();
            equitieHolds.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_No_IncludeProperties_Test_Success()
        {
            //Arrange
            await Insert_Test_Success();
            //Act

            var equitieHolds = await _equitieHoldRepository.Get(includeProperties: $"{nameof(EquitieHold.BrokerAccount)},{nameof(EquitieHold.Equity)}");

            //Assert
            equitieHolds.Should().NotBeNull();
            equitieHolds.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_No_Filter_OrderBy_Test_Success()
        {
            //Arrange
            await Insert_Test_Success();
            //Act

            var equitieHolds = await _equitieHoldRepository.Get(orderBy: b => b.OrderBy(x => x.BrokerAccountId));

            //Assert
            equitieHolds.Should().NotBeNull();
            equitieHolds.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_Filter_Test_Success()
        {
            //Arrange
            (var brokerId, var equityId) = await Insert_Test_Success();
            //Act

            var equitieHolds = await _equitieHoldRepository.Get(f => f.BrokerAccountId == brokerId);

            //Assert
            equitieHolds.Should().NotBeNull();
            equitieHolds.Count.Should().BeGreaterThan(0);
        }
    }
}
