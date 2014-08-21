using System.Linq;
using System.Web.Mvc;
using eMotive.Managers.Interfaces;
using eMotive.Models.Objects.SignupsMod;
using eMotive.Services.Interfaces;
//using Ninject;
using ServiceStack.Mvc;

namespace eMotive.MMI.Controllers
{
    public class SessionsController : ServiceStackController
    {
        private readonly ISessionManager signupManager;
        private readonly IPartialPageManager pageManager;

        public SessionsController(ISessionManager _signupManager, IPartialPageManager _pageManager)
        {
            signupManager = _signupManager;
            pageManager = _pageManager;
        }

       // [Inject]
        public IeMotiveConfigurationService ConfigurationService { get; set; }
       // [Inject]
        public INotificationService NotificationService { get; set; }

        public ActionResult Index(int id)
        {
            var signup = signupManager.FetchSignupInformation(User.Identity.Name, id);

            var pageText = pageManager.FetchPartials(new[] { "Session-List-header", "Session-List-Footer" }).ToDictionary(k => k.Key, v => v.Text);
            signup.HeaderText = pageText["Session-List-header"];
            signup.FooterText = pageText["Session-List-Footer"];

            return View(signup);
        }

        public ActionResult Signups()
        {

           /* var pageText = pageManager.FetchPartials(new[] { "Session-List-header", "Session-List-Footer" }).ToDictionary(k => k.Key, v => v.Text);
            signups.HeaderText = pageText["Session-List-header"];
            signups.FooterText = pageText["Session-List-Footer"];*/
            var pageText = pageManager.FetchPartials(new[] { "Session-List-header", "Session-List-Footer" }).ToDictionary(k => k.Key, v => v.Text);

            var userSignup = new UserSignupView
            {
                LoggedInUser = User.Identity.Name ?? string.Empty,
                Signups = signupManager.FetchAllM(),
                HeaderText = pageText["Session-List-header"] ?? string.Empty,
                FooterText = pageText["Session-List-Footer"] ?? string.Empty
            };

            userSignup.Initialise(); // Perhaps a bit messy, but will think how to tidy this up! Pre-optimising?

            return View(userSignup);
        }

    }
}
