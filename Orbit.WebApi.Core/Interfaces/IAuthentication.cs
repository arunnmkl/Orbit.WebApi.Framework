using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Orbit.WebApi.Core.Interfaces
{
    /// <summary>
    /// Interface IAuthentication
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Skips the authorization.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SkipAuthorization(HttpActionContext actionContext);

        /// <summary>
        /// Authenticates the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IPrincipal&gt;.</returns>
        Task<IPrincipal> AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Authenticates the specified HTTP request base.
        /// </summary>
        /// <param name="httpRequestBase">The HTTP request base.</param>
        /// <returns>IPrincipal.</returns>
        IPrincipal Authenticate(HttpRequestBase httpRequestBase);

        /// <summary>
        /// Authenticates the token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// true whether the token is valid else return false.
        /// </returns>
        Task<bool> AuthenticateToken(string accessToken, HttpAuthenticationContext context, CancellationToken cancellationToken);
    }
}