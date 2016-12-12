using System.Web;
using System.Web.Http.Filters;

namespace Orbit.WebApi.Core.Filters
{
    /// <summary>
    /// Action filter for no cookie response.
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute" />
    public class NoResponseCookieAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Occurs after the action method is invoked.
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (Security.Configuration.Current.CookieAuthenticationEnabled)
            {
                HttpContext.Current.Items.Add(string.Concat("remove-", Security.Configuration.Current.AuthCookieName), "true");
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
