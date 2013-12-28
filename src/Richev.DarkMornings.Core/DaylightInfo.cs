using System;
using System.Diagnostics;

namespace Richev.DarkMornings.Core
{
    [DebuggerDisplay("{IsCurrentlyInDaylight} {NextDaylightTransition}")]
    public class DaylightInfo
    {
        public bool IsCurrentlyInDaylight { get; set; }

        /// <summary>
        /// <para>When there will next be a transition between daylight/not daylight or vice versa for this commute.</para>
        /// <para>Will be null if this commute is always in daylight or dark.</para>
        /// </summary>
        public DateTime? NextDaylightTransition { get; set; }

        public DaylightTransition? TransitionType { get; set; }
    }
}