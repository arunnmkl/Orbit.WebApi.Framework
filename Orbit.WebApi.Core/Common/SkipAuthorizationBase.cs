using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Orbit.WebApi.Core.Common
{
    /// <summary>
    /// This is an abstract class with the basic implantation.
    /// </summary>
    public abstract class SkipAuthorizationBase
    {
        /// <summary>
        /// Skips the authorization for API applications, using the http action context.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if want to skip, <c>false</c> otherwise.</returns>
        public virtual bool SkipAuthorization(HttpActionContext actionContext)
        {
            var isAnonymous = actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>();

            if (isAnonymous.Any())
            {
                return true;
            }

            isAnonymous = actionContext.ActionDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>();

            if (isAnonymous.Any())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Skips the authorization for MVC applications, using the action descriptor.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if want to skip, <c>false</c> otherwise.</returns>
        public virtual bool SkipAuthorization(ActionDescriptor actionContext)
        {
            var isAnonymous = actionContext.ControllerDescriptor.GetCustomAttributes(typeof(System.Web.Mvc.AllowAnonymousAttribute), false);

            if (isAnonymous.Any())
            {
                return true;
            }

            isAnonymous = actionContext.GetCustomAttributes(typeof(System.Web.Mvc.AllowAnonymousAttribute), false);

            if (isAnonymous.Any())
            {
                return true;
            }

            return false;
        }
    }
}