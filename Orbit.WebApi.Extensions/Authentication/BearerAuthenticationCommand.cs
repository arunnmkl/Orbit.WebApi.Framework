using System.Collections.Generic;
using System.Web.Http.Controllers;
using Orbit.WebApi.Core.Interfaces;
using ApiSecurity = Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Extensions.Authentication
{
    /// <summary>
    /// Class BearerAuthenticationCommand, which contains all the authentication type classes
    /// </summary>
    public class BearerAuthenticationCommand : AuthenticationCommand, IBearerAuthenticationCommand
    {
        /// <summary>
        /// Gets or sets the authentication commands.
        /// </summary>
        /// <value>
        /// The authentication commands.
        /// </value>
        public new HashSet<IBearerAuthentication> AuthenticationCommands { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationCommand" /> class.
        /// </summary>
        public BearerAuthenticationCommand() : base()
        {
            AuthenticationCommands = new HashSet<IBearerAuthentication>();

            // Add all the authentication logic in here
            if (ApiSecurity.Configuration.Current.OAuthAuthenticationEnabled)
            {
                AuthenticationCommands.Add(new BearerAuthenticateController());
            }
        }

        /// <summary>
        /// Adds the new command.
        /// </summary>
        /// <param name="authentication">The authentication.</param>
        public void AddNewCommand(IBearerAuthentication authentication)
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
            return SkipAuthorizationBaseClassMethod(actionContext);
        }
    }
}