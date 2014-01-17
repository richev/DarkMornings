﻿using System;
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

            var daylightsModel = Builders.BuildDaylightInfoModel(_today, _daylights, Commute.FromWork, _workingDays);

            Assert.IsNull(daylightsModel.NextWorkingDayDaylightTransition);
            Assert.AreEqual(Commute.FromWork, daylightsModel.CommuteType);
            Assert.AreEqual(0, daylightsModel.NumberOfDaysToTransition);
            Assert.IsTrue(daylightsModel.IsCurrentlyInDaylight);
            Assert.AreEqual(100, daylightsModel.PercentOfTheYearInTheDark);
        }

        [Test]
        public void ShouldBuildDaylightInfoWhenNextDaylightTransitionIsSetToWorkingDay()
        {
            _daylights.NextDaylightTransition = _today.AddDays(4); // a Friday

            var daylightsModel = Builders.BuildDaylightInfoModel(_today, _daylights, Commute.FromWork, _workingDays);

            Assert.AreEqual(_today.AddDays(4), daylightsModel.NextWorkingDayDaylightTransition); // same day as the NextDaylightTransition
            Assert.AreEqual(Commute.FromWork, daylightsModel.CommuteType);
            Assert.AreEqual(4, daylightsModel.NumberOfDaysToTransition); // Tuesday to Friday
            Assert.IsFalse(daylightsModel.IsCurrentlyInDaylight);
            Assert.AreEqual(100, daylightsModel.PercentOfTheYearInTheDark);
        }

        [Test]
        public void ShouldBuildDaylightInfoWhenNextDaylightTransitionIsSetToNonWorkingDay()
        {
            _daylights.NextDaylightTransition = _today.AddDays(6); // a Sunday

            var daylightsModel = Builders.BuildDaylightInfoModel(_today, _daylights, Commute.FromWork, _workingDays);

            Assert.AreEqual(_today.AddDays(7), daylightsModel.NextWorkingDayDaylightTransition); // not quite the same as the NextDaylightTransition
            Assert.AreEqual(Commute.FromWork, daylightsModel.CommuteType);
            Assert.AreEqual(5, daylightsModel.NumberOfDaysToTransition); // Tuesday to Friday, plus the next Monday
            Assert.IsFalse(daylightsModel.IsCurrentlyInDaylight);
            Assert.AreEqual(100, daylightsModel.PercentOfTheYearInTheDark);
        }

        [Test]
        public void ShouldBuildDaylightInfoWhenInDarknessNinetyPercentOfTheYear()
        {
            _daylights.IsCurrentlyInDaylight = false;
            _daylights.CommutesInDaylightPerYear = 36;

            var daylightsModel = Builders.BuildDaylightInfoModel(_today, _daylights, Commute.FromWork, _workingDays);

            Assert.AreEqual(90.14, Math.Round(daylightsModel.PercentOfTheYearInTheDark, 2));
        }
    }
}
