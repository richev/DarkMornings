using System;
using NUnit.Framework;

namespace Richev.DarkMornings.Core.Tests
{
    [TestFixture]
    public class DaylightHunterTests
    {
        private static readonly Location _locationLondon = new Location { Latitude = 51, Longitude = -3 };
        private const int TimeZoneLondon = 0;

        private static readonly Location _locationNewYork = new Location { Latitude = 40.67, Longitude = -73.94 };
        private const int TimeZoneNewYork = -5;

        private static readonly Location _locationSydney = new Location { Latitude = -33.859972, Longitude = 151.211111 };
        private const int TimeZoneSydney = 10;

        private DaylightHunter _daylightHunter;

        [SetUp]
        public void SetUp()
        {
            _daylightHunter = new DaylightHunter();
        }

        private static readonly object[] _isCorrectForScenarioCases =
        {
            new object[] { "London in the winter", _locationLondon, TimeZoneLondon, new DateTime(2013, 12, 24, 7, 10, 0), new DateTime(2013, 12, 24, 18, 0, 0), false, new DateTime(2014, 02, 27, 7, 10, 0), DaylightTransition.SunRise, false, new DateTime(2014, 03, 29, 18, 31, 0), DaylightTransition.SunSet },
            
            new object[] { "London in the summer", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 7, 10, 0), new DateTime(2014, 6, 24, 18, 0, 0), true, new DateTime(2014, 09, 24, 7, 11, 0), DaylightTransition.SunRise, true, new DateTime(2014, 10, 7, 18, 29, 0), DaylightTransition.SunSet },
            
            new object[] { "Sydney in the winter", _locationSydney, TimeZoneSydney, new DateTime(2014, 6, 24, 6, 10, 0), new DateTime(2014, 6, 24, 18, 0, 0), false, new DateTime(2014, 9, 5, 6, 10, 0), DaylightTransition.SunRise, false, new DateTime(2014, 10, 06, 18, 59, 0), DaylightTransition.SunSet },
            
            new object[] { "New York in the winter", _locationNewYork, TimeZoneNewYork, new DateTime(2014, 2, 24, 6, 30, 0), new DateTime(2014, 2, 24, 18, 30, 0), false, new DateTime(2014, 3, 6, 6, 30, 0), DaylightTransition.SunRise, false, new DateTime(2014, 3, 19, 19, 00, 0), DaylightTransition.SunSet },
            
            new object[] { "Always light or dark", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 0, 10, 0), new DateTime(2014, 6, 24, 12, 0, 0), false, null, null, true, null, null },
            
            new object[] { "Always dark", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 0, 10, 0), new DateTime(2014, 6, 24, 3, 0, 0), false, null, null, false, null, null },
            
            new object[] { "Always dark midnight rollover", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 0, 10, 0), new DateTime(2014, 6, 24, 23, 0, 0), false, null, null, false, null, null },

            new object[] { "Always light", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 12, 0, 0), new DateTime(014, 6, 24, 13, 0, 0), true, null, null, true, null, null },
        };

        [Test]
        [TestCaseSource("_isCorrectForScenarioCases")]
        public void IsCorrectForScenario(
            string scenarioName,
            Location location, double timeZone,
            DateTime commuteToWorkStart, DateTime commuteToHomeStart,
            bool toWorkIsCurrentlyInDaylight, DateTime? toWorkNextDaylightTransition, DaylightTransition? toWorkDaylightTransition,
            bool fromWorkIsCurrentlyInDaylight, DateTime? fromWorkNextDaylightTransition, DaylightTransition? fromWorkDaylightTransition)
        {
            Console.WriteLine(scenarioName);

            var commuteInfo = _daylightHunter.GetDaylight(location, timeZone, commuteToWorkStart, commuteToWorkStart.AddMinutes(30), commuteToHomeStart, commuteToHomeStart.AddMinutes(30));

            Assert.AreEqual(toWorkIsCurrentlyInDaylight, commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(toWorkNextDaylightTransition, commuteInfo.ToWork.NextDaylightTransition);
            Assert.AreEqual(toWorkDaylightTransition, commuteInfo.ToWork.TransitionType);

            Assert.AreEqual(fromWorkIsCurrentlyInDaylight, commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(fromWorkNextDaylightTransition, commuteInfo.FromWork.NextDaylightTransition);
            Assert.AreEqual(fromWorkDaylightTransition, commuteInfo.FromWork.TransitionType);
        }
    }
}
