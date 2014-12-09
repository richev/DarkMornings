using System;

namespace Richev.DarkMornings.Core
{
    public class DaylightHunter
    {
        public const int DaysInYear = 365;

        public DaylightInfo GetDaylight(Location location, string timeZoneId, DateTime commuteStart, DateTime commuteEnd)
        {
            var sunCalculator = new SunCalculator(location.Longitude, location.Latitude);

            var daylightInfo = CalculateDaylightInfo(commuteStart, commuteEnd, sunCalculator, timeZoneId);

            return daylightInfo;
        }

        private DaylightInfo CalculateDaylightInfo(DateTime commuteStart, DateTime commuteEnd, SunCalculator sunCalculator, string timeZoneId)
        {
            var daylightInfo = new DaylightInfo();

            var sunRise = sunCalculator.CalculateSunRise(commuteStart.Date, timeZoneId);
            var sunSet = sunCalculator.CalculateSunSet(commuteStart.Date, timeZoneId);
            var commuteIsAfterSunrise = commuteStart.TimeOfDay >= sunRise.TimeOfDay;
            var commuteIsBeforeSunSet = commuteEnd.TimeOfDay <= sunSet.TimeOfDay;

            if (commuteEnd.Date > commuteStart.Date)
            {
                // Special case; overlapping midnight. Just assume that it's always dark
                return daylightInfo;
            }

            daylightInfo.IsCurrentlyInDaylight = commuteIsAfterSunrise && commuteIsBeforeSunSet;

            for (var d = 0; d < DaysInYear; d++)
            {
                sunRise = sunCalculator.CalculateSunRise(commuteStart.Date.AddDays(d), timeZoneId);
                sunSet = sunCalculator.CalculateSunSet(commuteEnd.Date.AddDays(d), timeZoneId);

                if (commuteStart.TimeOfDay >= sunRise.TimeOfDay && commuteEnd.TimeOfDay <= sunSet.TimeOfDay)
                {
                    daylightInfo.CommutesInDaylightPerYear++;
                }

                if (!daylightInfo.TransitionType.HasValue && DaylightTransitionHappened(sunRise, sunSet, commuteStart, commuteEnd, daylightInfo.IsCurrentlyInDaylight))
                {
                    // We need to work out on which edge the transition happened
                    if (commuteIsAfterSunrise != (commuteStart.TimeOfDay >= sunRise.TimeOfDay))
                    {
                        daylightInfo.NextDaylightTransition = sunRise.AddDays(-1);
                        daylightInfo.TransitionType = DaylightTransition.SunRise;
                    }
                    else if (commuteIsBeforeSunSet != (commuteEnd.TimeOfDay <= sunSet.TimeOfDay))
                    {
                        daylightInfo.NextDaylightTransition = sunSet.AddDays(-1);
                        daylightInfo.TransitionType = DaylightTransition.SunSet;
                    }
                    else
                    {
                        throw new Exception("something went very wrong");
                    }
                }
            }

            return daylightInfo;
        }

        private bool DaylightTransitionHappened(DateTime sunRise, DateTime sunSet, DateTime commuteStart, DateTime commuteEnd, bool wasInDaylight)
        {
            var nowInDayLight = commuteStart.TimeOfDay >= sunRise.TimeOfDay && commuteEnd.TimeOfDay <= sunSet.TimeOfDay;

            return wasInDaylight != nowInDayLight;
        }
    }
}
