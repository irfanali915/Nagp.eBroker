using Nagp.eBroker.Helper;
using System;
using Xunit;

namespace Nagp.eBroker.UnitTests.Helpers
{
    public class DateTimeProviderUnitTest
    {
        //SUT
        readonly IDateTimeProvider _dateTimeProvider;

        public DateTimeProviderUnitTest()
        {
            _dateTimeProvider = new DateTimeProvider();
        }

        [Fact]
        public void GetNow_Test()
        {
            //Arrange

            //Act
            var result = _dateTimeProvider.GetNow();

            //Assert
            Assert.IsType<DateTime>(result);
        }
    }
}
