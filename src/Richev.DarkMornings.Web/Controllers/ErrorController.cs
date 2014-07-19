using System.Web.Mvc;

namespace Richev.DarkMornings.Web.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public ActionResult NotFound()
        {
            return View();
        }

        [HttpGet]
        public ActionResult InternalError()
        {
            return View();
        }
    }
}