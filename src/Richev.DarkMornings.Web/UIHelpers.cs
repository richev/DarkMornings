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
                    "<span class=\"days-more\">{0}{1}<span class=\"count\">{2} more</span><span>{3} journeys {4}</span></span><span class=\"until\"><span>until</span></span><span class=\"calendar\"><span class=\"day\">{5:dddd}</span> <span class=\"date\">{5:%d}</span> <span class=\"month\">{5:MMMM}</span></span>",
                    GetDaylightIcon(daylightInfo.IsCurrentlyInDaylight),
                    GetCommuteDirectionIcon(daylightInfo.CommuteType),
                    daylightInfo.NumberOfDaysToTransition,
                    GetDaylightText(daylightInfo.IsCurrentlyInDaylight),
                    GetCommuteText(daylightInfo.CommuteType),
                    daylightInfo.NextWorkingDayDaylightTransition.Value);
            }
            else
            {
                journeyText = string.Format(
                    "<span>{0}Your journey {1} is always in the <strong>{2}</strong></span>",
                    GetDaylightIcon(daylightInfo.IsCurrentlyInDaylight),
                    GetCommuteText(daylightInfo.CommuteType),
                    GetDaylightText(daylightInfo.IsCurrentlyInDaylight));
            }

            return new HtmlString(journeyText);
        }

        private static string GetDaylightIcon(bool isCurrentlyInDaylight)
        {
            return isCurrentlyInDaylight ? "<i class=\"fa fa-sun-o\"></i>" : "<i class=\"fa fa-moon-o\"></i>";
        }

        private static string GetDaylightText(bool isCurrentlyInDaylight)
        {
            return isCurrentlyInDaylight ? "light" : "dark";
        }

        private static string GetCommuteDirectionIcon(Commute commuteType)
        {
            switch (commuteType)
            {
                case Commute.Outbound:
                    return "<i class=\"fa fa-arrow-circle-o-right commute-direction\"></i>";

                case Commute.Return:
                    return "<i class=\"fa fa-arrow-circle-o-left commute-direction\"></i>";

                default:
                    throw new ArgumentOutOfRangeException("commuteType");
            }
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