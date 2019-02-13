using System.Web;
using System.Web.Mvc;

namespace TheEverythingStore
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            /* Force all requests to use SSL 
             * DOESNT WORK LOCALLY
             */
            //filters.Add(new RequireHttpsAttribute());
        }
    }
}
