namespace Orbit.WebApi.Extensions.Owin.Externals
{
    /// <summary>
    /// This interface make sure that all the external validation classes are using the same mechanism.
    /// </summary>
    public interface IExternalValidation
    {
        /// <summary>
        /// Gets the base URI.
        /// </summary>
        /// <value>
        /// The base URI.
        /// </value>
        string BaseUri { get; }

        /// <summary>
        /// Verifies the external access token, if true then gives the valid user id and app id.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="appId">The application identifier.</param>
        /// <returns></returns>
        bool VerifyExternalAccessToken(string accessToken, out string userId, out string appId);

        /// <summary>
        /// Validates the token result.
        /// </summary>
        /// <param name="resultJson">The result JSON.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="appId">The application identifier.</param>
        /// <returns></returns>
        bool ValidateTokenResult(string resultJson, out string userId, out string appId);
    }
}
