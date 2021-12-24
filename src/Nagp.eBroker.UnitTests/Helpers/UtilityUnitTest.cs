using Nagp.eBroker.Helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nagp.eBroker.UnitTests.Helpers
{
    public class UtilityUnitTest
    {
        public static IEnumerable<object[]> GeTimeRange()
        {
            yield return new object[] { new DateTime(2021, 12, 24, 10, 0, 0), 9, 15, true };
            yield return new object[] { new DateTime(2021, 12, 24, 17, 0, 0), 9, 16, false };
            yield return new object[] { new DateTime(2021, 12, 24, 8, 0, 0), 10, 15, false };
        }

        public static IEnumerable<object[]> GeDayRange()
        {
            yield return new object[] { new DateTime(2021, 12, 24, 10, 0, 0), DayOfWeek.Monday, DayOfWeek.Friday, true };
            yield return new object[] { new DateTime(2021, 12, 25, 17, 0, 0), DayOfWeek.Sunday, DayOfWeek.Friday, false };
            yield return new object[] { new DateTime(2021, 12, 26, 8, 0, 0), DayOfWeek.Monday, DayOfWeek.Saturday, false };
        }

        [Theory]
        [MemberData(nameof(GeTimeRange))]
        public void IsInTimeRange_Test(DateTime dateTime, int startTime, int endTime, bool expected)
        {
            //Arrange

            //Act
            var result = Utility.IsInTimeRange(dateTime, startTime, endTime);

            //Assert
            Assert.Equal(result, expected);
        }

        [Theory]
        [MemberData(nameof(GeDayRange))]
        public void IsInDayRange_Test(DateTime dateTime, DayOfWeek startDay, DayOfWeek endDay, bool expected)
        {
            //Arrange

            //Act
            var result = Utility.IsInDayRange(dateTime, startDay, endDay);

            //Assert
            Assert.Equal(result, expected);
        }
    }
}
