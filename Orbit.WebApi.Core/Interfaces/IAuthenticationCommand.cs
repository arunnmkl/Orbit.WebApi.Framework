using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace Orbit.WebApi.Core.Interfaces
{
    /// <summary>
    /// Interface IAuthenticationCommand
    /// </summary>
    public interface IAuthenticationCommand
    {
        /// <summary>
        /// Skips the authorization.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SkipAuthorization(HttpActionContext actionContext);

        /// <summary>
        /// Skips the authorization.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SkipAuthorization(ActionDescriptor actionContext);

        /// <summary>
        /// Gets or sets the authentication commands.
        /// </summary>
        /// <value>The authentication commands.</value>
        HashSet<IAuthentication> AuthenticationCommands { get; set; }

        /// <summary>
        /// Adds the new command.
        /// </summary>
        /// <param name="authentication">The authentication.</param>
        void AddNewCommand(IAuthentication authentication);

        /// <summary>
        /// Validates the CSRF attack.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ValidateCSRFAttack(HttpAuthenticationContext context);
    }
}