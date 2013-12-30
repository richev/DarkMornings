using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Richev.DarkMornings.Web
{
    public static class UIHelpers
    {
        public static List<SelectListItem> GetHours(int selectedHour)
        {
            var hours = new List<SelectListItem>();

            for (var h = 0; h < 24; h++)
            {
                hours.Add(new SelectListItem
                              {
                                  Text = h.ToString("00"),
                                  Value = h.ToString(),
                                  Selected = h == selectedHour
                              });
            }

            return hours;
        }

        public static List<SelectListItem> GetMinutes(int selectedMinute)
        {
            var minutes = new List<SelectListItem>();

            for (var m = 0; m < 60; m += 5)
            {
                minutes.Add(new SelectListItem
                                {
                                    Text = m.ToString("00"),
                                    Value = m.ToString(),
                                    Selected = m == selectedMinute
                                });
            }

            return minutes;
        }

        public static HtmlString GetJourneyText(Models.DaylightInfo daylightInfo)
        {
            string journeyText;

            if (daylightInfo.NextWorkingDayDaylightTransition.HasValue)
            {
                journeyText = string.Format(
                    "<span class=\"days-more\"><span class=\"count\">{0} more</span> {1} {2}</span><span class=\"until\"><span>until</span></span><span class=\"calendar\"><span class=\"day\">{3:dddd}</span> <span class=\"date\">{3:%d}</span> <span class=\"month\">{3:MMMM}</span></span>",
                    daylightInfo.NumberOfDaysToTransition,
                    GetDaylightText(daylightInfo.IsCurrentlyInDaylight),
                    GetCommuteText(daylightInfo.CommuteType),
                    daylightInfo.NextWorkingDayDaylightTransition.Value);
            }
            else
            {
                journeyText = string.Format(
                    "<span>Your journey {0} is always in the <strong>{1}</strong></span>",
                    GetCommuteText(daylightInfo.CommuteType),
                    daylightInfo.IsCurrentlyInDaylight ? "light <i class=\"fa fa-sun-o\"></i>" : "dark <i class=\"fa fa-moon-o\"></i>");
            }

            return new HtmlString(journeyText);
        }

        private static string GetDaylightText(bool isCurrentlyInDaylight)
        {
            return isCurrentlyInDaylight ? "light journeys<i class=\"fa fa-sun-o\"></i>" : "dark journeys<i class=\"fa fa-moon-o\"></i>";
        }

        private static string GetCommuteText(Commute commuteType)
        {
            switch (commuteType)
            {
                case Commute.Outbound:
                    return "to work";

                case Commute.Return:
                    return "from work";

                default:
                    throw new ArgumentOutOfRangeException("commuteType");
            }
        }
    }
}