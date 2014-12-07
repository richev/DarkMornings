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
                    t = "0800",
                    f = "1830",
                    j = 30,
                    d = WorkingDays.Monday | WorkingDays.Tuesday | WorkingDays.Wednesday | WorkingDays.Thursday | WorkingDays.Friday
                };

                ModelState.Clear();

                return View(model);
            }

            if (model.d == 0)
            {
                ModelState.AddModelError("WorkingDays", "Please select at least one workday.");
            }

            if (!model.y.HasValue || !model.x.HasValue)
            {
                double? latitude;
                double? longitude;

                _locationService.GetLocationFromIPAddress(Request.UserHostAddress, out latitude, out longitude);

                model.y = latitude;
                model.x = longitude;
            }

            if (!model.y.HasValue || !model.x.HasValue)
            {
                ModelState.AddModelError("Location", "Sorry, we couldn't figure out your location.");
            }

            var today = DateTime.Now.Date;

            // TODO: Cleaner approach?
            DateTime outboundCommuteStart;
            if (UIHelpers.TryGetTime(model.t, out outboundCommuteStart))
            {
                today.AddHours(outboundCommuteStart.Hour).AddMinutes(outboundCommuteStart.Minute);
            }

            DateTime returnCommuteStart;
            if (UIHelpers.TryGetTime(model.f, out returnCommuteStart))
            {
                today.AddHours(returnCommuteStart.Hour).AddMinutes(returnCommuteStart.Minute);
            }

            if (Utils.GetTimeOfDayDifference(outboundCommuteStart, returnCommuteStart) <= new TimeSpan(0, model.j, 0))
            {
                ModelState.AddModelError("JourneysOverlap", "Your journeys overlap one another. That can't be right.");
            }

            if (!model.z.HasValue)
            {
                ModelState.AddModelError("TimeZoneInvalid", "No time zone is selected.");
            }
            else if (!TimeZones.Selected.ContainsKey(model.z.Value))
            {
                ModelState.AddModelError("TimeZoneInvalid", string.Format("The selected time zone {0} is not valid.", model.z.Value));
            }

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

                // TODO: Complete this later on
                //model.OtherLocations = Builders.BuildOtherLocations(model.wd, model.tw, model.fw, model.d);
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
