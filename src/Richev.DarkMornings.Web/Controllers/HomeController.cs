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
            CommuteInfoViewModel viewModel;

            if (model == null || model.HasDefaultValues())
            {
                viewModel = new CommuteInfoViewModel
                {
                    tw = { h = 7 },
                    fw = { h = 18, m = 30 },
                    WorkingDays = { Monday = true, Tuesday = true, Wednesday = true, Thursday = true, Friday = true }
                };

                return View(viewModel);
            }

            viewModel = new CommuteInfoViewModel();

            if (!model.wd.ToArray().Where(d => d == 'x').Any())
            {
                ModelState.AddModelError("WorkingDays", "Please select at least one day of the week.");
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

            viewModel.WorkingDays = Builders.BuildWorkingDays(model.wd);

            if (ModelState.IsValid)
            {
                var sunHunter = new Core.DaylightHunter();

                var morningCommute = DateTime.Now.Date.AddHours(model.tw.h).AddMinutes(model.tw.m);
                var eveningCommute = DateTime.Now.Date.AddHours(model.fw.h).AddMinutes(model.fw.m);

                var commuteInfo = sunHunter.GetDaylight(viewModel.la.Value, viewModel.lo.Value, morningCommute, eveningCommute);

                viewModel.tw.Daylights = Builders.BuildDaylights(DateTime.Now, commuteInfo.ToWork, Commute.ToWork, viewModel.WorkingDays);
                viewModel.fw.Daylights = Builders.BuildDaylights(DateTime.Now, commuteInfo.FromWork, Commute.FromWork, viewModel.WorkingDays);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
    }
}
