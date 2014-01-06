using System;

namespace Richev.DarkMornings.Core
{
    public static class Utils
    {
        public static double CalculateLongitudeTimeZone(double timeZoneOffset)
        {
            return timeZoneOffset * 15;
        }

        public static DateTime UtcToUserTimeZone(DateTime dateTime, double timeZoneOffset)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            var timeZoneName = TimeZones.Selected[timeZoneOffset];

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);

            var userDateTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);

            return userDateTime;
        }

        public static bool IsGmtDaylightSavingTime(DateTime dateTime, double timeZoneOffset)
        {
            var timeZoneName = TimeZones.Selected[timeZoneOffset];

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);

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


        public static void AllTimeZones()
        {
            var tzCollection = TimeZoneInfo.GetSystemTimeZones();
        }
    }
}
