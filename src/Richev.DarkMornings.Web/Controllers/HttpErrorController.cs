using System;
using System.Web.Mvc;

namespace Richev.DarkMornings.Web.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HttpErrorController : Controller
    {
        [HttpGet]
        public ActionResult Index(int? statusCode)
        {
            if (statusCode.HasValue && Enum.IsDefined(typeof(System.Net.HttpStatusCode), statusCode.Value))
            {
                Response.StatusCode = statusCode.Value;
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
            }

            return View();
        }

    }
}