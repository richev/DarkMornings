using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using Richev.DarkMornings.Web.Controllers;
using Richev.DarkMornings.Web.Models;
using Richev.DarkMornings.Web.Services;

namespace Richev.DarkMornings.Web.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<ILocationService> _locationServiceMock;

        private HomeController _homeController;

        private readonly string _workingDaysMondayOnly = string.Format("{1}{0}{1}{1}{1}{1}{1}", UIHelpers.WorkingDayTrue, UIHelpers.WorkingDayFalse);

        private double? _latitude = 10;
        private double? _longitude = 20;

        [SetUp]
        public void SetUp()
        {
            _latitude = 10;
            _longitude = 20;

            _locationServiceMock = new Mock<ILocationService>();
            _locationServiceMock.Setup(m => m.GetLocationFromIPAddress(null, out _latitude, out _longitude));

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            _homeController = new HomeController(_locationServiceMock.Object);
            _homeController.ControllerContext = new ControllerContext(context.Object, new RouteData(), _homeController);
        }

        [Test]
        [Ignore("Need to sort out the Moq")]
        public void IndexReturnsLocationErrorWhenLocationNotFound()
        {
            var model = new CommuteInfoModel { tw = { h = 7 }, tz = 0, wd = _workingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("Location"));
        }

        [Test]
        public void IndexReturnsWorkingDaysErrorWhenNoWorkingDaysSet()
        {
            var model = new CommuteInfoModel { tw = { h = 7 }, tz = 0 };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("WorkingDays"));
        }

        [Test]
        public void IndexReturnsJourneysOverlapErrorIfJourneysAreTheSame()
        {
            var model = new CommuteInfoModel { tw = { h = 7 }, fw = { h = 7 }, d = 30, tz = 0, wd = _workingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexReturnsJourneysOverlapErrorIfJourneysOverlap()
        {
            var model = new CommuteInfoModel { tw = { h = 7, }, fw = { h = 7, m = 30 }, d = 60, tz = 0, wd = _workingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexReturnsJourneysOverlapErrorIfJourneysTouch()
        {
            var model = new CommuteInfoModel { tw = { h = 7 }, fw = { h = 7, m = 30 }, d = 30, tz = 0, wd = _workingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexDoesNotReturnJourneysOverlapErrorIfJourneysDoNotOverlap()
        {
            var model = new CommuteInfoModel { tw = { h = 7 }, fw = { h = 18, m = 30 }, d = 30, tz = 0, wd = _workingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsFalse(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexReturnsTimeZoneErrorIfTimeZoneInvalid()
        {
            var model = new CommuteInfoModel { tw = { h = 7 }, fw = { h = 8 }, d = 30, tz = 0.1, wd = _workingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("TimeZoneInvalid"));
        }

        [Test]
        public void IndexDoesNotCallLocationServiceIfLocationIsSet()
        {
            var model = new CommuteInfoModel { tw = { h = 7 }, wd = _workingDaysMondayOnly, tz = 0 };

            var actionResult = _homeController.Index(model);

            var returnedModel = (CommuteInfoModel)((ViewResult)actionResult).Model;

            _locationServiceMock.Verify(s => s.GetLocationFromIPAddress(null, out _latitude, out _longitude), Times.Once());
            Assert.IsTrue(_homeController.ModelState.IsValid);
            Assert.AreEqual(_latitude, returnedModel.la);
            Assert.AreEqual(_longitude, returnedModel.lo);
        }

        [Test]
        public void IndexGetsLocationfromLocationServiceIfNotSet()
        {
            var toWorkHour = 7;
            var toWorkMinutes = 15;
            var fromWorkHour = 8;
            var fromWorkMinutes = 30;
            double? timeZone = 1;

            var model = new CommuteInfoModel
                        {
                            tw = { h = toWorkHour, m = toWorkMinutes },
                            fw = { h = fromWorkHour, m = fromWorkMinutes },
                            wd = _workingDaysMondayOnly, 
                            la = _latitude, 
                            lo = _longitude,
                            tz = timeZone
                        };

            var actionResult = _homeController.Index(model);

            var returnedModel = (CommuteInfoModel)((ViewResult)actionResult).Model;

            _locationServiceMock.Verify(s => s.GetLocationFromIPAddress(null, out _latitude, out _longitude), Times.Never());
            Assert.IsTrue(_homeController.ModelState.IsValid);
            Assert.AreEqual(toWorkHour, returnedModel.tw.h);
            Assert.AreEqual(toWorkMinutes, returnedModel.tw.m);
            Assert.AreEqual(fromWorkHour, returnedModel.fw.h);
            Assert.AreEqual(fromWorkMinutes, returnedModel.fw.m);
            Assert.AreEqual(_workingDaysMondayOnly, returnedModel.wd);
            Assert.AreEqual(_latitude, returnedModel.la);
            Assert.AreEqual(_longitude, returnedModel.lo);
            Assert.AreEqual(timeZone, returnedModel.tz);
        }
    }
}
