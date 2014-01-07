using System.Collections.Generic;

namespace Richev.DarkMornings.Core
{
    public static class TimeZones
    {
        public static readonly Dictionary<double, string> Selected = new Dictionary<double,string>
        {
            { -12,  "Dateline Standard Time" },
            { -11,  "UTC-11" },
            { -10,  "Hawaiian Standard Time" },
            { -9,  "Alaskan Standard Time" },
            { -8,  "Pacific Standard Time" },
            { -7,  "Mountain Standard Time" },
            { -6,  "Central Standard Time" },
            { -5,  "Eastern Standard Time" },
            { -4.5,  "Venezuela Standard Time" },
            { -4,  "Atlantic Standard Time" },
            { -3.5,  "Newfoundland Standard Time" },
            { -3,  "Argentina Standard Time" },
            { -2,  "Mid-Atlantic Standard Time" },
            { -1,  "Azores Standard Time" },
            { 0,  "GMT Standard Time" },
            { 1,  "Romance Standard Time" },
            { 2,  "E. Europe Standard Time" },
            { 3,  "Arabic Standard Time" },
            { 3.5,  "Iran Standard Time" },
            { 4,  "Russian Standard Time" },
            { 4.5,  "Afghanistan Standard Time" },
            { 5,  "Pakistan Standard Time" },
            { 5.5,  "India Standard Time" },
            { 5.75,  "Nepal Standard Time" },
            { 6,  "Central Asia Standard Time" },
            { 6.5,  "Myanmar Standard Time" },
            { 7,  "SE Asia Standard Time" },
            { 8,  "W. Australia Standard Time" },
            { 9,  "Korea Standard Time" },
            { 9.5,  "AUS Central Standard Time" },
            { 10,  "AUS Eastern Standard Time" },
            { 11,  "Central Pacific Standard Time" },
            { 12,  "New Zealand Standard Time" },
            { 13,  "Samoa Standard Time" },
        };
    }
}
