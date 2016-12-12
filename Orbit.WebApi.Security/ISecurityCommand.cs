using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.WebApi.Security
{
    /// <summary>
    /// Interface security command.
    /// </summary>
    public interface ISecurityCommand
    {
        /// <summary>
        /// Validates the username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>validated result</returns>
        bool ValidateUsernameAndPassword(string username, string password);

        /// <summary>
        /// Gets the decrypted username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>decrypted username</returns>
        string GetDecryptedUsername(string username, string clientId);

        /// <summary>
        /// Gets the decrypted password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>decrypted password</returns>
        string GetDecryptedPassword(string password, string clientId);

        /// <summary>
        /// Decrypts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>decrypted text</returns>
        string Decrypt(string text);

        /// <summary>
        /// Encrypts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>encrypted text</returns>
        string Encrypt(string text);

        /// <summary>
        /// Decrypts the string aes.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <returns>decrypted chiper text</returns>
        string DecryptStringAES(string cipherText);

        /// <summary>
        /// Decrypts the string from bytes.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <returns>decrypted string from bytes</returns>
        string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv);

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// Full name of the user.
        /// </returns>
        string GetUserFullName(long userId);

        /// <summary>
        /// Gets the user culture.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>the user culture</returns>
        string GetUserCulture(long userId);
    }
}
