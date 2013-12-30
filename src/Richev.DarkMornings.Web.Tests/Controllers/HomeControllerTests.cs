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
            var model = new CommuteInfo();

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("Location"));
        }

        [Test]
        public void IndexReturnsWorkingDaysErrorWhenNoWorkingDaysSet()
        {
            var model = new CommuteInfo();

            _homeController.Index(model);

            Assert.IsTrue(_homeController.ModelState.Keys.Contains("WorkingDays"));
        }

        [Test]
        public void IndexReturnsModelWithLocation()
        {
            double? latitude = 10;
            double? longitude = 20;

            _locationServiceMock.Setup(m => m.GetLocationFromIPAddress(null, out latitude, out longitude));

            var model = new CommuteInfo { WorkingDays = { Monday = true } };

            var actionResult = _homeController.Index(model);

            var returnedModel = (CommuteInfo)((ViewResult)actionResult).Model;

            Assert.IsTrue(_homeController.ModelState.IsValid);
            Assert.AreEqual(latitude, returnedModel.Latitude);
            Assert.AreEqual(longitude, returnedModel.Longitude);
        }
    }
}
