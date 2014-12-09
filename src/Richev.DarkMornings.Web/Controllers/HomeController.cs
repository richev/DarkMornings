using System;
using System.Web.Mvc;
using Richev.DarkMornings.Core;
using Richev.DarkMornings.Web.Models;
using Richev.DarkMornings.Web.Services;

namespace Richev.DarkMornings.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILocationService _locationService;

        public HomeController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public ActionResult Index(CommuteInfoModel model)
        {
            if (model == null || model.HasDefaultValues())
            {
                model = new CommuteInfoModel
                {
                    h = "0800",
                    w = "1830",
                    j = 30,
                    d = WorkingDays.Monday | WorkingDays.Tuesday | WorkingDays.Wednesday | WorkingDays.Thursday | WorkingDays.Friday
                };

                SetLocation(model);

                ModelState.Clear();

                return View(model);
            }

            if (!model.y.HasValue || !model.x.HasValue) // shouldn't happen, but could
            {
                SetLocation(model);
            }

            DateTime outboundCommuteStart;
            DateTime returnCommuteStart;
            model.Validate(ModelState, out outboundCommuteStart, out returnCommuteStart);

            if (ModelState.IsValid)
            {
                var daylightHunter = new DaylightHunter();

                var outboundCommuteEnd = outboundCommuteStart.AddMinutes(model.j);
                var returnCommuteEnd = returnCommuteStart.AddMinutes(model.j);
                
                var location = new Location { Latitude = model.y.Value, Longitude = model.x.Value };

                var toWorkDaylightInfo = daylightHunter.GetDaylight(location, model.z.Value, outboundCommuteStart, outboundCommuteEnd);
                var fromWorkDaylightInfo = daylightHunter.GetDaylight(location, model.z.Value, returnCommuteStart, returnCommuteEnd);

                model.ToWorkDaylights = Builders.BuildDaylightInfoModel(DateTime.Now.Date, toWorkDaylightInfo, Commute.ToWork, model.d);
                model.FromWorkDaylights = Builders.BuildDaylightInfoModel(DateTime.Now.Date, fromWorkDaylightInfo, Commute.FromWork, model.d);
            }

            return View(model);
        }

        private void SetLocation(CommuteInfoModel model)
        {
            var ipAddress = Request.IsLocal ? "82.44.44.102" : Request.UserHostAddress;

            var location = _locationService.GetLocationFromIPAddress(ipAddress);

            model.y = location.HasValue ? location.Value.Latitude : default(double?);
            model.x = location.HasValue ? location.Value.Longitude : default(double?);
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
    }
}
