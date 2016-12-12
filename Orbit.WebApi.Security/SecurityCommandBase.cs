using System;

namespace Orbit.WebApi.Security
{
    /// <summary>
    /// Security command base.
    /// </summary>
    /// <seealso cref="Orbit.WebApi.Security.ISecurityCommand" />
    public abstract class SecurityCommandBase : ISecurityCommand
    {
        /// <summary>
        /// Decrypts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// decrypted text
        /// </returns>
        public virtual string Decrypt(string text)
        {
            return text;
        }

        /// <summary>
        /// Decrypts the string aes.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <returns>
        /// decrypted chiper text
        /// </returns>
        public virtual string DecryptStringAES(string cipherText)
        {
            return cipherText;
        }

        /// <summary>
        /// Decrypts the string from bytes.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns>
        /// decrypted string from bytes
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encrypts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// encrypted text
        /// </returns>
        public virtual string Encrypt(string text)
        {
            return text;
        }

        /// <summary>
        /// Gets the decrypted password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>
        /// decrypted password
        /// </returns>
        public virtual string GetDecryptedPassword(string password, string clientId)
        {
            return password;
        }

        /// <summary>
        /// Gets the decrypted username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>
        /// decrypted username
        /// </returns>
        public virtual string GetDecryptedUsername(string username, string clientId)
        {
            return username;
        }

        /// <summary>
        /// Validates the username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// validated result
        /// </returns>
        public virtual bool ValidateUsernameAndPassword(string username, string password)
        {
            return true;
        }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// Full name of the user.
        /// </returns>
        public virtual string GetUserFullName(long userId)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the user culture.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>the user culture</returns>
        public virtual string GetUserCulture(long userId)
        {
            return string.Empty;
        }
    }
}
