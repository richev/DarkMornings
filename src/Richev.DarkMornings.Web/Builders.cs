using System;
using System.Collections.Generic;
using System.Linq;
using Richev.DarkMornings.Core;
using Richev.DarkMornings.Web.Models;

namespace Richev.DarkMornings.Web
{
    public static class Builders
    {
        public static DaylightInfoModel BuildDaylightInfoModel(DateTime today, DaylightInfo daylightInfo, Commute commuteType, bool[] workingDays)
        {
            var daysToTransition = 0;
            DateTime? nextWorkingDayDaylightTransition = null;

            if (daylightInfo.NextDaylightTransition.HasValue)
            {
                var days = new List<DateTime>();

                var day = today;
                while (day < daylightInfo.NextDaylightTransition.Value)
                {
                    day = day.AddDays(1);
                    days.Add(day);
                }

                // Nudge to the first working day, if necessary
                while (!workingDays[(int)days.Last().DayOfWeek])
                {
                    days.Add(days.Last().AddDays(1));
                }

                daysToTransition = days.Count(d => workingDays[(int)d.DayOfWeek]);
                nextWorkingDayDaylightTransition = days.Last();
            }

            return new DaylightInfoModel
            {
                IsCurrentlyInDaylight = daylightInfo.IsCurrentlyInDaylight,
                NextWorkingDayDaylightTransition = nextWorkingDayDaylightTransition,
                CommuteType = commuteType,
                NumberOfDaysToTransition = daysToTransition
            };
        }

        public static CommuteTimeModel BuildEndCommuteTimeModel(CommuteTimeModel commuteTime, int journeyDuration)
        {
            var time = new TimeSpan(commuteTime.h, commuteTime.m, 0);

            var endCommuteTime = time.Add(new TimeSpan(0, journeyDuration, 0));

            return new CommuteTimeModel { h = endCommuteTime.Hours, m = endCommuteTime.Minutes };
        }
    }
}