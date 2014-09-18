using System.Web.Mvc;
using eMotive.Managers.Interfaces;
using eMotive.MMI.Common;
using eMotive.MMI.Common.ActionFilters;
using eMotive.Models.Objects.Search;
using eMotive.SCE.Common;
using eMotive.Search.Interfaces;
using eMotive.Services.Interfaces;
using ServiceStack.Mvc;

namespace eMotive.MMI.Areas.Admin.Controllers
{
    public class SettingsController : ServiceStackController
    {
        private readonly INewsManager newsManager;
        private readonly IUserManager userManager;
        private readonly IRoleManager roleManager;
        private readonly ISearchManager searchManager;
        private readonly IEmailService emailService;
        private readonly IPageManager pageManager;
        private readonly IPartialPageManager partialPageManager;
        private readonly ISessionManager sessionManager;
        public SettingsController(ISearchManager _searchManager, IUserManager _userManager, IRoleManager _rolemanager, 
                                  IPageManager _pageManager, IPartialPageManager _partialPageManager, INewsManager _newsManager, IEmailService _emailService, ISessionManager _sessionManager)
        {
            newsManager = _newsManager;
            userManager = _userManager;
            roleManager = _rolemanager;
            searchManager = _searchManager;
            emailService = _emailService;
            pageManager = _pageManager;
            partialPageManager = _partialPageManager;
            sessionManager = _sessionManager;
        }

        [Common.ActionFilters.Authorize(Roles = "Super Admin, Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Common.ActionFilters.Authorize(Roles = "Super Admin, Admin")]
        public ActionResult Search()
        {
            var stats = new SearchStatistics {NumberOfDocuments = searchManager.NumberOfDocuments()};

            return View(stats);
        }

        [Common.ActionFilters.Authorize(Roles = "Super Admin, Admin")]
        public ActionResult Site()
        {
            return View();
        }


        [AjaxOnly]
        [Common.ActionFilters.Authorize(Roles="Super Admin, Admin")]
        public CustomJsonResult ReindexAllDocuments()
        {
            searchManager.DeleteAll();
            newsManager.ReindexSearchRecords();
            roleManager.ReindexSearchRecords();
            userManager.ReindexSearchRecords();
            emailService.ReindexSearchRecords();
            pageManager.ReindexSearchRecords();
            sessionManager.ReindexSearchRecords();
            partialPageManager.ReindexSearchRecords();

            return new CustomJsonResult
            {
                Data = new { success = true }
            };
        }

    }
}
