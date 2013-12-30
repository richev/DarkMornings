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

            Assert.AreEqual("<p>You have <strong>10 more</strong> dark journeys to work, until <div class=\"calendar\"><span class=\"day\">Saturday</span> <span class=\"date\">1</span> <span class=\"month\">March</span></div></p>", journeyText.ToString());
        }

        [Test]
        public void GetJourneyTextShouldReturnCorrectTextInEdgeCase()
        {
            _daylightInfo.IsCurrentlyInDaylight = true;

            var journeyText = UIHelpers.GetJourneyText(_daylightInfo);

            Assert.AreEqual("<p>Your journey to work is always in the <strong>light</strong>.</p>", journeyText.ToString());
        }
    }
}
