using System;
using NUnit.Framework;

namespace Richev.DarkMornings.Core.Tests
{
    [TestFixture]
    public class DaylightHunterTests
    {
        private readonly Location _locationLondon = new Location { Latitude = 51, Longitude = -3 };
        private const int TimeZoneLondon = 0;

        private readonly Location _locationNewYork = new Location { Latitude = 40.67, Longitude = -73.94 };
        private const int TimeZoneNewYork = -5;

        private readonly Location _locationSydney = new Location { Latitude = -33.859972, Longitude = 151.211111 };
        private const int TimeZoneSydney = 10;

        private DaylightHunter _daylightHunter;

        [SetUp]
        public void SetUp()
        {
            _daylightHunter = new DaylightHunter();
        }

        [Test]
        public void IsCorrectInWinter()
        {
            var outboundCommuteStart = new DateTime(2013, 12, 24, 7, 10, 0);
            var returnCommuteStart = new DateTime(2013, 12, 24, 18, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(_locationLondon, TimeZoneLondon, outboundCommuteStart, outboundCommuteStart.AddMinutes(30), returnCommuteStart, returnCommuteStart.AddMinutes(30));

            Assert.IsFalse(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 02, 27, 7, 10, 0), commuteInfo.ToWork.NextDaylightTransition);
            Assert.AreEqual(DaylightTransition.SunRise, commuteInfo.ToWork.TransitionType);

            Assert.IsFalse(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 03, 29, 18, 31, 0), commuteInfo.FromWork.NextDaylightTransition);
            Assert.AreEqual(DaylightTransition.SunSet, commuteInfo.FromWork.TransitionType);
        }

        [Test]
        public void IsCorrectInSummer()
        {
            var outboundCommuteStart = new DateTime(2014, 6, 24, 7, 10, 0);
            var returnCommuteStart = new DateTime(2014, 6, 24, 18, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(_locationLondon, TimeZoneLondon, outboundCommuteStart, outboundCommuteStart.AddMinutes(30), returnCommuteStart, returnCommuteStart.AddMinutes(30));

            Assert.IsTrue(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 09, 24, 7, 11, 0), commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsTrue(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 10, 7, 18, 29, 0), commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectInSydneyWinter()
        {
            var outboundCommuteStart = new DateTime(2014, 6, 24, 6, 10, 0);
            var returnCommuteStart = new DateTime(2014, 6, 24, 18, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(_locationSydney, TimeZoneSydney, outboundCommuteStart, outboundCommuteStart.AddMinutes(30), returnCommuteStart, returnCommuteStart.AddMinutes(30));

            Assert.IsFalse(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 9, 5, 6, 10, 0), commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsFalse(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 10, 5, 18, 30, 0), commuteInfo.FromWork.NextDaylightTransition); // not yet right
        }

        [Test]
        public void IsCorrectForAlwaysDarkOrLightCommute()
        {
            var outboundCommuteStart = new DateTime(2014, 6, 24, 0, 10, 0);
            var returnCommuteStart = new DateTime(2014, 6, 24, 12, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(_locationLondon, TimeZoneLondon, outboundCommuteStart, outboundCommuteStart.AddMinutes(30), returnCommuteStart, returnCommuteStart.AddMinutes(30));

            Assert.AreEqual(false, commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.ToWork.NextDaylightTransition);

            Assert.AreEqual(true, commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectForAlwaysDarkCommutes()
        {
            var outboundCommuteStart = new DateTime(2014, 6, 24, 0, 10, 0);
            var returnCommuteStart = new DateTime(2014, 6, 24, 3, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(_locationLondon, TimeZoneLondon, outboundCommuteStart, outboundCommuteStart.AddMinutes(30), returnCommuteStart, returnCommuteStart.AddMinutes(30));

            Assert.IsFalse(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsFalse(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectForAlwaysDarkCommuteThatRollsOverMidnight()
        {
            var outboundCommuteStart = new DateTime(2014, 6, 24, 0, 10, 0);
            var returnCommuteStart = new DateTime(2014, 6, 24, 23, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(_locationLondon, TimeZoneLondon, outboundCommuteStart, outboundCommuteStart.AddMinutes(30), returnCommuteStart, returnCommuteStart.AddMinutes(120));

            Assert.IsFalse(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectForAlwaysLightCommutes()
        {
            var outboundCommuteStart = new DateTime(2014, 6, 24, 12, 0, 0);
            var returnCommuteStart = new DateTime(2014, 6, 24, 13, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(_locationLondon, TimeZoneLondon, outboundCommuteStart, outboundCommuteStart.AddMinutes(30), returnCommuteStart, returnCommuteStart.AddMinutes(30));

            Assert.IsTrue(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsTrue(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectInNewYorkWinter()
        {
            var outboundCommuteStart = new DateTime(2014, 2, 24, 6, 30, 0);
            var returnCommuteStart = new DateTime(2014, 2, 24, 18, 30, 0);

            var commuteInfo = _daylightHunter.GetDaylight(_locationNewYork, TimeZoneNewYork, outboundCommuteStart, outboundCommuteStart.AddMinutes(30), returnCommuteStart, returnCommuteStart.AddMinutes(30));

            Assert.IsFalse(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 3, 6, 6, 30, 0), commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsFalse(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 3, 19, 19, 00, 0), commuteInfo.FromWork.NextDaylightTransition);
        }
    }
}
