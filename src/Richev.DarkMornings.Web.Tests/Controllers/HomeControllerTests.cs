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

        [SetUp]
        public void SetUp()
        {
            _locationServiceMock = new Mock<ILocationService>();

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            _homeController = new HomeController(_locationServiceMock.Object);
            _homeController.ControllerContext = new ControllerContext(context.Object, new RouteData(), _homeController);
        }

        [Test]
        public void IndexReturnsLocationErrorWhenLocationNotFound()
        {
            var model = new CommuteInfoModel { tw = { h = 7 } };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("Location"));
        }

        [Test]
        public void IndexReturnsWorkingDaysErrorWhenNoWorkingDaysSet()
        {
            var model = new CommuteInfoModel { tw = { h = 7 } };

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("WorkingDays"));
        }

        [Test]
        public void IndexDoesNotCallLocationServiceIfLocationIsSet()
        {
            double? latitude = 10;
            double? longitude = 20;

            _locationServiceMock.Setup(m => m.GetLocationFromIPAddress(null, out latitude, out longitude));

            var model = new CommuteInfoModel { tw = { h = 7 }, wd = string.Format("{1}{0}{1}{1}{1}{1}{1}", UIHelpers.WorkingDayTrue, UIHelpers.WorkingDayFalse), tz = 0 };

            var actionResult = _homeController.Index(model);

            var returnedModel = (CommuteInfoModel)((ViewResult)actionResult).Model;

            _locationServiceMock.Verify(s => s.GetLocationFromIPAddress(null, out latitude, out longitude), Times.Once());
            Assert.IsTrue(_homeController.ModelState.IsValid);
            Assert.AreEqual(latitude, returnedModel.la);
            Assert.AreEqual(longitude, returnedModel.lo);
        }

        [Test]
        public void IndexGetsLocationfromLocationServiceIfNotSet()
        {
            var toWorkHour = 7;
            var toWorkMinutes = 15;
            var fromWorkHour = 8;
            var fromWorkMinutes = 30;
            var workingDays = string.Format("{1}{0}{1}{1}{1}{1}{1}", UIHelpers.WorkingDayTrue, UIHelpers.WorkingDayFalse);
            double? latitude = 10;
            double? longitude = 20;
            double? timeZone = 1;

            _locationServiceMock.Setup(m => m.GetLocationFromIPAddress(null, out latitude, out longitude));

            var model = new CommuteInfoModel
                        {
                            tw = { h = toWorkHour, m = toWorkMinutes },
                            fw = { h = fromWorkHour, m = fromWorkMinutes },
                            wd = workingDays, 
                            la = latitude, 
                            lo = longitude,
                            tz = timeZone
                        };

            var actionResult = _homeController.Index(model);

            var returnedModel = (CommuteInfoModel)((ViewResult)actionResult).Model;

            _locationServiceMock.Verify(s => s.GetLocationFromIPAddress(null, out latitude, out longitude), Times.Never());
            Assert.IsTrue(_homeController.ModelState.IsValid);
            Assert.AreEqual(toWorkHour, returnedModel.tw.h);
            Assert.AreEqual(toWorkMinutes, returnedModel.tw.m);
            Assert.AreEqual(fromWorkHour, returnedModel.fw.h);
            Assert.AreEqual(fromWorkMinutes, returnedModel.fw.m);
            Assert.AreEqual(workingDays, returnedModel.wd);
            Assert.AreEqual(latitude, returnedModel.la);
            Assert.AreEqual(longitude, returnedModel.lo);
            Assert.AreEqual(timeZone, returnedModel.tz);
        }
    }
}
