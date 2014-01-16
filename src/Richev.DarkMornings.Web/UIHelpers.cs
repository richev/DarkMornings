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

        public static List<SelectListItem> GetJourneyDurations(int selectedJourneyDuration)
        {
            var jds = new List<KeyValuePair<string, int>> 
                     {
                         new KeyValuePair<string, int>("5 minutes", 5),
                         new KeyValuePair<string, int>("10 minutes", 10),
                         new KeyValuePair<string, int>("15 minutes", 15),
                         new KeyValuePair<string, int>("30 minutes", 30),
                         new KeyValuePair<string, int>("45 minutes", 45),
                         new KeyValuePair<string, int>("1 hour", 60),
                         new KeyValuePair<string, int>("1½ hours", 90),
                         new KeyValuePair<string, int>("2 hours", 120)
                     };

            var journeyDurations = new List<SelectListItem>();

            foreach (var jd in jds)
            {
                journeyDurations.Add(new SelectListItem
                {
                    Text = jd.Key,
                    Value = jd.Value.ToString(),
                    Selected = jd.Value == selectedJourneyDuration
                });
            }

            return journeyDurations;
        }

        public static HtmlString GetDaylightIcon(bool isCurrentlyInDaylight)
        {
            var daylightIcon = isCurrentlyInDaylight ? "<i class=\"fa fa-sun-o\"></i>" : "<i class=\"fa fa-moon-o\"></i>";

            return new HtmlString(daylightIcon);
        }

        public static string GetDaylightText(bool isCurrentlyInDaylight)
        {
            return isCurrentlyInDaylight ? "light" : "dark";
        }

        public static HtmlString GetCommuteDirectionIcon(Commute commuteType)
        {
            string commuteDirectionIcon;

            switch (commuteType)
            {
                case Commute.ToWork:
                    commuteDirectionIcon = "<i class=\"fa fa-arrow-circle-o-right commute-direction\"></i>";
                    break;

                case Commute.FromWork:
                    commuteDirectionIcon = "<i class=\"fa fa-arrow-circle-o-left commute-direction\"></i>";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("commuteType");
            }

            return new HtmlString(commuteDirectionIcon);
        }

        public static string GetCommuteText(Commute commuteType)
        {
            switch (commuteType)
            {
                case Commute.ToWork:
                    return "to work";

                case Commute.FromWork:
                    return "back home";

                default:
                    throw new ArgumentOutOfRangeException("commuteType");
            }
        }

        public static string FormatCommuteTime(CommuteTimeModel commuteTime)
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
            string tweetText;

            if (!model.tw.Daylights.NextWorkingDayDaylightTransition.HasValue &&
                !model.fw.Daylights.NextWorkingDayDaylightTransition.HasValue &&
                model.tw.Daylights.IsCurrentlyInDaylight == model.fw.Daylights.IsCurrentlyInDaylight)
            {
                tweetText = string.Format(
                    "My journeys {0} and {1} are always in the {2}!",
                    GetCommuteText(model.tw.Daylights.CommuteType),
                    GetCommuteText(model.fw.Daylights.CommuteType),
                    GetDaylightText(model.tw.Daylights.IsCurrentlyInDaylight));
            }
            else
            {
                var prefixToWorkWithIHave = model.tw.Daylights.NextWorkingDayDaylightTransition.HasValue;
                var prefixFromWorkWithIHave = !prefixToWorkWithIHave && model.fw.Daylights.NextWorkingDayDaylightTransition.HasValue;

                tweetText = string.Format(
                    "{0} and {1}!",
                    GetDaylightInfoTweetText(model.tw.Daylights, prefixToWorkWithIHave),
                    GetDaylightInfoTweetText(model.fw.Daylights, prefixFromWorkWithIHave));
            }

            return tweetText;
        }

        private static string GetDaylightInfoTweetText(DaylightInfoModel daylightInfo, bool prefixWithIHave)
        {
            if (daylightInfo.NextWorkingDayDaylightTransition.HasValue)
            {
                return string.Format(
                    "{0}{1} more {2} journeys {3}",
                    prefixWithIHave ? "I have " : string.Empty,
                    daylightInfo.NumberOfDaysToTransition,
                    GetDaylightText(daylightInfo.IsCurrentlyInDaylight),
                    GetCommuteText(daylightInfo.CommuteType));
            }

            return string.Format(
                "{0} journey {1} is always in the {2}",
                daylightInfo.CommuteType == Commute.ToWork ? "My" : "my",
                GetCommuteText(daylightInfo.CommuteType),
                GetDaylightText(daylightInfo.IsCurrentlyInDaylight));
        }
    }
}