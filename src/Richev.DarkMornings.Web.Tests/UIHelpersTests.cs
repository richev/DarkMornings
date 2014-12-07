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
        public void FormatCommuteTimeWorks()
        {
            var commuteTime = "1205";

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
                            ToWorkDaylights = new DaylightInfoModel
                                              {
                                                  IsCurrentlyInDaylight = false,
                                                  NumberOfDaysToTransition = 13,
                                                  CommuteType = Commute.ToWork,
                                                  NextWorkingDayDaylightTransition = new DateTime(2014, 2, 1)
                                              },
                            FromWorkDaylights = new DaylightInfoModel
                                                {
                                                    IsCurrentlyInDaylight = true,
                                                    NumberOfDaysToTransition = 54,
                                                    CommuteType = Commute.FromWork,
                                                    NextWorkingDayDaylightTransition = new DateTime(2014, 2, 1)
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
                            ToWorkDaylights = new DaylightInfoModel
                                              {
                                                  IsCurrentlyInDaylight = false,
                                                  CommuteType = Commute.ToWork
                                              },
                            FromWorkDaylights = new DaylightInfoModel
                                                {
                                                    IsCurrentlyInDaylight = true,
                                                    NumberOfDaysToTransition = 54,
                                                    CommuteType = Commute.FromWork,
                                                    NextWorkingDayDaylightTransition = new DateTime(2014, 2, 1)
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
                            ToWorkDaylights = new DaylightInfoModel
                                              {
                                                  IsCurrentlyInDaylight = false,
                                                  CommuteType = Commute.ToWork,
                                                  NumberOfDaysToTransition = 54,
                                                  NextWorkingDayDaylightTransition = new DateTime(2014, 2, 1)
                                              },
                            FromWorkDaylights = new DaylightInfoModel
                                                {
                                                    IsCurrentlyInDaylight = true,
                                                    CommuteType = Commute.FromWork,
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
                            ToWorkDaylights = new DaylightInfoModel
                                              {
                                                  IsCurrentlyInDaylight = false,
                                                  CommuteType = Commute.ToWork,
                                              },
                            FromWorkDaylights = new DaylightInfoModel
                                                {
                                                    IsCurrentlyInDaylight = true,
                                                    CommuteType = Commute.FromWork,
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
                            ToWorkDaylights = new DaylightInfoModel
                                              {
                                                  IsCurrentlyInDaylight = false,
                                                  CommuteType = Commute.ToWork,
                                              },
                            FromWorkDaylights = new DaylightInfoModel
                                                {
                                                    IsCurrentlyInDaylight = false,
                                                    CommuteType = Commute.FromWork,
                                                }
                        };

            var tweetText = UIHelpers.GetTweetText(model);

            Assert.AreEqual(
                "My journeys to work and back home are always in the dark!",
                tweetText);
        }
    }
}
