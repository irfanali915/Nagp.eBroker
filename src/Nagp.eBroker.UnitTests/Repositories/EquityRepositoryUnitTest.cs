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
    public class EquityRepositoryUnitTest
    {
        //SUT
        readonly IEquityRepository _equityRepository;
        readonly EBrokerContext context;

        public EquityRepositoryUnitTest()
        {
            context = MockContextHelper.Instance.GetMockContextPostgres();
            _equityRepository = new EquityRepository(context);
        }

        [Fact]
        public async Task<int> Insert_Test_Success()
        {
            //Arrange
            var equity = new Equity
            {
                Name = "Test equity",
                PricePerUnit = 100M,
                Units = 10
            };

            //Act
            await _equityRepository.Insert(equity);
            await _equityRepository.Save();
            //Assert
            equity.Id.Should().BeGreaterThan(0);
            return equity.Id;
        }

        [Fact]
        public async Task Update_Test_Success()
        {
            //Arrange
            var equityId = await Insert_Test_Success();
            var equity = await _equityRepository.GetByID(equityId);

            //Act
            equity.Name = "Testing";
            _equityRepository.Update(equity);
            var affectedRow = await _equityRepository.Save();

            //Assert
            affectedRow.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Delete_By_Id_Test_Success()
        {
            //Arrange
            var equityId = await Insert_Test_Success();

            //Act
            await _equityRepository.Delete(equityId);
            var affectedRow = await _equityRepository.Save();

            //Assert
            affectedRow.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Delete_Test_Success()
        {
            //Arrange
            var equityId = await Insert_Test_Success();
            var equitie = await _equityRepository.GetByID(equityId);
            context.Entry(equitie).State = EntityState.Detached;
            //Act

            _equityRepository.Delete(equitie);
            var affectedRow = await _equityRepository.Save();

            //Assert
            affectedRow.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_No_Filter_Test_Success()
        {
            //Arrange
            await Insert_Test_Success();
            //Act

            var equities = await _equityRepository.Get();

            //Assert
            equities.Should().NotBeNull();
            equities.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_No_Filter_OrderBy_Test_Success()
        {
            //Arrange
            await Insert_Test_Success();
            //Act

            var equities = await _equityRepository.Get(orderBy: b => b.OrderBy(x => x.Id));

            //Assert
            equities.Should().NotBeNull();
            equities.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Get_Filter_Test_Success()
        {
            //Arrange
            var equityId = await Insert_Test_Success();
            //Act

            var equities = await _equityRepository.Get(f => f.Id == equityId);

            //Assert
            equities.Should().NotBeNull();
            equities.Count.Should().BeGreaterThan(0);
        }
    }
}
