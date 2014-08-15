using System.Web.Mvc;
using ServiceStack.Mvc;

namespace eMotive.MMI.Controllers
{
    public class PageController : ServiceStackController
    {
        //
        // GET: /Page/

        public ActionResult Index()
        {
            return View();
        }

    }
}
