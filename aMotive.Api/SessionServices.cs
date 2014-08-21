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

    [Authenticate] 
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
