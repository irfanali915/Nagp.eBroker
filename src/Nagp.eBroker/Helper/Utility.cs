using System;

namespace Nagp.eBroker.Helper
{
    public static class Utility
    {
        public static bool IsInTimeRange(DateTime dateTime, int startTime, int endTime)
        {
            var startTimeSpan = TimeSpan.FromHours(startTime);
            var endTimeSpan = TimeSpan.FromHours(endTime);
            return dateTime.TimeOfDay >= startTimeSpan && dateTime.TimeOfDay <= endTimeSpan;
        }

        public static bool IsInDayRange(DateTime dateTime, DayOfWeek startDay, DayOfWeek endDay)
         => dateTime.DayOfWeek >= startDay && dateTime.DayOfWeek <= endDay;
    }
}
