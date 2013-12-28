using System;
using System.Collections.Generic;
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

        public static string GetCommuteText(Commute commuteType)
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