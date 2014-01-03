using System;
using NUnit.Framework;

namespace Richev.DarkMornings.Core.Tests
{
    [TestFixture]
    public class DaylightHunterTests
    {
        private const int TimezoneUK = 0;

        private const int TimeZoneNewYork = -5;

        private DaylightHunter _daylightHunter;

        [SetUp]
        public void SetUp()
        {
            _daylightHunter = new DaylightHunter();
        }

        [Test]
        public void IsCorrectInWinter()
        {
            var outboundCommuteAt = new DateTime(2013, 12, 24, 7, 10, 0);
            var returnCommuteAt = new DateTime(2013, 12, 24, 18, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(51, -3, TimezoneUK, outboundCommuteAt, returnCommuteAt);

            Assert.IsFalse(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 02, 27, 7, 10, 0), commuteInfo.ToWork.NextDaylightTransition);
            Assert.AreEqual(DaylightTransition.SunRise, commuteInfo.ToWork.TransitionType);

            Assert.IsFalse(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 03, 10, 18, 0, 0), commuteInfo.FromWork.NextDaylightTransition);
            Assert.AreEqual(DaylightTransition.SunSet, commuteInfo.FromWork.TransitionType);
        }

        [Test]
        public void IsCorrectInSummer()
        {
            var outboundCommuteAt = new DateTime(2014, 6, 24, 7, 10, 0);
            var returnCommuteAt = new DateTime(2014, 6, 24, 18, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(51, -3, TimezoneUK, outboundCommuteAt, returnCommuteAt);

            Assert.IsTrue(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 09, 24, 7, 11, 0), commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsTrue(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 10, 21, 17, 59, 0), commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectInSouthernHemisphereWinter()
        {
            var outboundCommuteAt = new DateTime(2014, 6, 24, 7, 10, 0);
            var returnCommuteAt = new DateTime(2014, 6, 24, 18, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(-51, -3, TimezoneUK, outboundCommuteAt, returnCommuteAt);

            Assert.IsFalse(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 9, 19, 7, 09, 0), commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsFalse(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 8, 11, 18, 0, 0), commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectForAlwaysDarkOrLightCommute()
        {
            var outboundCommuteAt = new DateTime(2014, 6, 24, 0, 10, 0);
            var eveningCommuteAt = new DateTime(2014, 6, 24, 12, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(51, -3, TimezoneUK, outboundCommuteAt, eveningCommuteAt);

            Assert.AreEqual(false, commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.ToWork.NextDaylightTransition);

            Assert.AreEqual(true, commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectForAlwaysDarkCommutes()
        {
            var outboundCommuteAt = new DateTime(2014, 6, 24, 0, 10, 0);
            var returnCommuteAt = new DateTime(2014, 6, 24, 3, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(51, -3, TimezoneUK, outboundCommuteAt, returnCommuteAt);

            Assert.IsFalse(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsFalse(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectForAlwaysLightCommutes()
        {
            var outboundCommuteAt = new DateTime(2014, 6, 24, 12, 0, 0);
            var returnCommuteAt = new DateTime(2014, 6, 24, 13, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(51, -3, TimezoneUK, outboundCommuteAt, returnCommuteAt);

            Assert.IsTrue(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsTrue(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(null, commuteInfo.FromWork.NextDaylightTransition);
        }

        [Test]
        public void IsCorrectInNewYork()
        {
            var outboundCommuteAt = new DateTime(2014, 6, 24, 12, 0, 0);
            var returnCommuteAt = new DateTime(2014, 6, 24, 13, 0, 0);

            var commuteInfo = _daylightHunter.GetDaylight(40.67, -73.94, TimeZoneNewYork, outboundCommuteAt, returnCommuteAt);

            Assert.IsFalse(commuteInfo.ToWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 9, 15, 7, 09, 0), commuteInfo.ToWork.NextDaylightTransition);

            Assert.IsFalse(commuteInfo.FromWork.IsCurrentlyInDaylight);
            Assert.AreEqual(new DateTime(2014, 8, 11, 18, 0, 0), commuteInfo.FromWork.NextDaylightTransition);
        }
    }
}
