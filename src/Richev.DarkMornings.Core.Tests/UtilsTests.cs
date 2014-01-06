using System;
using NUnit.Framework;

namespace Richev.DarkMornings.Core.Tests
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        public void CalculateLongitudeTimeZoneShouldWorkForLondon()
        {
            var longitudeTimeZone = Utils.CalculateLongitudeTimeZone(0);

            Assert.AreEqual(0, longitudeTimeZone);
        }

        [Test]
        public void CalculateLongitudeTimeZoneShouldWorkForNewYork()
        {
            var longitudeTimeZone = Utils.CalculateLongitudeTimeZone(-5);

            Assert.AreEqual(-75, longitudeTimeZone);
        }

        [Test]
        public void UtcToUserTimeZoneShouldWork()
        {
            var utcDateTime = new DateTime(2014, 6, 1, 12, 0, 0, DateTimeKind.Utc);

            Assert.IsFalse(utcDateTime.IsDaylightSavingTime());
            var dateTime = Utils.UtcToUserTimeZone(utcDateTime, 0);

            Assert.IsTrue(Utils.IsGmtDaylightSavingTime(dateTime));
        }
    }
}
