using System.Web.Mvc;

namespace Richev.DarkMornings.Web.Controllers
{
    [RoutePrefix(".well-known")]
    public class LetsEncryptController : Common.Web.LetsEncryptController
    {
        [Route("acme-challenge/{challenge}")]
        public override ActionResult Index(string challenge)
        {
            return base.Index(challenge);
        }
    }
}