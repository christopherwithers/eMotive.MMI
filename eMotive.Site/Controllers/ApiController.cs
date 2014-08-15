using System.Web.Mvc;
using GoogleAnalyticsTracker.Web.Mvc;
using ServiceStack.Mvc;

namespace eMotive.MMI.Controllers
{
    [ActionTracking("UA-45423815-1", "MMI")]
    public class AnalyticsController : ServiceStackController
    {
        public JsonResult Create()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
