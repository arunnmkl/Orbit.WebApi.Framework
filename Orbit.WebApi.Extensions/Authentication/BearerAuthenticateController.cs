using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Orbit.WebApi.Core.Common;
using Orbit.WebApi.Extensions.Validation;

namespace Orbit.WebApi.Extensions.Authentication
{
    /// <summary>
    /// Bearer authenticate controller
    /// </summary>
    /// <seealso cref="BearerAuthenticationBase" />
    public class BearerAuthenticateController : BearerAuthenticationBase
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
        public override async Task<bool> AuthenticateToken(string accessToken, HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var authResult = await AuthenticationTokenValidator.AuthenticateToken(accessToken, context, cancellationToken);
            if (authResult.ErrorResponse != null)
            {
                ErrorMessage = authResult.ErrorResponse;
            }

            return authResult.IsValid;
        }
    }
}
