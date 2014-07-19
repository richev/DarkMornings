using System.Web.Mvc;

namespace Richev.DarkMornings.Web.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
            return View();
        }

        [HttpGet]
        public ActionResult InternalError()
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            return View();
        }
    }
}