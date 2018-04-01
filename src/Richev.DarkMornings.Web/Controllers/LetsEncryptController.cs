using System.Web.Mvc;

namespace Richev.DarkMornings.Web.Controllers
{
    [RoutePrefix(".well-known")]
    public class LetsEncryptController : Common.Web.LetsEncryptController
    {
        [Route("acme-challenge/{challenge}/{filename?}")]
        public override ActionResult Index(string challenge, string filename = null)
        {
            return base.Index(challenge, filename);
        }
    }
}