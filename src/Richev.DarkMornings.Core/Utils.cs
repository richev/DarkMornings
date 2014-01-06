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
    }
}
