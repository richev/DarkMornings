using System;

namespace Richev.DarkMornings.Core
{
    public static class Utils
    {
        public static double CalculateLongitudeTimeZone(double timeZoneOffset)
        {
            return timeZoneOffset * 15;
        }

        public static DateTime UtcToUserTimeZone(DateTime dateTime, string timeZoneId)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            var userDateTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);

            return userDateTime;
        }

        public static bool IsGmtDaylightSavingTime(DateTime dateTime, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            return timeZone.IsDaylightSavingTime(dateTime);
        }

        public static TimeSpan GetTimeOfDayDifference(DateTime d1, DateTime d2)
        {
            if (d1.TimeOfDay.Ticks > d2.TimeOfDay.Ticks)
            {
                return d1 - d2;
            }

            return d2 - d1;
        }
    }
}
