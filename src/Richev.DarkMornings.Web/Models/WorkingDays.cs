namespace Richev.DarkMornings.Web.Models
{
    public class WorkingDays
    {
        public bool Sunday { get; set; }

        public bool Monday { get; set; }

        public bool Tuesday { get; set; }

        public bool Wednesday { get; set; }

        public bool Thursday { get; set; }

        public bool Friday { get; set; }

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