using System.ComponentModel;

namespace Richev.DarkMornings.Web.Models
{
    public class WorkingDays
    {
        [DisplayName("Sun")]
        public bool Sunday { get; set; }

        [DisplayName("Mon")]
        public bool Monday { get; set; }

        [DisplayName("Tue")]
        public bool Tuesday { get; set; }

        [DisplayName("Wed")]
        public bool Wednesday { get; set; }

        [DisplayName("Thu")]
        public bool Thursday { get; set; }

        [DisplayName("Fri")]
        public bool Friday { get; set; }

        [DisplayName("Sat")]
        public bool Saturday { get; set; }

        /// <summary>
        /// Returns the days as a boolean array in the same order (Sunday first) as the System.DayOfWeek enumeration
        /// </summary>
        public bool[] ToArray()
        {
            return new[] { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };
        }
    }
}