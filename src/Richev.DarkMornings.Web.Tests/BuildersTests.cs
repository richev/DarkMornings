using System;
using NUnit.Framework;

namespace Richev.DarkMornings.Web.Tests
{
    [TestFixture]
    public class BuildersTests
    {
        private readonly DateTime _today = new DateTime(2014, 2, 3, 21, 38, 0); // a Monday

        private Core.DaylightInfo _daylights;

        private bool[] _workingDays;

        [SetUp]
        public void SetUp()
        {
            _daylights = new Core.DaylightInfo { TransitionType = Core.DaylightTransition.SunRise };

            _workingDays = new[] { false, true, true, true, true, true, false }; // Mon-Fri
        }

        [Test]
        public void ShouldBuildDaylightInfoWhenNextDaylightTransitionIsNull()
        {
            _daylights.IsCurrentlyInDaylight = true;

            var daylights = Builders.BuildDaylights(_today, _daylights, Commute.FromWork, _workingDays);

            Assert.IsNull(daylights.NextWorkingDayDaylightTransition);
            Assert.AreEqual(Commute.FromWork, daylights.CommuteType);
            Assert.AreEqual(0, daylights.NumberOfDaysToTransition);
            Assert.IsTrue(daylights.IsCurrentlyInDaylight);
        }

        [Test]
        public void ShouldBuildDaylightInfoWhenNextdaylightTransitionIsSetToWorkingDay()
        {
            _daylights.NextDaylightTransition = _today.AddDays(4); // a Friday

            var daylights = Builders.BuildDaylights(_today, _daylights, Commute.FromWork, _workingDays);

            Assert.AreEqual(_today.AddDays(4), daylights.NextWorkingDayDaylightTransition); // same day as the NextDaylightTransition
            Assert.AreEqual(Commute.FromWork, daylights.CommuteType);
            Assert.AreEqual(4, daylights.NumberOfDaysToTransition); // Tuesday to Friday
            Assert.IsFalse(daylights.IsCurrentlyInDaylight);
        }

        [Test]
        public void ShouldBuildDaylightInfoWhenNextdaylightTransitionIsSetToNonWorkingDay()
        {
            _daylights.NextDaylightTransition = _today.AddDays(6); // a Sunday

            var daylights = Builders.BuildDaylights(_today, _daylights, Commute.FromWork, _workingDays);

            Assert.AreEqual(_today.AddDays(4), daylights.NextWorkingDayDaylightTransition); // not quite the same as the NextDaylightTransition
            Assert.AreEqual(Commute.FromWork, daylights.CommuteType);
            Assert.AreEqual(4, daylights.NumberOfDaysToTransition); // Tuesday to Friday (ignoring the non-working Saturday and Sunday)
            Assert.IsFalse(daylights.IsCurrentlyInDaylight);
        }
    }
}
