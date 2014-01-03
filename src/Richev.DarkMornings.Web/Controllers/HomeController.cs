using System;
using System.Linq;
using System.Web.Mvc;
using Richev.DarkMornings.Web.Models;
using Richev.DarkMornings.Web.Services;
using CommuteInfo = Richev.DarkMornings.Web.Models.CommuteInfoModel;

namespace Richev.DarkMornings.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILocationService _locationService;

        public HomeController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // TODO: Exclude WorkingDays from form submission

        [HttpGet]
        public ActionResult Index(CommuteInfo model)
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
                            wd = model.wd
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
                ModelState.AddModelError("Location", "Sorry, we couldn't figure out your location. Depending on your browser, you should see a prompt at the top (or bottom) of this screen saying that Dark Mornings wants to track your location. You'll have to accept this in order for this site to work.");
            }

            if (ModelState.IsValid)
            {
                var sunHunter = new Core.DaylightHunter();

                var morningCommute = DateTime.Now.Date.AddHours(model.tw.h).AddMinutes(model.tw.m);
                var eveningCommute = DateTime.Now.Date.AddHours(model.fw.h).AddMinutes(model.fw.m);

                var commuteInfo = sunHunter.GetDaylight(viewModel.la.Value, viewModel.lo.Value, morningCommute, eveningCommute);

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
