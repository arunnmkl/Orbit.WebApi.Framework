using Microsoft.Owin.Security.DataHandler;
using OwinSecurity = Microsoft.Owin.Security;

namespace Orbit.WebApi.Core.Security
{
    /// <summary>
    /// Class to encapsulates the Access Token Format.
    /// </summary>
    /// <seealso cref="Microsoft.Owin.Security.ISecureDataFormat{Microsoft.Owin.Security.AuthenticationTicket}" />
    public class AccessTokenFormat : OwinSecurity.ISecureDataFormat<OwinSecurity.AuthenticationTicket>
    {
        /// <summary>
        /// Protects the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>authentication ticket as string</returns>
        public string Protect(OwinSecurity.AuthenticationTicket data)
        {
            var ticketFormat = new TicketDataFormat(new MachineKeyProtector());
            return ticketFormat.Protect(data);
        }

        /// <summary>
        /// Unprotects the specified protected text.
        /// </summary>
        /// <param name="protectedText">The protected text.</param>
        /// <returns>authentication ticket</returns>
        public OwinSecurity.AuthenticationTicket Unprotect(string protectedText)
        {
            var ticketFormat = new TicketDataFormat(new MachineKeyProtector());
            return ticketFormat.Unprotect(protectedText);
        }
    }
}
