using System;
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

            Assert.AreEqual("<span class=\"days-more\"><span class=\"count\">10 more</span> dark journeys<i class=\"fa fa-moon-o\"></i> to work</span><span class=\"until\"><span>until</span></span><span class=\"calendar\"><span class=\"day\">Saturday</span> <span class=\"date\">1</span> <span class=\"month\">March</span></span>", journeyText.ToString());
        }

        [Test]
        public void GetJourneyTextShouldReturnCorrectTextInEdgeCase()
        {
            _daylightInfo.IsCurrentlyInDaylight = true;

            var journeyText = UIHelpers.GetJourneyText(_daylightInfo);

            Assert.AreEqual("<span>Your journey to work is always in the <strong>light <i class=\"fa fa-sun-o\"></i></strong></span>", journeyText.ToString());
        }
    }
}
