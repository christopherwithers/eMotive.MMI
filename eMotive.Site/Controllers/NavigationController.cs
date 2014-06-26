using System;
using System.Web.Mvc;
using eMotive.Managers.Interfaces;
using eMotive.Models.Objects.Menu;
using eMotive.Models.Objects.Users;

namespace eMotive.MMI.Controllers
{
    public class NavigationController : Controller
    {
       // private readonly INavigationServices navigationService;
        private readonly IUserManager userManager;

        private User user;


        public NavigationController(IUserManager _userManager)
        {
            userManager = _userManager;

        }

        public string UserWelcome()
        {
            if (user == null)
                return "<p>Welcome <b>";
            user = userManager.Fetch(User.Identity.Name);
            return string.Concat("<p>Welcome <b>", user.Forename, " ", user.Surname, "</b></p><p>", DateTime.Now.ToString("dddd d MMMM yyyy"), "</p>");
        }

        public PartialViewResult MainMenu()
        {
            return PartialView("_mainMenu", User.Identity.IsAuthenticated ? FetchMainMenu(true) : FetchMainMenu(false));
        }

        private Menu FetchMainMenu(bool _loggedIn)
        {

            if (!_loggedIn)
                return BuildLoggedOutMenu();

            //applicant menu
            return BuildApplicantMenu();
        }

        private Menu BuildLoggedOutMenu()
        {
            var menu = new Menu
            {
                ID = 1,
                Title = "LoggedOutMenu",
                MenuItems = new[]
                            {
                                 new MenuItem
                                    {
                                        ID = 1,
                                        Name = "MMI",
                                        URL = Url.Action("Login", "Account"),//"/SCE/Account/Login",
                                        Title = "MMI Public Homepage"
                                    }
                            }
            };

            return menu;
        }

        private Menu BuildApplicantMenu()
        {
            var menu = new Menu
            {
                ID = 1,
                Title = "ApplicantMenu",
                MenuItems = new[]
                            {
                                 new MenuItem
                                    {
                                        ID = 1,
                                        Name = "MMI Home",
                                        URL = Url.Action("Index","Home"),//"/SCE/Home/",
                                        Title = "MMI Home"
                                    },
                                 new MenuItem
                                    {
                                        ID = 2,
                                        Name = "Sessions",
                                        URL = Url.Action("Signups","Interviews"),//"/SCE/Interviews/Signups",
                                        Title = "View Session Slots"
                                    },
                                    new MenuItem
                                    {
                                        ID = 2,
                                        Name = "My Details",
                                        URL = Url.Action("InterviewerDetails","Account"),//"/SCE/Account/Details",
                                        Title = "My Details"
                                    },
                                 new MenuItem
                                    {
                                        ID = 2,
                                        Name = "Change Password",
                                        URL = Url.Action("Details","Account"),//"/SCE/Account/Details",
                                        Title = "Change Password"
                                    },                                 
                                 new MenuItem
                                    {
                                        ID = 2,
                                        Name = "Contact Us",
                                        URL = Url.Action("ContactUs","Home"),//"/SCE/Home/ContactUs",
                                        Title = "Our Contact Details"
                                    },
                                 new MenuItem
                                    {
                                        ID = 2,
                                        Name = "Logout",
                                        URL = Url.Action("Logout","Account"),//"/SCE/Account/Logout",
                                        Title = "Logout Of The SCE System"
                                    }

                            }
            };

            return menu;
        }

    }
}
