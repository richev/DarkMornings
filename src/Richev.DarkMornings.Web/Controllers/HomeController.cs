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
            CommuteInfoModel viewModel;

            if (model == null || model.HasDefaultValues())
            {
                viewModel = new CommuteInfoModel
                {
                    tw = { h = 8 },
                    fw = { h = 18, m = 30 },
                    wd = string.Format("{1}{0}{0}{0}{0}{0}{1}", UIHelpers.WorkingDayTrue, UIHelpers.WorkingDayFalse)
                };

                ModelState.Clear();

                return View(viewModel);
            }

            viewModel = new CommuteInfoModel
                        {
                            tw = model.tw,
                            fw = model.fw,
                            wd = model.wd,
                            tz = model.tz
                        };

            if (!model.wd.ToArray().Where(d => d == UIHelpers.WorkingDayTrue).Any())
            {
                ModelState.AddModelError("WorkingDays", "Please select at least one workday.");
            }

            if (!model.la.HasValue || !model.lo.HasValue)
            {
                double? latitude;
                double? longitude;

                _locationService.GetLocationFromIPAddress(Request.UserHostAddress, out latitude, out longitude);

                viewModel.la = latitude;
                viewModel.lo = longitude;
            }
            else
            {
                viewModel.la = model.la;
                viewModel.lo = model.lo;
            }

            if (!viewModel.la.HasValue || !viewModel.lo.HasValue)
            {
                ModelState.AddModelError("Location", "Sorry, we couldn't figure out your location.");
            }

            if (ModelState.IsValid)
            {
                var daylightHunter = new DaylightHunter();

                var today = DateTime.Now.Date;
                
                // TODO: Use the timezone offset

                var location = new Location { Latitude = viewModel.la.Value, Longitude = viewModel.lo.Value };
                var outboundCommuteAt = today.AddHours(model.tw.h).AddMinutes(model.tw.m);
                var returnCommuteAt = today.AddHours(model.fw.h).AddMinutes(model.fw.m);

                var commuteInfo = daylightHunter.GetDaylight(location, viewModel.tz.Value, outboundCommuteAt, returnCommuteAt);

                var workingDays = viewModel.wd.Select(d => d == UIHelpers.WorkingDayTrue).ToArray();

                viewModel.tw.Daylights = Builders.BuildDaylights(DateTime.Now, commuteInfo.ToWork, Commute.ToWork, workingDays);
                viewModel.fw.Daylights = Builders.BuildDaylights(DateTime.Now, commuteInfo.FromWork, Commute.FromWork, workingDays);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }
    }
}
