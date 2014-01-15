using System;
using NUnit.Framework;

namespace Richev.DarkMornings.Core.Tests
{
    [TestFixture]
    public class DaylightHunterTests
    {
        private readonly TimeSpan _commuteDuration = new TimeSpan(0, 30, 0);

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

        // TODO: Check all commutesInDaylightPerYearAssert values

        private static readonly object[] _getDaylightIsCorrectForCommuteCases =
        {
            new object[] { "London in the winter, sunrise", _locationLondon, TimeZoneLondon, new DateTime(2013, 12, 24, 7, 10, 0), false, 234, new DateTime(2014, 02, 20, 7, 10, 0), DaylightTransition.SunRise },
            new object[] { "London in the winter, sunset", _locationLondon, TimeZoneLondon, new DateTime(2013, 12, 24, 18, 0, 0), false, 184, new DateTime(2014, 3, 30, 19, 22, 0), DaylightTransition.SunSet },
            
            new object[] { "London in the summer, sunrise", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 7, 10, 0), true, 234, new DateTime(2014, 10, 1, 7, 11, 0), DaylightTransition.SunRise },
            new object[] { "London in the summer, sunset", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 18, 0, 0), true, 185, new DateTime(2014, 9, 30, 18, 29, 0), DaylightTransition.SunSet },
            
            new object[] { "New York in the winter, sunrise", _locationNewYork, TimeZoneNewYork, new DateTime(2014, 2, 24, 6, 30, 0), false, 148, new DateTime(2014, 3, 5, 6, 30, 0), DaylightTransition.SunRise },
            new object[] { "New York in the winter, sunset", _locationNewYork, TimeZoneNewYork, new DateTime(2014, 2, 24, 18, 30, 0), false, 180, new DateTime(2014, 3, 18, 19, 00, 0), DaylightTransition.SunSet },

            new object[] { "New York in the summer, sunrise", _locationNewYork, TimeZoneNewYork, new DateTime(2014, 6, 24, 6, 30, 0), true, 147, new DateTime(2014, 9, 2, 6, 31, 0), DaylightTransition.SunRise },
            new object[] { "New York in the summer, sunset", _locationNewYork, TimeZoneNewYork, new DateTime(2014, 6, 24, 18, 30, 0), true, 180, new DateTime(2014, 9, 14, 18, 59, 0), DaylightTransition.SunSet },
            
            new object[] { "Seattle in the winter, sunrise", _locationSeattle, TimeZoneSeattle, new DateTime(2014, 2, 24, 6, 30, 0), false, 136, new DateTime(2014, 4, 14, 6, 30, 0), DaylightTransition.SunRise },
            new object[] { "Seattle in the winter, sunset", _locationSeattle, TimeZoneSeattle, new DateTime(2014, 2, 24, 18, 30, 0), false, 197, new DateTime(2014, 3, 9, 19, 00, 0), DaylightTransition.SunSet },

            new object[] { "Seattle in the summer, sunrise", _locationSeattle, TimeZoneSeattle, new DateTime(2014, 6, 24, 6, 30, 0), true, 136, new DateTime(2014, 8, 28, 6, 31, 0), DaylightTransition.SunRise },
            new object[] { "Seattle in the summer, sunset", _locationSeattle, TimeZoneSeattle, new DateTime(2014, 6, 24, 18, 30, 0), true, 197, new DateTime(2014, 9, 22, 18, 58, 0), DaylightTransition.SunSet },
            
            new object[] { "Sydney in the winter, sunrise", _locationSydney, TimeZoneSydney, new DateTime(2014, 6, 24, 6, 20, 0), false, 161, new DateTime(2014, 8, 28, 6, 20, 0), DaylightTransition.SunRise },
            new object[] { "Sydney in the winter, sunset", _locationSydney, TimeZoneSydney, new DateTime(2014, 6, 24, 18, 0, 0), false, 182, new DateTime(2014, 10, 5, 18, 59, 0), DaylightTransition.SunSet },
            
            new object[] { "Sydney in the summer, sunrise", _locationSydney, TimeZoneSydney, new DateTime(2014, 1, 24, 6, 20, 0), true, 160, new DateTime(2014, 2, 1, 6, 21, 0), DaylightTransition.SunRise },
            new object[] { "Sydney in the summer, sunset", _locationSydney, TimeZoneSydney, new DateTime(2014, 1, 24, 18, 0, 0), true, 183, new DateTime(2014, 4, 6, 17, 42, 0), DaylightTransition.SunSet },
            
            new object[] { "Auckland in the winter, sunrise", _locationAuckland, TimeZoneAuckland, new DateTime(2014, 6, 24, 6, 20, 0), false, 88, new DateTime(2014, 9, 16, 6, 20, 0), DaylightTransition.SunRise },
            new object[] { "Auckland in the winter, sunset", _locationAuckland, TimeZoneAuckland, new DateTime(2014, 6, 24, 18, 20, 0), false, 189, new DateTime(2014, 9, 28, 19, 21, 0), DaylightTransition.SunSet },

            new object[] { "Auckland in the summer, sunrise", _locationAuckland, TimeZoneAuckland, new DateTime(2014, 1, 24, 6, 40, 0), true, 136, new DateTime(2014, 2, 1, 6, 41, 0), DaylightTransition.SunRise },
            new object[] { "Auckland in the summer, sunset", _locationAuckland, TimeZoneAuckland, new DateTime(2014, 1, 24, 18, 0, 0), true, 190, new DateTime(2014, 4, 6, 18, 6, 0), DaylightTransition.SunSet },
            
            new object[] { "Always dark", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 0, 10, 0), false, 0, null, null },
            new object[] { "Always light", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 12, 0, 0), true, DaylightHunter.DaysInYear, null, null },
            
            new object[] { "Always dark midnight rollover", _locationLondon, TimeZoneLondon, new DateTime(2014, 6, 24, 23, 40, 0), false, 0, null, null }
        };

        [Test]
        [TestCaseSource("_getDaylightIsCorrectForCommuteCases")]
        public void GetDaylightIsCorrectForCommute(
            string scenarioName,
            Location location, double timeZone, DateTime commuteStart,
            bool isCurrentlyInDaylightAssert, int commutesInDaylightPerYearAssert, DateTime? nextDaylightTransitionAssert, DaylightTransition? daylightTransitionAssert)
        {
            Console.WriteLine(scenarioName);

            var daylightInfo = _daylightHunter.GetDaylight(location, timeZone, commuteStart, commuteStart.Add(_commuteDuration));

            Assert.AreEqual(isCurrentlyInDaylightAssert, daylightInfo.IsCurrentlyInDaylight);
            Assert.AreEqual(commutesInDaylightPerYearAssert, daylightInfo.CommutesInDaylightPerYear);
            Assert.AreEqual(nextDaylightTransitionAssert, daylightInfo.NextDaylightTransition);
            Assert.AreEqual(daylightTransitionAssert, daylightInfo.TransitionType);
        }
    }
}
