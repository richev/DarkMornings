using System;

namespace Richev.DarkMornings.Web.Models
{
    public class DaylightInfoModel
    {
        public Commute CommuteType { get; set; }

        public bool IsCurrentlyInDaylight { get; set; }

        public double PercentOfTheYearInTheDark { get; set; }

        /// <summary>
        /// <para>When there will next be a transition between daylight/not daylight or vice versa on ON A WORKING DAY for this commute.</para>
        /// <para>Will be null if this commute is always in daylight or darkness.</para>
        /// </summary>
        public DateTime? NextWorkingDayDaylightTransition { get; set; }

        /// <summary>
        /// <para>Will be null if this commute is always in daylight or darkness.</para>
        /// </summary>
        public int NumberOfDaysToTransition { get; set; }
    }
}