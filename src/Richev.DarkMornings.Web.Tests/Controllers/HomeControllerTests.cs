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

        private const WorkingDays WorkingDaysMondayOnly = WorkingDays.Monday;

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
            var model = new CommuteInfoModel { h = "0700", z = 0, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("Location"));
        }

        [Test]
        public void IndexReturnsWorkingDaysErrorWhenNoWorkingDaysSet()
        {
            var model = new CommuteInfoModel { h = "0700", w = "1800", z = 0 };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("WorkingDays"));
        }

        [Test]
        public void IndexReturnsJourneysOverlapErrorIfJourneysAreTheSame()
        {
            var model = new CommuteInfoModel { h = "0700", w = "0700", j = 30, z = 0, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexReturnsJourneysOverlapErrorIfJourneysOverlap()
        {
            var model = new CommuteInfoModel { h = "0700", w = "0730", j = 60, z = 0, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexReturnsJourneysOverlapErrorIfJourneysTouch()
        {
            var model = new CommuteInfoModel { h = "0700", w = "0730", j = 30, z = 0, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexDoesNotReturnJourneysOverlapErrorIfJourneysDoNotOverlap()
        {
            var model = new CommuteInfoModel { h = "0700", w = "1830", j = 30, z = 0, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsFalse(_homeController.ModelState.Keys.Contains("JourneysOverlap"));
        }

        [Test]
        public void IndexReturnsTimeZoneErrorIfTimeZoneInvalid()
        {
            var model = new CommuteInfoModel { h = "0700", w = "0800", j = 30, z = 0.1, d = WorkingDaysMondayOnly };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("TimeZoneInvalid"));
        }

        [Test]
        public void IndexDoesNotCallLocationServiceIfLocationIsSet()
        {
            var model = new CommuteInfoModel { h = "0700", w = "1800", d = WorkingDaysMondayOnly, z = 0 };

            var actionResult = _homeController.Index(model);

            var returnedModel = (CommuteInfoModel)((ViewResult)actionResult).Model;

            _locationServiceMock.Verify(s => s.GetLocationFromIPAddress(null, out _latitude, out _longitude), Times.Once());
            Assert.IsTrue(_homeController.ModelState.IsValid);
            Assert.AreEqual(_latitude, returnedModel.y);
            Assert.AreEqual(_longitude, returnedModel.x);
        }

        [Test]
        public void IndexGetsLocationfromLocationServiceIfNotSet()
        {
            var toWork = "0715";
            var fromWork = "0830";
            double? timeZone = 1;

            var model = new CommuteInfoModel
                        {
                            h = toWork,
                            w = fromWork,
                            d = WorkingDaysMondayOnly, 
                            y = _latitude, 
                            x = _longitude,
                            z = timeZone
                        };

            var actionResult = _homeController.Index(model);

            var returnedModel = (CommuteInfoModel)((ViewResult)actionResult).Model;

            _locationServiceMock.Verify(s => s.GetLocationFromIPAddress(null, out _latitude, out _longitude), Times.Never());
            Assert.IsTrue(_homeController.ModelState.IsValid);
            Assert.AreEqual(toWork, returnedModel.h);
            Assert.AreEqual(fromWork, returnedModel.w);
            Assert.AreEqual(WorkingDaysMondayOnly, returnedModel.d);
            Assert.AreEqual(_latitude, returnedModel.y);
            Assert.AreEqual(_longitude, returnedModel.x);
            Assert.AreEqual(timeZone, returnedModel.z);
        }
    }
}
