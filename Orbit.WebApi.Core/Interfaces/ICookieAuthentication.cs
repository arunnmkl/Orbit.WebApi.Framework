using System.Security.Principal;

namespace Orbit.WebApi.Core.Interfaces
{
    /// <summary>
    /// Interface ICookieAuthentication
    /// </summary>
    public interface ICookieAuthentication : IAuthentication
    {
        /// <summary>
        /// Authenticates the cookie.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <returns>IPrincipal.</returns>
        IPrincipal AuthenticateCookie(string ticket);
    }
}
