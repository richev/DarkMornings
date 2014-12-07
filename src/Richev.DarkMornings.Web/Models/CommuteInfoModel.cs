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
        /// Commute to work
        /// </summary>
        [DisplayName("to work time")]
        [StringLength(4, MinimumLength = 4)]
        public string t { get; set; }

        /// <summary>
        /// Commute from work
        /// </summary>
        [DisplayName("from work time")]
        [StringLength(4, MinimumLength = 4)]
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