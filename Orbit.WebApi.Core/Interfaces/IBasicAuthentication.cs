using System.Security.Principal;

namespace Orbit.WebApi.Core.Interfaces
{
    /// <summary>
    /// Interface IBasicAuthentication
    /// </summary>
    public interface IBasicAuthentication : IAuthentication
    {
        /// <summary>
        /// Authenticates the username password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>IPrincipal.</returns>
        IPrincipal AuthenticateUsernamePassword(string userName, string password);
    }
}
