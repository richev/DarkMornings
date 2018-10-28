using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Richev.DarkMornings.Core;

namespace Richev.DarkMornings.Web.Models
{
    [Flags]
    public enum WorkingDays
    {
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64
    }

    public class CommuteInfoModel
    {
        /// <summary>
        /// 24-hour time in the format hhmm.
        /// </summary>
        private const string RegexTime = "^(0[0-9]|1[0-9]|2[0-3])[0-5][0-9]$";

        /// <summary>
        /// Latitude
        /// </summary>
        [DisplayName("latitude (y)")]
        [Range(-90, 90)]
        public double? y { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        [DisplayName("longitude (x)")]
        [Range(-180, 180)]
        public double? x { get; set; }

        /// <summary>
        /// Journey duration minutes
        /// </summary>
        [DisplayName("journey duration minutes (j)")]
        [Range(5, 120)]
        public int j { get; set; }

        /// <summary>
        /// Working days
        /// </summary>
        [DisplayName("working days (d)")]
        public WorkingDays d { get; set; }

        /// <summary>
        /// Leave home at time, formatted as HHmm
        /// </summary>
        [DisplayName("leave home at")]
        [Required(ErrorMessage = "The {0} time (h) must represent a 24-hour time in the format hhmm")]
        [RegularExpression(RegexTime, ErrorMessage = "The {0} time (h) must represent a 24-hour time in the format hhmm")]
        public string h { get; set; }

        /// <summary>
        /// Leave work at time, formatted as HHmm
        /// </summary>
        [DisplayName("leave work at")]
        [Required(ErrorMessage = "The {0} time (w) must represent a 24-hour time in the format hhmm")]
        [RegularExpression(RegexTime, ErrorMessage = "The {0} time (w) must represent a 24-hour time in the format hhmm")]
        public string w { get; set; }

        /// <summary>
        /// The time zone that was used.
        /// </summary>
        public string TimeZoneId { get; set; }

        public DaylightInfoModel ToWorkDaylights { get; set; }

        public DaylightInfoModel FromWorkDaylights { get; set; }

        /// <summary>
        /// Validates the model.
        /// </summary>
        /// <param name="modelState">Controller.ModelState</param>
        /// <param name="outboundCommuteStart">If the model is valid, will contain the start time of the outbound commute.</param>
        /// <param name="returnCommuteStart">If the model is valid, will contain the start time of the return commute.</param>
        public void Validate(System.Web.Mvc.ModelStateDictionary modelState, out DateTime outboundCommuteStart, out DateTime returnCommuteStart)
        {
            if (d == 0)
            {
                modelState.AddModelError("WorkingDays", "Please select at least one workday.");
            }

            if (!y.HasValue || !x.HasValue)
            {
                modelState.AddModelError("Location", "Sorry, we couldn't figure out your location."); // TODO: Actually this is kind of a fatal error
            }

            var today = DateTime.Now.Date;

            if (UIHelpers.TryGetTime(h, out outboundCommuteStart))
            {
                today.AddHours(outboundCommuteStart.Hour).AddMinutes(outboundCommuteStart.Minute);
            }

            if (UIHelpers.TryGetTime(w, out returnCommuteStart))
            {
                today.AddHours(returnCommuteStart.Hour).AddMinutes(returnCommuteStart.Minute);
            }

            if (Utils.GetTimeOfDayDifference(outboundCommuteStart, returnCommuteStart) <= new TimeSpan(0, j, 0))
            {
                modelState.AddModelError("JourneysOverlap", "Your journeys overlap one another. That can't be right.");
            }
        }

        public bool HasDefaultValues()
        {
            return !y.HasValue &&
                   !x.HasValue &&
                   string.IsNullOrEmpty(h) &&
                   string.IsNullOrEmpty(w) &&
                   d == 0 &&
                   j == 0;
        }
    }
}