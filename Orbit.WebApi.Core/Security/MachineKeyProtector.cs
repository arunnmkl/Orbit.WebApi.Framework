using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;

namespace Orbit.WebApi.Core.Security
{
    /// <summary>
    /// Machine key protector.
    /// </summary>
    /// <seealso cref="Microsoft.Owin.Security.DataProtection.IDataProtector" />
    public class MachineKeyProtector : IDataProtector
    {
        /// <summary>
        /// The purposes
        /// </summary>
        private readonly string[] purposes =
        {
            typeof(OAuthAuthorizationServerMiddleware).Namespace,
            "Access_Token",
            "v1"
        };

        /// <summary>
        /// Called to protect user data.
        /// </summary>
        /// <param name="userData">The original data that must be protected</param>
        /// <returns>
        /// A different byte array that may be unprotected or altered only by software that has access to
        /// the an identical IDataProtection service.
        /// </returns>
        public byte[] Protect(byte[] userData)
        {
            return System.Web.Security.MachineKey.Protect(userData, purposes);
        }

        /// <summary>
        /// Called to unprotect user data
        /// </summary>
        /// <param name="protectedData">The byte array returned by a call to Protect on an identical IDataProtection service.</param>
        /// <returns>
        /// The byte array identical to the original userData passed to Protect.
        /// </returns>
        public byte[] Unprotect(byte[] protectedData)
        {
            return System.Web.Security.MachineKey.Unprotect(protectedData, purposes);
        }
    }
}
