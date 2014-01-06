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

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");

            var userDateTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);

            return DateTime.SpecifyKind(userDateTime, DateTimeKind.Local);
        }
    }
}
