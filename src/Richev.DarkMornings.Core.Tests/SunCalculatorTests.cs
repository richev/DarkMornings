using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Richev.DarkMornings.Core.Tests
{
    [TestFixture]
    public class SunCalculatorTests
    {
        [Test]
        public void Foo()
        {
            var toWork = DateTime.Now.Date.AddHours(8).AddMinutes(10);
            var fromWork = DateTime.Now.Date.AddHours(17).AddMinutes(30);
            var journeyTime = 30 / 60.0;

            //var sc = new SunCalculator(-0.1, 51.52);
            var sc = new SunCalculator(3.189, 55.95); // Edinburgh

            var data = new List<string>();

            var toWorkTime = TimeToMinutes(toWork);
            var fromWorkTime = TimeToMinutes(fromWork);

            for (var date = new DateTime(2013, 1, 1); date < new DateTime(2014, 1, 1); date = date.AddDays(1))
            {
                var sunRise = sc.CalculateSunRise(date, "GMT Standard Time");
                var sunSet = sc.CalculateSunSet(date, "GMT Standard Time");

                var sunRiseTime = TimeToMinutes(sunRise);
                var sunSetTime = TimeToMinutes(sunSet);

                for (var hour = 0.0; hour < 24; hour++)
                {
                    GetDataRow(data.Add, date, hour, sunRiseTime, sunSetTime, toWorkTime, fromWorkTime, journeyTime, false);
                }

                // Sunrise infill
                GetInfillDataRows(data.Add, date, sunRiseTime, sunRiseTime, sunSetTime, toWorkTime, fromWorkTime, journeyTime);

                // Sunset infill
                GetInfillDataRows(data.Add, date, sunSetTime, sunRiseTime, sunSetTime, toWorkTime, fromWorkTime, journeyTime);

                //GetInfillDataRows(data.Add, date, toWorkTime, toWorkTime, toWorkTime + journeyTime, toWorkTime, fromWorkTime, journeyTime);
                //GetInfillDataRows(data.Add, date, fromWorkTime, sunRiseTime, sunSetTime, toWorkTime, fromWorkTime, journeyTime);
            }

            System.Diagnostics.Debug.WriteLine(string.Join("\n", data));
        }

        private void GetInfillDataRows(Action<string> addFunc, DateTime date, double time, double sunRiseTime, double sunSetTime, double toWorkTime, double fromWorkTime, double journeyTime, bool ignoreIfInt = true)
        {
            var roundedTime = Math.Round(time, 2);
            GetDataRow(addFunc, date, roundedTime, sunRiseTime, sunSetTime, toWorkTime, fromWorkTime, journeyTime, ignoreIfInt);
            GetDataRow(addFunc, date, roundedTime + 0.01, sunRiseTime, sunSetTime, toWorkTime, fromWorkTime, journeyTime, ignoreIfInt);
        }

        private double TimeToMinutes(DateTime date)
        {
            return date.Hour + (date.Minute / 60.0);
        }

        private void GetDataRow(Action<string> addFunc, DateTime date, double time, double sunRiseTime, double sunSetTime, double toWorkTime, double fromWorkTime, double journeyTime, bool ignoreIfInt = true)
        {
            if (ignoreIfInt && time % 1 == 0) return;

            var light = (time >= sunRiseTime && time <= sunSetTime) ? 1 : 2;

            if (date.Month == DateTime.Now.Month && date.Day == DateTime.Now.Day)
            {
                // Highlight today
                light = (time >= sunRiseTime && time <= sunSetTime) ? 3 : 4;
            }
            /*else if (time >= toWorkTime && time <= (toWorkTime + journeyTime))
            {
                // Highlight to work
                light = (time >= sunRiseTime && time <= sunSetTime) ? 3 : 4;
            }
            else if (time >= fromWorkTime && time <= (fromWorkTime + journeyTime))
            {
                // Highlight from work
                light = (time >= sunRiseTime && time <= sunSetTime) ? 3 : 4;
            }*/

            addFunc(string.Format("{0:yyyy-MM-dd},{1},{2}", date, time, light));
        }
    }
}
