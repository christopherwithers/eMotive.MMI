using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using eMotive.Managers.Interfaces;
using eMotive.Managers.Objects;
using eMotive.MMI.Common;
using eMotive.MMI.Common.ActionFilters;
using eMotive.Services.Interfaces;
using Extensions;
using Ninject;

namespace eMotive.MMI.Controllers
{
    [Common.ActionFilters.Authorize(Roles = "Interviewer")]
    public class InterviewsController : Controller
    {
        private readonly ISessionManager signupManager;
        private readonly IPartialPageManager pageManager;

        public InterviewsController(ISessionManager _signupManager, IPartialPageManager _pageManager)
        {
            signupManager = _signupManager;
            pageManager = _pageManager;
        }

        [Inject]
        public IeMotiveConfigurationService ConfigurationService { get; set; }
        [Inject]
        public INotificationService NotificationService { get; set; }

       /* public ActionResult Disability()
        {
            var signups = signupManager.FetchSignupInformation(User.Identity.Name);

            var pageText = pageManager.FetchPartials(new[] { "Disability-Session-List-header", "Disability-Session-List-Footer" }).ToDictionary(k => k.Key, v => v.Text);
            signups.HeaderText = pageText["Disability-Session-List-header"];
            signups.FooterText = pageText["Disability-Session-List-Footer"];

       //     signups.HeaderText = pageManager.Fetch("Disability-Session-List-header").Text;
            
            return View(signups);
        }*/

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult Signups()
        {
            var signups = signupManager.FetchSignupInformation(User.Identity.Name);

            var pageText = pageManager.FetchPartials(new[] {"Session-List-header", "Session-List-Footer"}).ToDictionary(k => k.Key, v => v.Text);
            signups.HeaderText = pageText["Session-List-header"];
            signups.FooterText = pageText["Session-List-Footer"];

            return View(signups);
        }

        public ActionResult Slots(int? id)
        {
            var slots = signupManager.FetchSlotInformation(id.HasValue ? id.Value : -1, User.Identity.Name);

            if (slots != null)
            {
                var replacements = new Dictionary<string, string>(4)
                {
                    {"#interviewdate#", slots.Date.ToString("dddd d MMMM yyyy")},
                    {"#description#", slots.Description},
                    {"#group#", slots.Group.Name}
                };
                //Disability-Interview-Date-Page
                var sb = new StringBuilder(pageManager.Fetch("Interview-Date-Page").Text);

                foreach (var replacment in replacements)
                {
                    sb.Replace(replacment.Key, replacment.Value);
                }

                slots.HeaderText = sb.ToString();
                slots.FooterText = pageManager.Fetch("Interview-Date-Page-Footer").Text;
            }

            return View(slots);
        }

        [AjaxOnly]
        public CustomJsonResult SignupToSlot(int idSignup, int idSlot)
        {
            if (signupManager.SignupToSlot(idSignup, idSlot, User.Identity.Name))
            {
                var signup = signupManager.Fetch(idSignup);
                var slot = signup.Slots.Single(n => n.ID == idSlot);

                ApplicantSignupPush(signup.ID, signup.Slots.Sum(n => n.TotalPlacesAvailable),
                    signup.Slots.Sum(n => n.ApplicantsSignedUp.HasContent() ? n.TotalPlacesAvailable - n.ApplicantsSignedUp.Count() : n.TotalPlacesAvailable));

                ApplicantSlotPush(slot.ID, slot.TotalPlacesAvailable,
                    slot.ApplicantsSignedUp.HasContent() ? slot.TotalPlacesAvailable - slot.ApplicantsSignedUp.Count() : slot.TotalPlacesAvailable);

                return new CustomJsonResult
                    {
                        Data = new {success = true, message = "successfully signed up."}
                    };
            }

            var issues = NotificationService.FetchIssues();


                return new CustomJsonResult
                {
                    Data = new { success = false, message = issues }
                };
            
        }

        [AjaxOnly]
        public CustomJsonResult CancelSignupToSlot(int idSignup, int idSlot)
        {
            if (signupManager.CancelSignupToSlot(idSignup, idSlot, User.Identity.Name))
            {
                var signup = signupManager.Fetch(idSignup);
                var slot = signup.Slots.Single(n => n.ID == idSlot);

                ApplicantSignupPush(signup.ID, signup.Slots.Sum(n => n.TotalPlacesAvailable),
                    signup.Slots.Sum(n => n.ApplicantsSignedUp.HasContent() ? n.TotalPlacesAvailable - n.ApplicantsSignedUp.Count() : n.TotalPlacesAvailable));

                ApplicantSlotPush(slot.ID, slot.TotalPlacesAvailable,
                    slot.ApplicantsSignedUp.HasContent() ? slot.TotalPlacesAvailable - slot.ApplicantsSignedUp.Count() : slot.TotalPlacesAvailable);

                return new CustomJsonResult
                {
                    Data = new { success = true, message = "successfully cancelled appointment." }
                };
            }

            return new CustomJsonResult
            {
                Data = new { success = false, message = "An error occurred. The signup could not be cancelled." }
            };
        }

        private void ApplicantSignupPush(int _signupID, int _totalPlaces, int _remainingPlaces)
        {
            var pusher = new PusherServer.Pusher(ConfigurationService.PusherID(), ConfigurationService.PusherKey(), ConfigurationService.PusherSecret());

            var result = pusher.Trigger("SignupSelection", "PlacesChanged",
                                                    new
                                                    {
                                                        SignUpId = _signupID,
                                                        TotalPlaces = _totalPlaces,
                                                        PlacesAvailable = _remainingPlaces
                                                    });
        }

        private void ApplicantSlotPush(int _slotID, int _totalPlaces, int _remainingPlaces)
        {
            var pusher = new PusherServer.Pusher(ConfigurationService.PusherID(), ConfigurationService.PusherKey(), ConfigurationService.PusherSecret());

            var result = pusher.Trigger("SignupSelection", "SlotChanged",
                                                    new
                                                    {
                                                        SlotId = _slotID,
                                                        TotalPlaces = _totalPlaces,
                                                        PlacesAvailable = _remainingPlaces
                                                    });
        }

    }
}
