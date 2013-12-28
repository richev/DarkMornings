using System;
using NUnit.Framework;

namespace RichEv.DarkMornings.Core.Tests
{
    [TestFixture]
    public class SunTimesTests
    {
        [Test]
        [Ignore]
        public void ShouldGetSunriseAndSunsetTimes()
        {
            var suntimes = new SunTimes();

            DateTime rises;
            DateTime sets;

            suntimes.GetSunTimes(51, -3, new DateTime(2013, 12, 24), out rises, out sets);

            Assert.AreEqual(new DateTime(2013, 12, 24, 08, 13, 46), rises);
            Assert.AreEqual(new DateTime(2013, 12, 24, 16, 05, 37), sets);
        }
    }
}
