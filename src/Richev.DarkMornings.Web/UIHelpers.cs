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
                    "You have <strong>{0} more</strong> {1} journeys {2}, until <strong>{3}</strong>.",
                    daylightInfo.NumberOfDaysToTransition,
                    daylightInfo.IsCurrentlyInDaylight ? "light" : "dark",
                    GetCommuteText(daylightInfo.CommuteType),
                    daylightInfo.NextWorkingDayDaylightTransition.Value.ToString("dddd d MMMM"));
            }
            else
            {
                journeyText = string.Format(
                    "Your journey {0} is always in the <strong>{1}</strong>.",
                    GetCommuteText(daylightInfo.CommuteType), daylightInfo.IsCurrentlyInDaylight ? "light" : "dark");
            }

            return new HtmlString(string.Format("<p>{0}</p>", journeyText));
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