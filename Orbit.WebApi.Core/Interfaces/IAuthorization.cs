using System.Net.Http;
using System.Web.Http.Controllers;

namespace Orbit.WebApi.Core.Interfaces
{
    /// <summary>
    /// Interface IAuthorization
    /// </summary>
    public interface IAuthorization
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
        /// Determines whether the specified action context is authorized.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="logicalResourceName">Name of the logical resource.</param>
        /// <param name="permission">The permission.</param>
        /// <param name="action">The action.</param>
        /// <returns><c>true</c> if the specified action context is authorized; otherwise, <c>false</c>.</returns>
        bool IsAuthorized(HttpActionContext actionContext, string logicalResourceName = null, string permission = null, string action = null);

        /// <summary>
        /// Gets the unauthorized response.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>HttpResponseMessage.</returns>
        HttpResponseMessage GetUnauthorizedResponse(HttpActionContext actionContext);
    }
}