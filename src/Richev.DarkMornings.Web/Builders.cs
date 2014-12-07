using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Richev.DarkMornings.Core;
using Richev.DarkMornings.Web.Models;

namespace Richev.DarkMornings.Web
{
    public static class Builders
    {
        public static DaylightInfoModel BuildDaylightInfoModel(DateTime today, DaylightInfo daylightInfo, Commute commuteType, WorkingDays workingDays)
        {
            var daysToTransition = 0;
            DateTime? nextWorkingDayDaylightTransition = null;

            if (daylightInfo.NextDaylightTransition.HasValue)
            {
                var day = today;

                if (day >= daylightInfo.NextDaylightTransition.Value)
                {
                    throw new InvalidOperationException(string.Format("daylightInfo.NextDaylightTransition.Value equals {0}; but it should be in the future.", daylightInfo.NextDaylightTransition.Value));
                }

                var days = new List<DateTime>();

                while (day < daylightInfo.NextDaylightTransition.Value)
                {
                    day = day.AddDays(1);
                    days.Add(day);
                }

                // Nudge to the first working day, if necessary
                // TODO: Get rid of the flags variable here
                var workingDayFlags = new[] { workingDays.HasFlag(WorkingDays.Sunday), workingDays.HasFlag(WorkingDays.Monday), workingDays.HasFlag(WorkingDays.Tuesday), workingDays.HasFlag(WorkingDays.Wednesday), workingDays.HasFlag(WorkingDays.Thursday), workingDays.HasFlag(WorkingDays.Friday), workingDays.HasFlag(WorkingDays.Saturday) };
                while (!workingDayFlags[(int)days.Last().DayOfWeek])
                {
                    days.Add(days.Last().AddDays(1));
                }

                daysToTransition = days.Count(d => workingDayFlags[(int)d.DayOfWeek]);
                nextWorkingDayDaylightTransition = days.Last();
            }

            return new DaylightInfoModel
            {
                IsCurrentlyInDaylight = daylightInfo.IsCurrentlyInDaylight,
                PercentOfTheYearInTheDark = GetPercentOfTheYearInTheDark(daylightInfo.CommutesInDaylightPerYear),
                NextWorkingDayDaylightTransition = nextWorkingDayDaylightTransition,
                CommuteType = commuteType,
                NumberOfDaysToTransition = daysToTransition,
            };
        }

        private static double GetPercentOfTheYearInTheDark(int commutesInDaylightPerYear)
        {
            var commutesInDarknessPerYear = DaylightHunter.DaysInYear - commutesInDaylightPerYear;
            var percentOfTheYearInTheDark = ((double)commutesInDarknessPerYear / DaylightHunter.DaysInYear) * 100;

            return percentOfTheYearInTheDark;
        }

        /*public static CommuteTimeModel BuildEndCommuteTimeModel(CommuteTimeModel commuteTime, int journeyDuration)
        {
            var time = new TimeSpan(commuteTime.h, commuteTime.m, 0);

            var endCommuteTime = time.Add(new TimeSpan(0, journeyDuration, 0));

            return new CommuteTimeModel { h = endCommuteTime.Hours, m = endCommuteTime.Minutes };
        }*/

        public static string BuildQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();

            return "?" + string.Join("&", array);
        }

        /*public static OtherLocationModel[] BuildOtherLocations(string workingDays, CommuteTimeModel toWork, CommuteTimeModel fromWork, int duration)
        {
            var otherLocations = new List<OtherLocationModel>();

            otherLocations.Add(new OtherLocationModel { City = "Copenhagen", Country = "Denmark", TimeZoneOffset = 1, Latitude = 55.716667, Longitude = 12.566667 });
            otherLocations.Add(new OtherLocationModel { City = "Melbourne", Country = "Australia", TimeZoneOffset = 10, Latitude = -37.813611, Longitude = 144.963056 });
            otherLocations.Add(new OtherLocationModel { City = "Helsinki", Country = "Finland", TimeZoneOffset = 2, Latitude = 60.170833, Longitude = 24.9375 });
            otherLocations.Add(new OtherLocationModel { City = "Tokyo", Country = "Japan", TimeZoneOffset = 9, Latitude = 35.689506, Longitude = 139.6917 });
            otherLocations.Add(new OtherLocationModel { City = "Vienna", Country = "Austria", TimeZoneOffset = 1, Latitude = 48.2, Longitude = 16.366667 });
            otherLocations.Add(new OtherLocationModel { City = "Zurich", Country = "Switzerland", TimeZoneOffset = 1, Latitude = 47.366667, Longitude = 8.55 });
            otherLocations.Add(new OtherLocationModel { City = "Stockholm", Country = "Sweden", TimeZoneOffset = 1, Latitude = 59.329444, Longitude = 18.068611 });
            otherLocations.Add(new OtherLocationModel { City = "Munich", Country = "Germany", TimeZoneOffset = 1, Latitude = 48.133333, Longitude = 11.566667 });
            otherLocations.Add(new OtherLocationModel { City = "Sydney", Country = "Australia", TimeZoneOffset = 10, Latitude = -33.859972, Longitude = 151.211111 });
            otherLocations.Add(new OtherLocationModel { City = "Auckland", Country = "New Zealand", TimeZoneOffset = 12, Latitude = -36.840417, Longitude = 174.739869 });

            var today = DateTime.Now.Date;

            var outboundCommuteStart = today.AddHours(toWork.h).AddMinutes(toWork.m);
            var returnCommuteStart = today.AddHours(fromWork.h).AddMinutes(fromWork.m);

            foreach (var otherLocation in otherLocations)
            {
                // TODO: Refactor from code in HomeController
                var daylightHunter = new DaylightHunter();

                var outboundCommuteEnd = outboundCommuteStart.AddMinutes(duration);
                var returnCommuteEnd = returnCommuteStart.AddMinutes(duration);
                
                var location = new Location { Latitude = otherLocation.Latitude, Longitude = otherLocation.Longitude };

                var toWorkDaylightInfo = daylightHunter.GetDaylight(location, otherLocation.TimeZoneOffset, outboundCommuteStart, outboundCommuteEnd);
                var fromWorkDaylightInfo = daylightHunter.GetDaylight(location, otherLocation.TimeZoneOffset, returnCommuteStart, returnCommuteEnd);

                var workingDaysBools = workingDays.Select(d => d == UIHelpers.WorkingDayTrue).ToArray();

                otherLocation.ToWorkDaylights = BuildDaylightInfoModel(DateTime.Now.Date, toWorkDaylightInfo, Commute.ToWork, workingDaysBools);
                otherLocation.FromWorkDaylights = BuildDaylightInfoModel(DateTime.Now.Date, fromWorkDaylightInfo, Commute.FromWork, workingDaysBools);
            }

            return otherLocations.ToArray();
        }*/
    }
}