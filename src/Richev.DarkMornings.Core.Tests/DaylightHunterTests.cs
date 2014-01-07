using System;
using NUnit.Framework;

namespace Richev.DarkMornings.Core.Tests
{
    [TestFixture]
    public class DaylightHunterTests
    {
        private static readonly Location _locationLondon = new Location { Latitude = 51.5072, Longitude = 0.1275 };
        private const int TimeZoneLondon = 0;

        private static readonly Location _locationNewYork = new Location { Latitude = 40.67, Longitude = -73.94 };
        private const int TimeZoneNewYork = -5;

        private static readonly Location _locationSeattle = new Location { Latitude = 47.6097, Longitude = -122.3331 };
        private const int TimeZoneSeattle = -8;

        private static readonly Location _locationSydney = new Location { Latitude = -33.859972, Longitude = 151.211111 };
        private const int TimeZoneSydney = 10;

        private static readonly Location _locationAuckland = new Location { Latitude = -36.8404, Longitude = 174.7399 };
        private const int TimeZoneAuckland = 12;

        private DaylightHunter _daylightHunter;

        [SetUp]
        public void SetUp()
        {
            _daylightHunter = new DaylightHunter();
        }

        // Values checked with http://www.timeanddate.com/worldclock/sunrise.html

        private static readonly object[] _isCorrectForScenarioCases =
        {
            new object[] { "London in the winter", _locationLondon, TimeZoneLondon, new DateTime(2013, 12, 24, 7, 10, 0), new DateTime(2013, 12, 24, 18, 0, 0), false, new DateTime(2014, 02, 20, 7, 10, 0), DaylightTransition.SunRise, false, new DateTime(2014, 3, 30, 19, 22, 0), DaylightTransition.SunSet },
            
            new object[] { "London in the summer", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 7, 10, 0), new DateTime(2014, 6, 24, 18, 0, 0), true, new DateTime(2014, 10, 1, 7, 11, 0), DaylightTransition.SunRise, true, new DateTime(2014, 9, 30, 18, 29, 0), DaylightTransition.SunSet },
            
            new object[] { "New York in the winter", _locationNewYork, TimeZoneNewYork, new DateTime(2014, 2, 24, 6, 30, 0), new DateTime(2014, 2, 24, 18, 30, 0), false, new DateTime(2014, 3, 5, 6, 30, 0), DaylightTransition.SunRise, false, new DateTime(2014, 3, 18, 19, 00, 0), DaylightTransition.SunSet },

            new object[] { "New York in the summer", _locationNewYork, TimeZoneNewYork, new DateTime(2014, 6, 24, 6, 30, 0), new DateTime(2014, 6, 24, 18, 30, 0), true, new DateTime(2014, 9, 2, 6, 31, 0), DaylightTransition.SunRise, true, new DateTime(2014, 9, 14, 18, 59, 0), DaylightTransition.SunSet },
            
            new object[] { "Seattle in the winter", _locationSeattle, TimeZoneSeattle, new DateTime(2014, 2, 24, 6, 30, 0), new DateTime(2014, 2, 24, 18, 30, 0), false, new DateTime(2014, 4, 14, 6, 30, 0), DaylightTransition.SunRise, false, new DateTime(2014, 3, 9, 19, 00, 0), DaylightTransition.SunSet },

            new object[] { "Seattle in the summer", _locationSeattle, TimeZoneSeattle, new DateTime(2014, 6, 24, 6, 30, 0), new DateTime(2014, 6, 24, 18, 30, 0), true, new DateTime(2014, 8, 28, 6, 31, 0), DaylightTransition.SunRise, true, new DateTime(2014, 9, 22, 18, 58, 0), DaylightTransition.SunSet },
            
            new object[] { "Sydney in the winter", _locationSydney, TimeZoneSydney, new DateTime(2014, 6, 24, 6, 20, 0), new DateTime(2014, 6, 24, 18, 0, 0), false, new DateTime(2014, 8, 28, 6, 20, 0), DaylightTransition.SunRise, false, new DateTime(2014, 10, 5, 18, 59, 0), DaylightTransition.SunSet },
            
            new object[] { "Sydney in the summer", _locationSydney, TimeZoneSydney, new DateTime(2014, 1, 24, 6, 20, 0), new DateTime(2014, 1, 24, 18, 0, 0), true, new DateTime(2014, 2, 1, 6, 21, 0), DaylightTransition.SunRise, true, new DateTime(2014, 4, 6, 17, 42, 0), DaylightTransition.SunSet },
            
            new object[] { "Auckland in the winter", _locationAuckland, TimeZoneAuckland, new DateTime(2014, 6, 24, 6, 20, 0), new DateTime(2014, 6, 24, 18, 0, 0), false, new DateTime(2014, 9, 16, 6, 20, 0), DaylightTransition.SunRise, false, new DateTime(2014, 9, 28, 19, 21, 0), DaylightTransition.SunSet },
            
            new object[] { "Auckland in the summer", _locationAuckland, TimeZoneAuckland, new DateTime(2014, 1, 24, 6, 40, 0), new DateTime(2014, 1, 24, 18, 0, 0), true, new DateTime(2014, 2, 1, 6, 41, 0), DaylightTransition.SunRise, true, new DateTime(2014, 4, 6, 18, 6, 0), DaylightTransition.SunSet },
            
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
