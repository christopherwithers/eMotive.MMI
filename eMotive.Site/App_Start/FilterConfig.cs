using System.Web.Mvc;
using eMotive.MMI.Common.ActionFilters;

namespace eMotive.MMI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CriticalErrorAttribute());
          //  filters.Add(DependencyResolver.Current.GetService<CriticalErrorAttribute>());
         //   filters.Add(new CriticalErrorAttribute());
        }
    }
}
