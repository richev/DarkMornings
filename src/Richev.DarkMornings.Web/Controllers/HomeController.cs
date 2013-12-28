using System;
using System.Web.Mvc;
using CommuteInfo = Richev.DarkMornings.Web.Models.CommuteInfo;

namespace Richev.DarkMornings.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new CommuteInfo
            { 
                MorningCommute = { Hour = 7 },
                EveningCommute = { Hour = 18, Minute = 30 },
                WorkingDays = { Monday = true, Tuesday = true, Wednesday = true, Thursday = true, Friday = true }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(CommuteInfo model)
        {
            var sunHunter = new Core.DaylightHunter();

            var morningCommute = DateTime.Now.Date.AddHours(model.MorningCommute.Hour).AddMinutes(model.MorningCommute.Minute);
            var eveningCommute = DateTime.Now.Date.AddHours(model.EveningCommute.Hour).AddMinutes(model.EveningCommute.Minute);

            if (!model.Latitude.HasValue || !model.Longitude.HasValue)
            {
                double? latitude;
                double? longitude;

                Utils.GetLocationFromIPAddress(Request.UserHostAddress, out latitude, out longitude);

                model.Latitude = latitude;
                model.Longitude = longitude;
            }

            if (model.Latitude.HasValue && model.Longitude.HasValue)
            {
                var commuteInfo = sunHunter.GetDaylight(model.Latitude.Value, model.Longitude.Value, morningCommute, eveningCommute);

                model.MorningCommute.Daylights = Builders.BuildDaylights(DateTime.Now, commuteInfo.OutboundCommute, Commute.Outbound, model.WorkingDays);
                model.EveningCommute.Daylights = Builders.BuildDaylights(DateTime.Now, commuteInfo.ReturnCommute, Commute.Return, model.WorkingDays);

                model.Message = string.Format("Looks like you're at {0:0.0}, {1:0.0}", model.Latitude, model.Longitude);
            }
            else
            {
                model.Message = "Sorry, we couldn't figure out your location.";
            }

            return View(model);
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
