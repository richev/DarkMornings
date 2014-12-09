using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using Richev.DarkMornings.Core;
using Richev.DarkMornings.Web.Controllers;
using Richev.DarkMornings.Web.Models;
using Richev.DarkMornings.Web.Services;

namespace Richev.DarkMornings.Web.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<IGeoService> _geoServiceMock;

        private HomeController _homeController;

        private const WorkingDays WorkingDaysMondayOnly = WorkingDays.Monday;

        private Location _location = new Location { Latitude = 10, Longitude = 20 };

        [SetUp]
        public void SetUp()
        {
            _geoServiceMock = new Mock<IGeoService>();
            _geoServiceMock.Setup(m => m.GetLocationFromIPAddress(null)).Returns(_location);
            _geoServiceMock.Setup(m => m.GetTimeZoneId(_location)).Returns("GMT Standard Time");

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            _homeController = new HomeController(_geoServiceMock.Object);
            _homeController.ControllerContext = new ControllerContext(context.Object, new RouteData(), _homeController);
        }

        [Test]
        [Ignore("Need to sort out the Moq")]
        public void IndexReturnsLocationErrorWhenLocationNotFound()
        {
            var model = new CommuteInfoModel { h = "0700", d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("Location"));
        }

        [Test]
        public void IndexReturnsWorkingDaysErrorWhenNoWorkingDaysSet()
        {
            var model = new CommuteInfoModel { h = "0700", w = "1800" };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("WorkingDays"));
        }

        [Test]
        public void IndexReturnsJourneysOverlapErrorIfJourneysAreTheSame()
        {
            var model = new CommuteInfoModel { h = "0700", w = "0700", j = 30, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexReturnsJourneysOverlapErrorIfJourneysOverlap()
        {
            var model = new CommuteInfoModel { h = "0700", w = "0730", j = 60, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexReturnsJourneysOverlapErrorIfJourneysTouch()
        {
            var model = new CommuteInfoModel { h = "0700", w = "0730", j = 30, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexDoesNotReturnJourneysOverlapErrorIfJourneysDoNotOverlap()
        {
            var model = new CommuteInfoModel { h = "0700", w = "1830", j = 30, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsFalse(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexDoesNotCallLocationServiceIfLocationIsSet()
        {
            var model = new CommuteInfoModel { h = "0700", w = "1800", d = WorkingDaysMondayOnly };

            var actionResult = _homeController.Index(model);

            var returnedModel = (CommuteInfoModel)((ViewResult)actionResult).Model;

            _geoServiceMock.Verify(s => s.GetLocationFromIPAddress(null), Times.Once());
            Assert.IsTrue(_homeController.ModelState.IsValid);
            Assert.AreEqual(_location.Latitude, returnedModel.y);
            Assert.AreEqual(_location.Longitude, returnedModel.x);
        }

        [Test]
        public void IndexGetsLocationfromLocationServiceIfNotSet()
        {
            var toWork = "0715";
            var fromWork = "0830";

            var model = new CommuteInfoModel
                        {
                            h = toWork,
                            w = fromWork,
                            d = WorkingDaysMondayOnly,
                            y = _location.Latitude,
                            x = _location.Longitude,
                        };

            var actionResult = _homeController.Index(model);

            var returnedModel = (CommuteInfoModel)((ViewResult)actionResult).Model;

            _geoServiceMock.Verify(s => s.GetLocationFromIPAddress(null), Times.Never());
            Assert.IsTrue(_homeController.ModelState.IsValid);
            Assert.AreEqual(toWork, returnedModel.h);
            Assert.AreEqual(fromWork, returnedModel.w);
            Assert.AreEqual(WorkingDaysMondayOnly, returnedModel.d);
            Assert.AreEqual(_location.Latitude, returnedModel.y);
            Assert.AreEqual(_location.Longitude, returnedModel.x);
        }
    }
}
