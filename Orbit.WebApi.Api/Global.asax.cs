using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Orbit.WebApi.Api
{
    /// <summary>
    /// Global objects.
    /// </summary>
    /// <seealso cref="System.Web.HttpApplication" />
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// Applications the start.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(Extensions.Startup.Config);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// Applications the end request.
        /// </summary>
        protected void Application_EndRequest()
        {
            if (Core.Security.Configuration.Current.CookieAuthenticationEnabled)
            {
                var noResponseItem = HttpContext.Current.Items[string.Concat("remove-", Core.Security.Configuration.Current.AuthCookieName)];
                if (Convert.ToBoolean(noResponseItem))
                {
                    var rCookie = Context.Request.Cookies[Core.Security.Configuration.Current.AuthCookieName];
                    if (rCookie != null)
                    {
                        rCookie.Expires = DateTime.Now.AddDays(-1);
                    }

                    Context.Response.Cookies.Remove(Core.Security.Configuration.Current.AuthCookieName);
                }
            }
        }
    }
}
