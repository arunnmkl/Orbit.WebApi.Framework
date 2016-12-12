using System;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using Orbit.WebApi.Core.Dependency;
using Orbit.WebApi.Core.Interfaces;
using Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Extensions.Authentication
{
    /// <summary>
    /// Class MVCAuthenticationAttribute.
    /// </summary>
    public class MVCAuthenticationAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Gets or sets the realm.
        /// </summary>
        /// <value>The realm.</value>
        public string Realm { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MVCAuthenticationAttribute"/> class.
        /// </summary>
        public MVCAuthenticationAttribute()
        {
        }

        /// <summary>
        /// Called when a process requests authorization.
        /// </summary>
        /// <param name="filterContext">The filter context, which encapsulates information for using <see cref="T:System.Web.Mvc.AuthorizeAttribute" />.</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            IAuthenticationCommand authenticationCommand = DependencyResolverContainer.Resolve<IAuthenticationCommand>();
            bool skipAuth = false;

            if (authenticationCommand != null)
            {
                if (authenticationCommand.SkipAuthorization(filterContext.ActionDescriptor))
                {
                    skipAuth = true;
                }

                IPrincipal principal = null;
                //string message = "Invalid username or password";

                foreach (var item in authenticationCommand.AuthenticationCommands)
                {
                    principal = item.Authenticate(filterContext.HttpContext.Request);

                    if (principal != null)
                    {
                        SetPrincipal(filterContext, principal);
                        if (item is IBasicAuthentication)
                        {
                            SetAuthCookie(filterContext, principal);
                        }

                        return;
                    }
                }

                if (!skipAuth)
                    Challenge(filterContext);
            }
        }

        private void SetAuthCookie(AuthorizationContext context, IPrincipal principal)
        {
            // TODO: Add logic here
            var cookie = new System.Web.HttpCookie(Configuration.Current.AuthCookieName, "Identity.SecureTicketString");

            cookie.Path = "/";
            cookie.HttpOnly = true;
            context.HttpContext.Response.SetCookie(cookie);
        }

        /// <summary>
        /// Sets the principal.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="principal">The principal.</param>
        private void SetPrincipal(AuthorizationContext context, IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (context != null)
            {
                context.RequestContext.HttpContext.User = principal;
            }
        }

        /// <summary>
        /// Challenges the specified context.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        private void Challenge(AuthorizationContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            response.AddHeader("WWW-Authenticate", String.Format("Basic realm=\"{0}\"", Realm ?? "WebClient"));
            response.End();
        }
    }
}