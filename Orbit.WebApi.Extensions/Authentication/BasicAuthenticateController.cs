using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Orbit.WebApi.Core.Common;

namespace Orbit.WebApi.Extensions.Authentication
{
    /// <summary>
    /// Class BasicAuthenticateController with BasicAuthenticationBase.
    /// </summary>
    /// <seealso cref="Orbit.WebApi.Core.Common.BasicAuthenticationBase" />
    public class BasicAuthenticateController : BasicAuthenticationBase
    {
        /// <summary>
        /// Authenticates the token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="context"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// true whether the token is valid else return false.
        /// </returns>
        public override Task<bool> AuthenticateToken(string accessToken, HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Authenticates the username password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// IPrincipal.
        /// </returns>
        public override IPrincipal AuthenticateUsernamePassword(string userName, string password)
        {
            try
            {
                return AuthenticationCommands.Authenticate(userName, password);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }
    }
}