using System;

namespace Richev.DarkMornings.Web.Models
{
    public class DaylightInfoModel
    {
        public bool IsCurrentlyInDaylight { get; set; }

        /// <summary>
        /// <para>When there will next be a transition between daylight/not daylight or vice versa on ON A WORKING DAY for this commute.</para>
        /// <para>Will be null if this commute is always in daylight or dark.</para>
        /// </summary>
        public DateTime? NextWorkingDayDaylightTransition { get; set; }

        public Commute CommuteType { get; set; }

        public int NumberOfDaysToTransition { get; set; }
    }
}