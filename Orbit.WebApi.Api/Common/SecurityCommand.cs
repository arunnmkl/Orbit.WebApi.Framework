using ApiSecurity = Orbit.WebApi.Security;

namespace Orbit.WebApi.Api.Common
{
    public class SecurityCommand : ApiSecurity.SecurityCommandBase
    {
        /// <summary>
        /// Encrypts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// encrypted text
        /// </returns>
        public override string Encrypt(string text)
        {
            text = SecureString.Encrypt(text);
            return base.Encrypt(text);
        }

        /// <summary>
        /// Gets the decrypted password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>
        /// decrypted password
        /// </returns>
        public override string GetDecryptedPassword(string password, string clientId)
        {
            return base.GetDecryptedPassword(password, clientId);
        }

        /// <summary>
        /// Gets the decrypted username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>
        /// decrypted username
        /// </returns>
        public override string GetDecryptedUsername(string username, string clientId)
        {
            return base.GetDecryptedUsername(username, clientId);
        }

        /// <summary>
        /// Validates the username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// validated result
        /// </returns>
        public override bool ValidateUsernameAndPassword(string username, string password)
        {
            return base.ValidateUsernameAndPassword(username, password);
        }
    }
}