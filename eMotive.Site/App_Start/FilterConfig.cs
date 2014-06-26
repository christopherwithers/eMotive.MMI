using System.Web.Mvc;

namespace eMotive.MMI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
          //  filters.Add(DependencyResolver.Current.GetService<CriticalErrorAttribute>());
         //   filters.Add(new CriticalErrorAttribute());
        }
    }
}
