using System;
using System.Linq;
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
                    tw = { h = 8 },
                    fw = { h = 18, m = 30 },
                    d = 30,
                    wd = string.Format("{1}{0}{0}{0}{0}{0}{1}", UIHelpers.WorkingDayTrue, UIHelpers.WorkingDayFalse)
                };

                ModelState.Clear();

                return View(model);
            }

            if (!model.wd.ToArray().Where(d => d == UIHelpers.WorkingDayTrue).Any())
            {
                ModelState.AddModelError("WorkingDays", "Please select at least one workday.");
            }

            if (!model.la.HasValue || !model.lo.HasValue)
            {
                double? latitude;
                double? longitude;

                _locationService.GetLocationFromIPAddress(Request.UserHostAddress, out latitude, out longitude);

                model.la = latitude;
                model.lo = longitude;
            }

            if (!model.la.HasValue || !model.lo.HasValue)
            {
                ModelState.AddModelError("Location", "Sorry, we couldn't figure out your location.");
            }

            var today = DateTime.Now.Date;


            var outboundCommuteStart = today.AddHours(model.tw.h).AddMinutes(model.tw.m);
            var returnCommuteStart = today.AddHours(model.fw.h).AddMinutes(model.fw.m);

            if (Utils.GetTimeOfDayDifference(outboundCommuteStart, returnCommuteStart) <= new TimeSpan(0, model.d, 0))
            {
                ModelState.AddModelError("JourneysOverlap", "Your journeys overlap one another, that can't be right.");
            }

            if (!TimeZones.Selected.ContainsKey(model.tz.Value))
            {
                ModelState.AddModelError("TimeZoneInvalid", string.Format("The selected time zone {0} is not valid.", model.tz.Value));
            }

            if (ModelState.IsValid)
            {
                var daylightHunter = new DaylightHunter();

                var outboundCommuteEnd = outboundCommuteStart.AddMinutes(model.d);
                var returnCommuteEnd = returnCommuteStart.AddMinutes(model.d);
                
                var location = new Location { Latitude = model.la.Value, Longitude = model.lo.Value };

                var toWorkDaylightInfo = daylightHunter.GetDaylight(location, model.tz.Value, outboundCommuteStart, outboundCommuteEnd);
                var fromWorkDaylightInfo = daylightHunter.GetDaylight(location, model.tz.Value, returnCommuteStart, returnCommuteEnd);

                var workingDays = model.wd.Select(d => d == UIHelpers.WorkingDayTrue).ToArray();

                model.tw.Daylights = Builders.BuildDaylightInfoModel(DateTime.Now.Date, toWorkDaylightInfo, Commute.ToWork, workingDays);
                model.fw.Daylights = Builders.BuildDaylightInfoModel(DateTime.Now.Date, fromWorkDaylightInfo, Commute.FromWork, workingDays);
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
