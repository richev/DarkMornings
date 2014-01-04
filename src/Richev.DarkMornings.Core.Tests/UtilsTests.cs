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
    }
}
