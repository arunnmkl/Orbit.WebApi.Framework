using System.Net.Http;

namespace Orbit.WebApi.Core.Interfaces
{
    /// <summary>
    /// Interface IApiKeyValidator
    /// </summary>
    public interface IApiKeyValidator
    {
        /// <summary>
        /// Validates the key.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ValidateKey(HttpRequestMessage message);

        /// <summary>
        /// Validates the key.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ValidateKey(string apiKey);
    }
}
