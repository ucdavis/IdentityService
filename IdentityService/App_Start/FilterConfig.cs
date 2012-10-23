using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using IdentityService.App_Start;

namespace IdentityService
{
    public class FilterConfig
    {
        // HTTP Filters
        public static void RegisterGlobalFilters(HttpFilterCollection filters)
        {
            filters.Add(new MyAuthenticationFiter());
        }

        //MVC Filters
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}