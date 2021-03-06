﻿using System;
using System.Diagnostics;

namespace Richev.DarkMornings.Core
{
    [DebuggerDisplay("IsCurrentlyInDaylight={IsCurrentlyInDaylight} CommutesInDaylightPerYear={CommutesInDaylightPerYear} NextDaylightTransition={NextDaylightTransition} TransitionType={TransitionType}")]
    public class DaylightInfo
    {
        /// <summary>
        /// Whether the commute is in daylight for the current day
        /// </summary>
        public bool IsCurrentlyInDaylight { get; set; }

        /// <summary>
        /// The number of days that this commute will be in the daylight.
        /// </summary>
        public int CommutesInDaylightPerYear { get; set; }

        /// <summary>
        /// <para>When there will next be a transition between daylight/not daylight or vice versa for this commute.</para>
        /// <para>Will be null if this commute is always in daylight or darkness.</para>
        /// </summary>
        public DateTime? NextDaylightTransition { get; set; }

        /// <summary>
        /// Whether the next commute daylight transition will be a sunset or sunrise.
        /// </summary>
        public DaylightTransition? TransitionType { get; set; }
    }
}