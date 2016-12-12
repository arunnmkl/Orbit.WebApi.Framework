using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Orbit.WebApi.Core;
using Orbit.WebApi.Core.Common;
using Orbit.WebApi.Core.Dependency;
using Orbit.WebApi.Core.Interfaces;
using ApiSecurity = Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Extensions.Authentication
{
    /// <summary>
    /// Class AuthenticationCommand, which contains all the authentication type classes
    /// </summary>
    public class AuthenticationCommand : SkipAuthorizationBase, IAuthenticationCommand
    {
        /// <summary>
        /// Gets or sets the authentication commands.
        /// </summary>
        /// <value>The authentication commands.</value>
        public HashSet<IAuthentication> AuthenticationCommands { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationCommand" /> class.
        /// </summary>
        public AuthenticationCommand()
        {
            AuthenticationCommands = new HashSet<IAuthentication>();

            // Add all the authentication logic in here
            if (ApiSecurity.Configuration.Current.CookieAuthenticationEnabled)
            {
                AuthenticationCommands.Add(new CookieAuthenticationController());
            }

            if (ApiSecurity.Configuration.Current.BasicAuthenticationEnabled)
            {
                AuthenticationCommands.Add(new BasicAuthenticateController());
            }
        }

        /// <summary>
        /// Adds the new command.
        /// </summary>
        /// <param name="authentication">The authentication.</param>
        public void AddNewCommand(IAuthentication authentication)
        {
            AuthenticationCommands.Add(authentication);
        }

        /// <summary>
        /// Skips the authorization, for OAuth validation.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns></returns>
        public override bool SkipAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.ControllerContext.Request;
            var authenticationHeader = request.Headers.Authorization;

            if (authenticationHeader != null
                && authenticationHeader.Scheme == "Bearer"
                && !string.IsNullOrEmpty(authenticationHeader.Parameter))
            {
                return true;
            }

            return SkipAuthorizationBaseClassMethod(actionContext);
        }

        /// <summary>
        /// Skips the authorization base class method.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns></returns>
        public bool SkipAuthorizationBaseClassMethod(HttpActionContext actionContext)
        {
            return base.SkipAuthorization(actionContext);
        }

        /// <summary>
        /// Validates the CSRF attack.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if valid token, <c>false</c> otherwise.</returns>
        public bool ValidateCSRFAttack(HttpAuthenticationContext context)
        {
            bool isCsrf = true;
            ICSRFValidation validator = DependencyResolverContainer.Resolve<ICSRFValidation>();
            if (validator != null)
            {
                var request = context.Request;
                string csrfCookie = request.GetCookie(ApiSecurity.Configuration.Current.CSRFCookieName);
                string csrfHeaderValue = request.GetHeader(ApiSecurity.Configuration.Current.CSRFHeaderName);

                isCsrf = string.IsNullOrEmpty(csrfCookie) || string.IsNullOrEmpty(csrfHeaderValue) ? true : validator.Validate(csrfCookie, csrfHeaderValue);
            }

            return isCsrf;
        }
    }
}