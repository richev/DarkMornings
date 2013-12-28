using System;
using System.Collections.Generic;
using System.Linq;
using Richev.DarkMornings.Web.Models;

namespace Richev.DarkMornings.Web
{
    public static class Builders
    {
        public static DaylightInfo BuildDaylights(DateTime today, Core.DaylightInfo daylightInfo, Commute commuteType, WorkingDays workingDays)
        {
            int daysToTransition = 0;

            if (daylightInfo.NextDaylightTransition.HasValue)
            {
                var days = new List<DateTime>();

                // in the same order (Sunday first) as the System.DayOfWeek enumeration
                var workingDayBools = new[] { workingDays.Sunday, workingDays.Monday, workingDays.Tuesday, workingDays.Wednesday, workingDays.Thursday, workingDays.Friday, workingDays.Saturday };

                var day = today;
                while (day < daylightInfo.NextDaylightTransition.Value)
                {
                    day = day.AddDays(1);
                    days.Add(day);
                }

                // TODO: Get this sorted out

                switch (daylightInfo.TransitionType)
                {
                    case Core.DaylightTransition.SunRise:
                        while (!workingDayBools[(int)days.Last().DayOfWeek])
                        {
                            days.Add(days.Last().AddDays(1));
                        }
                        break;

                    case Core.DaylightTransition.SunSet:
                        while (!workingDayBools[(int)days.Last().DayOfWeek])
                        {
                            days.RemoveAt(days.Count - 1);
                        }
                        break;
                }

                daysToTransition = days.Count(d => workingDayBools[(int)d.DayOfWeek]);
            }

            return new DaylightInfo
            {
                IsCurrentlyInDaylight = daylightInfo.IsCurrentlyInDaylight,
                NextWorkingDayDaylightTransition = daylightInfo.NextDaylightTransition, // not quite!
                CommuteType = commuteType,
                NumberOfDaysToTransition = daysToTransition
            };
        }
    }
}