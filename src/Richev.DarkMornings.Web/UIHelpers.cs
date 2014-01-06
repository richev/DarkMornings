using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Richev.DarkMornings.Web.Models;

namespace Richev.DarkMornings.Web
{
    public static class UIHelpers
    {
        /// <summary>
        /// <para>Character 'x', used in a Sunday-first seven-character string to indicate a working day</para>
        /// <para>e.g. oxxxxxo indicates that Mon-Fri are working days.</para>
        /// </summary>
        public const char WorkingDayTrue = 'x';

        public const char WorkingDayFalse = 'o';

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

        public static HtmlString GetJourneyText(DaylightInfo daylightInfo)
        {
            string journeyText;

            if (daylightInfo.NextWorkingDayDaylightTransition.HasValue)
            {
                journeyText = string.Format(
@"<div class=""days-more"">
    {0}
    <div class=""days-more-content"">
        {1}<span class=""count"">{2} more</span> <span>{3} journeys</span>
    </div>
</div>
<div class=""until"">
    <span>{4},</span> until
</div>
<div class=""calendar"">
    <span class=""day"">{5:dddd}</span>
    <span class=""date"">{5:%d}</span>
    <span class=""month"">{5:MMMM}</span>
</div>",
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
@"<div class=""days-more always"">
    {0}
    <div class=""days-more-content always"">
        {1}Your journey {2} is always in the {3}
    </div>
</div>",
                    GetDaylightIcon(daylightInfo.IsCurrentlyInDaylight),
                    GetCommuteDirectionIcon(daylightInfo.CommuteType),
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
                case Commute.ToWork:
                    return "<i class=\"fa fa-arrow-circle-o-right commute-direction\"></i>";

                case Commute.FromWork:
                    return "<i class=\"fa fa-arrow-circle-o-left commute-direction\"></i>";

                default:
                    throw new ArgumentOutOfRangeException("commuteType");
            }
        }

        private static string GetCommuteText(Commute commuteType)
        {
            switch (commuteType)
            {
                case Commute.ToWork:
                    return "to work";

                case Commute.FromWork:
                    return "from work";

                default:
                    throw new ArgumentOutOfRangeException("commuteType");
            }
        }

        public static string FormatCommuteTime(CommuteTime commuteTime)
        {
            return string.Format("{0:00}:{1:00}", commuteTime.h, commuteTime.m);
        }

        public static string FormatWorkingDays(bool[] workingDays)
        {
            var setWorkingDayNames = new List<string>();

            var dayNames = Enum.GetNames(typeof(DayOfWeek));

            if (workingDays.Length != dayNames.Length)
            {
                throw new InvalidOperationException();
            }

            for (int i = 0; i < dayNames.Length; i++)
            {
                if (workingDays[i])
                {
                    setWorkingDayNames.Add(dayNames[i]);
                }
            }

            var formattedWorkingDays = string.Join(", ", setWorkingDayNames);

            if (setWorkingDayNames.Count > 1)
            {
                formattedWorkingDays = ReplaceLastOccurrence(formattedWorkingDays, ", ", " and ");
            }

            return formattedWorkingDays;
        }

        private static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            var place = source.LastIndexOf(find, StringComparison.InvariantCulture);
            var result = source.Remove(place, find.Length).Insert(place, replace);

            return result;
        }

        public static string FormatDaysCount(string workingDays)
        {
            var workingDaysCount = workingDays.Count(d => d == WorkingDayTrue);

            return string.Format("{0} day{1}", workingDaysCount, workingDaysCount == 1 ? " " : "s");
        }

        public static string GetTweetText(CommuteInfoModel model)
        {
            var tweetText = string.Format(
                "I have {0} more {1} journeys to work and {2} more {3} journeys from work!",
                model.tw.Daylights.NumberOfDaysToTransition,
                GetDaylightText(model.tw.Daylights.IsCurrentlyInDaylight),
                model.fw.Daylights.NumberOfDaysToTransition,
                GetDaylightText(model.fw.Daylights.IsCurrentlyInDaylight));

            return tweetText;
        }
    }
}