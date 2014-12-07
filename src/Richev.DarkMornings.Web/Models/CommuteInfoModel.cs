using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        /// Timezone hours offset
        /// </summary>
        [DisplayName("timezone hours offset (z)")]
        [Required]
        [Range(-12, 13)]
        public double? z { get; set; } // not an int, because some timezones are half an hour out

        /// <summary>
        /// Working days
        /// </summary>
        [DisplayName("working days (d)")]
        public WorkingDays d { get; set; }

        /// <summary>
        /// Commute to work time, formatted as HH:mm
        /// </summary>
        [DisplayName("leave home at")]
        [Required]
        [RegularExpression(RegexTime, ErrorMessage = "The {0} time (t) must be represent a 24-hour time in the format hhmm")]
        public string t { get; set; }

        /// <summary>
        /// Commute from work time, formatted as HH:mm
        /// </summary>
        [DisplayName("leave work at")]
        [Required]
        [RegularExpression(RegexTime, ErrorMessage = "The {0} time (f) must be represent a 24-hour time in the format hhmm")]
        public string f { get; set; }

        public DaylightInfoModel ToWorkDaylights { get; set; }

        public DaylightInfoModel FromWorkDaylights { get; set; }

        public bool HasDefaultValues()
        {
            return !y.HasValue &&
                   !x.HasValue &&
                   !z.HasValue &&
                   string.IsNullOrEmpty(t) &&
                   string.IsNullOrEmpty(f) &&
                   d == 0 &&
                   j == 0;
        }
    }
}