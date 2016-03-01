using System;
using System.Web.Mvc;
using Richev.DarkMornings.Core;
using Richev.DarkMornings.Web.Models;
using Richev.DarkMornings.Web.Services;

namespace Richev.DarkMornings.Web.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
#if !DEBUG
    [RequireHttps]
#endif
    public class HomeController : Controller
    {
        private readonly IGeoService _geoService;

        public HomeController(IGeoService geoService)
        {
            _geoService = geoService;
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
                try
                {
                    var daylightHunter = new DaylightHunter();

                    var outboundCommuteEnd = outboundCommuteStart.AddMinutes(model.j);
                    var returnCommuteEnd = returnCommuteStart.AddMinutes(model.j);

                    var location = new Location { Latitude = model.y.Value, Longitude = model.x.Value };

                    model.TimeZoneId = _geoService.GetTimeZoneId(location);

                    var toWorkDaylightInfo = daylightHunter.GetDaylight(location, model.TimeZoneId, outboundCommuteStart, outboundCommuteEnd);
                    var fromWorkDaylightInfo = daylightHunter.GetDaylight(location, model.TimeZoneId, returnCommuteStart, returnCommuteEnd);

                    model.ToWorkDaylights = Builders.BuildDaylightInfoModel(DateTime.Now.Date, toWorkDaylightInfo, Commute.ToWork, model.d);
                    model.FromWorkDaylights = Builders.BuildDaylightInfoModel(DateTime.Now.Date, fromWorkDaylightInfo, Commute.FromWork, model.d);
                }
                catch (UserDisplayableException ex)
                {
                    ModelState.AddModelError("UserDisplayable", ex.Message);
                }
            }

            return View(model);
        }

        private void SetLocation(CommuteInfoModel model)
        {
            var ipAddress = Request.IsLocal ? "82.44.44.102" : Request.UserHostAddress;

            var location = _geoService.GetLocationFromIPAddress(ipAddress);

            model.y = location.HasValue ? Math.Round(location.Value.Latitude, 2) : default(double?);
            model.x = location.HasValue ? Math.Round(location.Value.Longitude, 2) : default(double?);
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
    }
}
