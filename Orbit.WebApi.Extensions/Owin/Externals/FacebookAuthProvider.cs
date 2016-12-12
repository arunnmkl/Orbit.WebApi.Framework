using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Facebook;

namespace Orbit.WebApi.Extensions.Owin.Externals
{
    /// <summary>
    /// This class is responsible for the Facebook authentication and adds the token to a claim.
    /// </summary>
    /// <seealso cref="Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider" />
    public class FacebookAuthProvider : FacebookAuthenticationProvider
    {
        /// <summary>
        /// Invoked whenever Facebook successfully authenticates a user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the user <see cref="T:System.Security.Claims.ClaimsIdentity" />.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> representing the completed operation.
        /// </returns>
        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            return Task.FromResult<object>(null);
        }
    }
}