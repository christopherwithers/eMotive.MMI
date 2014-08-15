using System.Collections.Generic;
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

    [Route("/Courses/New", "GET")]
    public class NewCourse
    {
    }

    [Authenticate] 
    public class CourseService : Service
    {
        public object Get(NewCourse request)
        {
            return new ServiceResult<string>
            {
                Success = true,
                Result = string.Empty,
                Errors = new string[] { }
            };

        }
    }
}
