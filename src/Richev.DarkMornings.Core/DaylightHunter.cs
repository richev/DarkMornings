using System;

namespace Richev.DarkMornings.Core
{
    public class DaylightHunter
    {
        /// <summary>
        /// Used as a bailout value
        /// </summary>
        private const int DaysInYear = 365;

        public CommuteInfo GetDaylight(Location location, double timeZone, DateTime outboundCommuteStart, DateTime outboundCommuteEnd, DateTime returnCommuteStart, DateTime returnCommuteEnd)
        {
            var sunCalculator = new SunCalculator(
                location.Longitude,
                location.Latitude,
                Utils.CalculateLongitudeTimeZone(timeZone));

            var commuteInfo = new CommuteInfo();

            commuteInfo.ToWork = CalculateDaylightInfo(outboundCommuteStart, outboundCommuteEnd, sunCalculator);
            commuteInfo.FromWork = CalculateDaylightInfo(returnCommuteStart, returnCommuteEnd, sunCalculator);

            return commuteInfo;
        }

        private DaylightInfo CalculateDaylightInfo(DateTime commuteStart, DateTime commuteEnd, SunCalculator sunCalculator)
        {
            var daylightInfo = new DaylightInfo();

            var sunRise = sunCalculator.CalculateSunRise(commuteStart.Date);
            var sunSet = sunCalculator.CalculateSunSet(commuteStart.Date);
            var commuteIsAfterSunrise = commuteStart.TimeOfDay >= sunRise.TimeOfDay;
            var commuteIsBeforeSunSet = commuteEnd.TimeOfDay <= sunSet.TimeOfDay;

            daylightInfo.IsCurrentlyInDaylight = commuteIsAfterSunrise && commuteIsBeforeSunSet;

            var d = 0;

            while (!DaylightTransitionHappened(sunRise, sunSet, commuteStart, commuteEnd, daylightInfo.IsCurrentlyInDaylight) && d <= DaysInYear)
            {
                sunRise = sunCalculator.CalculateSunRise(commuteStart.Date.AddDays(++d));
                sunSet = sunCalculator.CalculateSunSet(commuteEnd.Date.AddDays(d));
            }
            if (DaylightTransitionHappened(sunRise, sunSet, commuteStart, commuteEnd, daylightInfo.IsCurrentlyInDaylight))
            {
                // We need to work out on which edge the transition happened
                if (commuteIsAfterSunrise != (commuteStart.TimeOfDay >= sunRise.TimeOfDay))
                {
                    daylightInfo.NextDaylightTransition = sunRise;
                    daylightInfo.TransitionType = DaylightTransition.SunRise;
                }
                else if (commuteIsBeforeSunSet != (commuteEnd.TimeOfDay <= sunSet.TimeOfDay))
                {
                    daylightInfo.NextDaylightTransition = sunSet;
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
