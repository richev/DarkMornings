﻿using System;
using NUnit.Framework;
using Richev.DarkMornings.Web.Models;

namespace Richev.DarkMornings.Web.Tests
{
    [TestFixture]
    public class UIHelpersTests
    {
        private DaylightInfo _daylightInfo;

        [SetUp]
        public void SetUp()
        {
            _daylightInfo = new DaylightInfo();
        }

        [Test]
        public void GetJourneyTextShouldReturnCorrectTextInTypicalScenario()
        {
            _daylightInfo.NextWorkingDayDaylightTransition = new DateTime(2014, 3, 1);
            _daylightInfo.NumberOfDaysToTransition = 10;

            var journeyText = UIHelpers.GetJourneyText(_daylightInfo);

            Assert.AreEqual("<span class=\"days-more\"><i class=\"fa fa-moon-o\"></i><i class=\"fa fa-arrow-circle-o-right commute-direction\"></i><span class=\"count\">10 more</span><span>dark journeys to work</span></span><span class=\"until\"><span>until</span></span><span class=\"calendar\"><span class=\"day\">Saturday</span> <span class=\"date\">1</span> <span class=\"month\">March</span></span>", journeyText.ToString());
        }

        [Test]
        public void GetJourneyTextShouldReturnCorrectTextInEdgeCase()
        {
            _daylightInfo.IsCurrentlyInDaylight = true;

            var journeyText = UIHelpers.GetJourneyText(_daylightInfo);

            Assert.AreEqual("<span><i class=\"fa fa-sun-o\"></i>Your journey to work is always in the <strong>light</strong></span>", journeyText.ToString());
        }
    }
}
