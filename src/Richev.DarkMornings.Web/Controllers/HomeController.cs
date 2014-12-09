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

                ModelState.Clear();

                return View(model);
            }

            if (!model.y.HasValue || !model.x.HasValue)
            {
                double? latitude;
                double? longitude;

                _locationService.GetLocationFromIPAddress(Request.UserHostAddress, out latitude, out longitude);

                model.y = latitude;
                model.x = longitude;
            }

            DateTime outboundCommuteStart;
            DateTime returnCommuteStart;
            model.Validate(ModelState, out outboundCommuteStart, out returnCommuteStart);

            if (ModelState.IsValid)
            {
                // TODO: Refactor from code in Builders

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

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
    }
}
