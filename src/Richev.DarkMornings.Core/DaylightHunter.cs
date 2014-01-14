﻿using System;

namespace Richev.DarkMornings.Core
{
    public class DaylightHunter
    {
        /// <summary>
        /// Used as a bailout value
        /// </summary>
        private const int DaysInYear = 365;

        public DaylightInfo GetDaylight(Location location, double timeZoneOffset, DateTime commuteStart, DateTime commuteEnd)
        {
            var sunCalculator = new SunCalculator(
                location.Longitude,
                location.Latitude,
                Utils.CalculateLongitudeTimeZone(timeZoneOffset));

            var daylightInfo = CalculateDaylightInfo(commuteStart, commuteEnd, sunCalculator, timeZoneOffset);

            return daylightInfo;
        }

        private DaylightInfo CalculateDaylightInfo(DateTime commuteStart, DateTime commuteEnd, SunCalculator sunCalculator, double timeZoneOffset)
        {
            var daylightInfo = new DaylightInfo();

            var sunRise = sunCalculator.CalculateSunRise(commuteStart.Date, timeZoneOffset);
            var sunSet = sunCalculator.CalculateSunSet(commuteStart.Date, timeZoneOffset);
            var commuteIsAfterSunrise = commuteStart.TimeOfDay >= sunRise.TimeOfDay;
            var commuteIsBeforeSunSet = commuteEnd.TimeOfDay <= sunSet.TimeOfDay;

            if (commuteEnd.Date > commuteStart.Date)
            {
                // Special case; overlapping midnight. Just assume that it's always dark
                return daylightInfo;
            }

            daylightInfo.IsCurrentlyInDaylight = commuteIsAfterSunrise && commuteIsBeforeSunSet;

            var d = 0;

            while (!DaylightTransitionHappened(sunRise, sunSet, commuteStart, commuteEnd, daylightInfo.IsCurrentlyInDaylight) && d <= DaysInYear)
            {
                sunRise = sunCalculator.CalculateSunRise(commuteStart.Date.AddDays(++d), timeZoneOffset);
                sunSet = sunCalculator.CalculateSunSet(commuteEnd.Date.AddDays(d), timeZoneOffset);
            }
            if (DaylightTransitionHappened(sunRise, sunSet, commuteStart, commuteEnd, daylightInfo.IsCurrentlyInDaylight))
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

            return daylightInfo;
        }

        private bool DaylightTransitionHappened(DateTime sunRise, DateTime sunSet, DateTime commuteStart, DateTime commuteEnd, bool wasInDaylight)
        {
            var nowInDayLight = commuteStart.TimeOfDay >= sunRise.TimeOfDay && commuteEnd.TimeOfDay <= sunSet.TimeOfDay;

            return wasInDaylight != nowInDayLight;
        }
    }
}
