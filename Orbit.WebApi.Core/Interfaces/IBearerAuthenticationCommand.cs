using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace Orbit.WebApi.Core.Interfaces
{
    /// <summary>
    /// Interface IBearerAuthenticationCommand
    /// </summary>
    public interface IBearerAuthenticationCommand : IAuthenticationCommand
    {
        /// <summary>
        /// Gets or sets the authentication commands.
        /// </summary>
        /// <value>
        /// The authentication commands.
        /// </value>
        new HashSet<IBearerAuthentication> AuthenticationCommands { get; set; }

        /// <summary>
        /// Adds the new command.
        /// </summary>
        /// <param name="authentication">The authentication.</param>
        void AddNewCommand(IBearerAuthentication authentication);
    }
}