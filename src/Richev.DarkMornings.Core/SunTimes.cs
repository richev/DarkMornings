using System;
using System.Xml.Linq;

namespace RichEv.DarkMornings.Core
{
    public class SunTimes : ISunTimes
    {
        // http://www.esrl.noaa.gov/gmd/grad/solcalc/
        // https://github.com/mourner/suncalc/blob/master/suncalc.js

        /// <summary>
        /// <para>URL template</para>
        /// <para>See http://www.earthtools.org/webservices.htm#sun </para>
        /// <para>
        /// http://www.earthtools.org/sun/&lt;latitude&gt;/&lt;longitude&gt;/&lt;day&gt;/&lt;month&gt;/&lt;timezone&gt;/&lt;dst&gt;
        /// </para>
        /// </summary>
        private const string Url = "http://www.earthtools.org/sun/{0}/{1}/{2}/{3}/{4}/{5}";

        private const int TimezoneAutomatic = 99;

        public void GetSunTimes(double latitude, double longitude, DateTime date, out DateTime sunRises, out DateTime sunSets)
        {
            var url = string.Format(Url, latitude, longitude, date.Day, date.Month, TimezoneAutomatic, date.IsDaylightSavingTime() ? 1 : 0);

            var doc = XDocument.Load(url);

            var sunrise = doc.Root.Element("morning").Element("sunrise").Value;
            sunRises = GetSuntime(date, sunrise);

            var sunset = doc.Root.Element("evening").Element("sunset").Value;
            sunSets = GetSuntime(date, sunset);
        }

        private DateTime GetSuntime(DateTime date, string time)
        {
            var timeParts = time.Split(':');

            var sunTime = date.Date;
            sunTime = sunTime.AddHours(int.Parse(timeParts[0]));
            sunTime = sunTime.AddMinutes(int.Parse(timeParts[1]));
            sunTime = sunTime.AddSeconds(int.Parse(timeParts[2]));

            return sunTime;
        }
    }

    public interface ISunTimes
    {
        void GetSunTimes(double latitude, double longitude, DateTime date, out DateTime sunRises, out DateTime sunSets);
    }
}
