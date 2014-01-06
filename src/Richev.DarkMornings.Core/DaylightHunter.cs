using System;

namespace Richev.DarkMornings.Core
{
    public class DaylightHunter
    {
        /// <summary>
        /// Used as a bailout value
        /// </summary>
        private const int DaysInYear = 365;

        public CommuteInfo GetDaylight(Location location, double timeZone, DateTime outboundCommuteAt, DateTime returnCommuteAt)
        {
            var sunCalculator = new SunCalculator(
                location.Longitude,
                location.Latitude,
                Utils.CalculateLongitudeTimeZone(timeZone));

            outboundCommuteAt = Utils.UtcToUserTimeZone(outboundCommuteAt, timeZone);
            returnCommuteAt = Utils.UtcToUserTimeZone(returnCommuteAt, timeZone);

            var commuteInfo = new CommuteInfo();

            commuteInfo.ToWork = CalculateDaylightInfo(outboundCommuteAt, sunCalculator);
            commuteInfo.FromWork = CalculateDaylightInfo(returnCommuteAt, sunCalculator);

            return commuteInfo;
        }

        private DaylightInfo CalculateDaylightInfo(DateTime commuteAt, SunCalculator sunCalculator)
        {
            var daylightInfo = new DaylightInfo();

            var sunRise = sunCalculator.CalculateSunRise(commuteAt);
            var sunSet = sunCalculator.CalculateSunSet(commuteAt);
            var commuteIsAfterSunrise = commuteAt.TimeOfDay >= sunRise.TimeOfDay;
            var commuteIsBeforeSunSet = commuteAt.TimeOfDay <= sunSet.TimeOfDay;

            daylightInfo.IsCurrentlyInDaylight = commuteIsAfterSunrise && commuteIsBeforeSunSet;

            var d = 0;

            while (!DaylightTransitionHappened(sunRise, sunSet, commuteAt, daylightInfo.IsCurrentlyInDaylight) && d <= DaysInYear)
            {
                sunRise = sunCalculator.CalculateSunRise(commuteAt.AddDays(++d));
                sunSet = sunCalculator.CalculateSunSet(commuteAt.AddDays(d));
            }
            if (DaylightTransitionHappened(sunRise, sunSet, commuteAt, daylightInfo.IsCurrentlyInDaylight))
            {
                // We need to work out on which edge the transition happened
                if (commuteIsAfterSunrise != (commuteAt.TimeOfDay >= sunRise.TimeOfDay))
                {
                    daylightInfo.NextDaylightTransition = sunRise;
                    daylightInfo.TransitionType = DaylightTransition.SunRise;
                }
                else if (commuteIsBeforeSunSet != (commuteAt.TimeOfDay <= sunSet.TimeOfDay))
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

        private bool DaylightTransitionHappened(DateTime sunRise, DateTime sunSet, DateTime commuteAt, bool startedInDaylight)
        {
            var nowInDayLight = commuteAt.TimeOfDay >= sunRise.TimeOfDay && commuteAt.TimeOfDay <= sunSet.TimeOfDay;

            return startedInDaylight != nowInDayLight;
        }
    }
}
