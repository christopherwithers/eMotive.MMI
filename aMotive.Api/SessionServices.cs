using System.Collections.Generic;
using eMotive.Managers.Interfaces;
using eMotive.Models.Objects.SignupsMod;
using eMotive.Services.Interfaces;
using ServiceStack.Common.Extensions;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;


namespace eMotive.Api
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public T Result { get; set; }
    }

  /*  [Route("/Courses/New", "GET")]
    public class NewCourse
    {
    }*/

    [Route("/Sessions")]
    [Route("/Sessions/{Ids}")]
    public class GetSessions
    {
        public int[] Ids { get; set; }
    }

    [Route("/Sessions/Signup/Add", "POST")]
    [Route("/Sessions/Signup/Remove", "DELETE")]
    public class SlotSignup
    {
        public int IdSignup { get; set; }
        public int IdSlot { get; set; }
        public string Username { get; set; }
    }

  //  [Authenticate] 
    public class SessionService : Service
    {
        private readonly ISessionManager _sessionManager;

        public SessionService(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public INotificationService NotificationService { get; set; }
 
        public object Get(GetSessions request)
        {
            var result = request.Ids.IsEmpty()
                ? null
                : _sessionManager.FetchM(request.Ids[0]);

            var success = result != null;

            var issues = NotificationService.FetchIssues(); //TODO: how to deal with errors when going directly into the api?? perhaps organise messages better?

            return new ServiceResult<Signup>
            {
                Success = success,
                Result = result,
                Errors = issues
            };
        }

        public object Post(SlotSignup request)
        {
            if (_sessionManager.SignupToSlot(request.IdSignup, request.IdSlot, request.Username))
            {
               // var signup = _sessionManager.Fetch(request.IdSignup);
               // var slot = signup.Slots.Single(n => n.ID == request.IdSlot);

               // ApplicantSignupPush(signup.ID, signup.Slots.Sum(n => n.TotalPlacesAvailable),
                //    signup.Slots.Sum(n => n.ApplicantsSignedUp.HasContent() ? n.TotalPlacesAvailable - n.ApplicantsSignedUp.Count() : n.TotalPlacesAvailable));

               // ApplicantSlotPush(slot.ID, slot.TotalPlacesAvailable,
                //    slot.ApplicantsSignedUp.HasContent() ? slot.TotalPlacesAvailable - slot.ApplicantsSignedUp.Count() : slot.TotalPlacesAvailable);

                return new ServiceResult<bool>
                {
                  //  Data = new { success = true, message = "successfully signed up." }
                    Success = true,
                    Result = true,
                    Errors = null
                };
            }

            var issues = NotificationService.FetchIssues();

            return new ServiceResult<bool>
            {

                Success = false,
                Result = false,
                Errors = issues
            };
        }

        public object Delete(SlotSignup request)
        {
            if (_sessionManager.CancelSignupToSlot(request.IdSignup, request.IdSlot, request.Username))
            {
                // var signup = _sessionManager.Fetch(request.IdSignup);
                // var slot = signup.Slots.Single(n => n.ID == request.IdSlot);

                // ApplicantSignupPush(signup.ID, signup.Slots.Sum(n => n.TotalPlacesAvailable),
                //    signup.Slots.Sum(n => n.ApplicantsSignedUp.HasContent() ? n.TotalPlacesAvailable - n.ApplicantsSignedUp.Count() : n.TotalPlacesAvailable));

                // ApplicantSlotPush(slot.ID, slot.TotalPlacesAvailable,
                //    slot.ApplicantsSignedUp.HasContent() ? slot.TotalPlacesAvailable - slot.ApplicantsSignedUp.Count() : slot.TotalPlacesAvailable);

                return new ServiceResult<bool>
                {
                    //  Data = new { success = true, message = "successfully signed up." }
                    Success = true,
                    Result = true,
                    Errors = null
                };
            }

            //var issues = NotificationService.FetchIssues();

            return new ServiceResult<bool>
            {

                Success = false,
                Result = false,
                Errors = new[] {"An error occurred. The signup could not be cancelled."}
            };
        }

        /*public object Get(NewCourse request)
        {
            return new ServiceResult<string>
            {
                Success = true,
                Result = string.Empty,
                Errors = new string[] { }
            };

        }*/
    }
}
