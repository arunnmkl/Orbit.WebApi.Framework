using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Orbit.WebApi.Core.Common;
using Orbit.WebApi.Core.Interfaces;
using Orbit.WebApi.Core.Security;
using Orbit.WebApi.Security;

namespace Orbit.WebApi.Extensions.Authentication
{
    /// <summary>
    /// Class AuthorizationController.
    /// </summary>
    public class AuthorizationController : SkipAuthorizationBase, IAuthorization
    {
        /// <summary>
        /// Gets or sets the error message.
        /// This value should be get populated once any error has come so that User will get to know the exact error in the response
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Determines whether the specified action context is authorized.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="logicalResourceName">Name of the logical resource.</param>
        /// <param name="permission">The permission.</param>
        /// <param name="action">The action.</param>
        /// <returns><c>true</c> if the specified action context is authorized; otherwise, <c>false</c>.</returns>
        public bool IsAuthorized(HttpActionContext actionContext, string logicalResourceName = null, string permission = null, string action = null)
        {
            var principal = actionContext.RequestContext.Principal as IPrincipal;

            if (principal != null)
            {
                Thread.CurrentPrincipal = principal;

                if (string.IsNullOrEmpty(logicalResourceName) && string.IsNullOrEmpty(permission))
                {
                    return true;
                }

                else if (string.IsNullOrEmpty(logicalResourceName) && string.IsNullOrEmpty(permission) && string.IsNullOrEmpty(action))
                {
                    return true;
                }

                else if (!string.IsNullOrEmpty(logicalResourceName) && !string.IsNullOrEmpty(permission) && !string.IsNullOrEmpty(action))
                {
                    return IsAuthorized(logicalResourceName, action, permission);
                }

                else if (!string.IsNullOrEmpty(logicalResourceName) && !string.IsNullOrEmpty(permission))
                {
                    return IsAuthorized(logicalResourceName, permission);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the unauthorized response.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage GetUnauthorizedResponse(HttpActionContext actionContext)
        {
            return actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ErrorMessage);
        }

        /// <summary>
        /// Determines whether the specified action context is authorized.
        /// </summary>
        /// <param name="logicalResourceName">Name of the logical resource.</param>
        /// <param name="permission">The permission.</param>
        /// <returns><c>true</c> if the specified logical resource name is authorized; otherwise, <c>false</c>.</returns>
        public bool IsAuthorized(string logicalResourceName, string permission)
        {
            if (Configuration.Current.OrbitAuthorizationByPassed == false)
            {
                using (new AuthorizationScope(logicalResourceName, permission))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether the specified action context is authorized.
        /// </summary>
        /// <param name="logicalResourceName">Name of the logical resource.</param>
        /// <param name="permission">The permission.</param>
        /// <param name="action">The action.</param>
        /// <returns><c>true</c> if the specified logical resource name is authorized; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsAuthorized(string logicalResourceName, string permission, string action)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Authenticates the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Security.Principal.IPrincipal&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<IPrincipal> AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}