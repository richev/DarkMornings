using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Richev.DarkMornings.Web.Models;

namespace Richev.DarkMornings.Web
{
    public static class UIHelpers
    {
        /// <summary>
        /// For model hours and minutes, important because of JavaScript assumptions.
        /// </summary>
        private const string PadLeadingZero = "00";

        public static List<SelectListItem> GetHours(string selectedTime)
        {
            DateTime time;
            TryGetTime(selectedTime, out time);

            var hours = new List<SelectListItem>();

            for (var h = 0; h < 24; h++)
            {
                hours.Add(new SelectListItem
                              {
                                  Text = h.ToString(PadLeadingZero),
                                  Value = h.ToString(PadLeadingZero),
                                  Selected = h == time.Hour
                              });
            }

            return hours;
        }

        public static List<SelectListItem> GetMinutes(string selectedTime)
        {
            DateTime time;
            TryGetTime(selectedTime, out time);

            var minutes = new List<SelectListItem>();

            for (var m = 0; m < 60; m += 5)
            {
                minutes.Add(new SelectListItem
                                {
                                    Text = m.ToString(PadLeadingZero),
                                    Value = m.ToString(PadLeadingZero),
                                    Selected = m == time.Minute
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
                         new KeyValuePair<string, int>("20 minutes", 20),
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

        /// <summary>
        /// Returns an HtmlString for a sun or a moon icon.
        /// </summary>
        public static HtmlString GetDaylightIcon(bool isCurrentlyInDaylight)
        {
            var daylightIcon = isCurrentlyInDaylight ? "<i class=\"fa fa-sun-o\"></i>" : "<i class=\"fa fa-moon-o\"></i>";

            return new HtmlString(daylightIcon);
        }

        /// <summary>
        /// Returns "light" or "dark"
        /// </summary>
        public static string GetDaylightText(bool isCurrentlyInDaylight)
        {
            return isCurrentlyInDaylight ? "light" : "dark";
        }

        /// <summary>
        /// Returns a string such as "light journeys".
        /// </summary>
        /// <param name="isCurrentlyInDaylight"></param>
        /// <param name="numberOfDaysToTransition"></param>
        /// <returns></returns>
        public static string GetDaylightJourneysText(bool isCurrentlyInDaylight, int numberOfDaysToTransition)
        {
            var daylightJourneysText = GetDaylightText(isCurrentlyInDaylight);

            if (numberOfDaysToTransition == 1)
            {
                daylightJourneysText += " journey";
            }
            else
            {
                daylightJourneysText += " journeys";
            }

            return daylightJourneysText;
        }

        /// <summary>
        /// Returns an HtmlString for a right pointing (to work) or left pointing (from work) icon.
        /// </summary>
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

        public static bool TryGetTime(string commuteTime, out DateTime time)
        {
            return DateTime.TryParseExact(commuteTime, "HHmm", CultureInfo.CurrentCulture, DateTimeStyles.None, out time);
        }

        public static string GetTweetText(CommuteInfoModel model)
        {
            string tweetText;

            if (!model.ToWorkDaylights.NextWorkingDayDaylightTransition.HasValue &&
                !model.FromWorkDaylights.NextWorkingDayDaylightTransition.HasValue &&
                model.ToWorkDaylights.IsCurrentlyInDaylight == model.FromWorkDaylights.IsCurrentlyInDaylight)
            {
                tweetText = string.Format(
                    "My journeys {0} and {1} are always in the {2}!",
                    GetCommuteText(model.ToWorkDaylights.CommuteType),
                    GetCommuteText(model.FromWorkDaylights.CommuteType),
                    GetDaylightText(model.ToWorkDaylights.IsCurrentlyInDaylight));
            }
            else
            {
                var prefixToWorkWithIHave = model.ToWorkDaylights.NextWorkingDayDaylightTransition.HasValue;
                var prefixFromWorkWithIHave = !prefixToWorkWithIHave && model.FromWorkDaylights.NextWorkingDayDaylightTransition.HasValue;

                tweetText = string.Format(
                    "{0} and {1}!",
                    GetDaylightInfoTweetText(model.ToWorkDaylights, prefixToWorkWithIHave),
                    GetDaylightInfoTweetText(model.FromWorkDaylights, prefixFromWorkWithIHave));
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