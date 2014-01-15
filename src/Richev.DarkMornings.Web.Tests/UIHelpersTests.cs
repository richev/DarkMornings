using System;
using NUnit.Framework;
using Richev.DarkMornings.Web.Models;

namespace Richev.DarkMornings.Web.Tests
{
    [TestFixture]
    public class UIHelpersTests
    {
        private DaylightInfoModel _daylightInfo;

        [SetUp]
        public void SetUp()
        {
            _daylightInfo = new DaylightInfoModel();
        }

        [Test]
        public void GetJourneyTextShouldReturnCorrectTextInTypicalScenario()
        {
            _daylightInfo.NextWorkingDayDaylightTransition = new DateTime(2014, 3, 1);
            _daylightInfo.NumberOfDaysToTransition = 10;

            var journeyText = UIHelpers.GetJourneyText(_daylightInfo);

            Assert.AreEqual(
@"<div class=""days-more"">
    <i class=""fa fa-moon-o""></i>
    <div class=""days-more-content"">
        <i class=""fa fa-arrow-circle-o-right commute-direction""></i><span class=""count"">10 more</span> <span>dark journeys</span>
    </div>
</div>
<div class=""until"">
    <span>to work,</span> until
</div>
<div class=""calendar"">
    <span class=""day"">Saturday</span>
    <span class=""date"">1</span>
    <span class=""month"">March</span>
</div>", journeyText.ToString());
        }

        [Test]
        public void GetJourneyTextShouldReturnCorrectTextInEdgeCase()
        {
            _daylightInfo.IsCurrentlyInDaylight = true;

            var journeyText = UIHelpers.GetJourneyText(_daylightInfo);

            Assert.AreEqual(
@"<div class=""days-more always"">
    <i class=""fa fa-sun-o""></i>
    <div class=""days-more-content always"">
        <i class=""fa fa-arrow-circle-o-right commute-direction""></i>Your journey to work is always in the light
    </div>
</div>", journeyText.ToString());
        }

        [Test]
        public void FormatCommuteTimeWorks()
        {
            var commuteTime = new CommuteTimeModel { h = 12, m = 5 };

            var formattedCommuteTime = UIHelpers.FormatCommuteTime(commuteTime);

            Assert.AreEqual("12:05", formattedCommuteTime);
        }

        [Test]
        public void FormatWorkingDaysWorksForSingleDay()
        {
            var workingDays = new[] { false, false, false, false, true, false, false };

            var formattedWorkingDays = UIHelpers.FormatWorkingDays(workingDays);

            Assert.AreEqual("Thursday", formattedWorkingDays);
        }

        [Test]
        public void FormatWorkingDaysWorksForMultipleDays()
        {
            var workingDays = new[] { false, true, true, false, true, false, false };

            var formattedWorkingDays = UIHelpers.FormatWorkingDays(workingDays);

            Assert.AreEqual("Monday, Tuesday and Thursday", formattedWorkingDays);
        }

        [Test]
        public void GetTweetTextShouldWorkForGeneralScenario()
        {
            var model = new CommuteInfoModel
                            {
                                tw = new CommuteTimeModel
                                         {
                                             Daylights = new DaylightInfoModel
                                                             {
                                                                 IsCurrentlyInDaylight = false,
                                                                 NumberOfDaysToTransition = 13,
                                                                 CommuteType = Commute.ToWork,
                                                                 NextWorkingDayDaylightTransition = new DateTime(2014, 2, 1)
                                                             }
                                         },
                                fw = new CommuteTimeModel
                                         {
                                             Daylights = new DaylightInfoModel
                                                             {
                                                                 IsCurrentlyInDaylight = true,
                                                                 NumberOfDaysToTransition = 54,
                                                                 CommuteType = Commute.FromWork,
                                                                 NextWorkingDayDaylightTransition = new DateTime(2014, 2, 1)
                                                             }
                                         }
                            };

            var tweetText = UIHelpers.GetTweetText(model);

            Assert.AreEqual(
                "I have 13 more dark journeys to work and 54 more light journeys back home!",
                tweetText);
        }

        [Test]
        public void GetTweetTextShouldWorkWhenJourneyToWorkAlwaysInDark()
        {
            var model = new CommuteInfoModel
            {
                tw = new CommuteTimeModel
                {
                    Daylights = new DaylightInfoModel
                    {
                        IsCurrentlyInDaylight = false,
                        CommuteType = Commute.ToWork
                    }
                },
                fw = new CommuteTimeModel
                {
                    Daylights = new DaylightInfoModel
                    {
                        IsCurrentlyInDaylight = true,
                        NumberOfDaysToTransition = 54,
                        CommuteType = Commute.FromWork,
                        NextWorkingDayDaylightTransition = new DateTime(2014, 2, 1)
                    }
                }
            };

            var tweetText = UIHelpers.GetTweetText(model);

            Assert.AreEqual(
                "My journey to work is always in the dark and I have 54 more light journeys back home!",
                tweetText);
        }

        [Test]
        public void GetTweetTextShouldWorkWhenJourneyFromWorkAlwaysInLight()
        {
            var model = new CommuteInfoModel
            {
                tw = new CommuteTimeModel
                {
                    Daylights = new DaylightInfoModel
                    {
                        IsCurrentlyInDaylight = false,
                        CommuteType = Commute.ToWork,
                        NumberOfDaysToTransition = 54,
                        NextWorkingDayDaylightTransition = new DateTime(2014, 2, 1)
                    }
                },
                fw = new CommuteTimeModel
                {
                    Daylights = new DaylightInfoModel
                    {
                        IsCurrentlyInDaylight = true,
                        CommuteType = Commute.FromWork,
                    }
                }
            };

            var tweetText = UIHelpers.GetTweetText(model);
            
            Assert.AreEqual(
                "I have 54 more dark journeys to work and my journey back home is always in the light!",
                tweetText);
        }

        [Test]
        public void GetTweetTextShouldWorkWhenJourneyToWorkAlwaysInDarkAndFromWorkAlwaysInLight()
        {
            var model = new CommuteInfoModel
            {
                tw = new CommuteTimeModel
                {
                    Daylights = new DaylightInfoModel
                    {
                        IsCurrentlyInDaylight = false,
                        CommuteType = Commute.ToWork,
                    }
                },
                fw = new CommuteTimeModel
                {
                    Daylights = new DaylightInfoModel
                    {
                        IsCurrentlyInDaylight = true,
                        CommuteType = Commute.FromWork,
                    }
                }
            };

            var tweetText = UIHelpers.GetTweetText(model);

            Assert.AreEqual(
                "My journey to work is always in the dark and my journey back home is always in the light!",
                tweetText);
        }

        [Test]
        public void GetTweetTextShouldWorkWhenJourneyToWorkAlwaysInDarkAndFromWorkAlwaysInDark()
        {
            var model = new CommuteInfoModel
            {
                tw = new CommuteTimeModel
                {
                    Daylights = new DaylightInfoModel
                    {
                        IsCurrentlyInDaylight = false,
                        CommuteType = Commute.ToWork,
                    }
                },
                fw = new CommuteTimeModel
                {
                    Daylights = new DaylightInfoModel
                    {
                        IsCurrentlyInDaylight = false,
                        CommuteType = Commute.FromWork,
                    }
                }
            };

            var tweetText = UIHelpers.GetTweetText(model);

            Assert.AreEqual(
                "My journeys to work and back home are always in the dark!",
                tweetText);
        }
    }
}
