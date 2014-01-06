using System;

namespace Richev.DarkMornings.Core
{
    public static class Utils
    {
        public static double CalculateLongitudeTimeZone(double timeZone)
        {
            return timeZone * 15;
        }

        public static DateTime UtcToUserTimeZone(DateTime dateTime, double timeZoneOffset)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            var userDateTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);

            return userDateTime;
        }

        public static bool IsGmtDaylightSavingTime(DateTime dateTime)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time").IsDaylightSavingTime(dateTime);
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
