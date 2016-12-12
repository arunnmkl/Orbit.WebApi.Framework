using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orbit.WebApi.Base.NetHttp;
using Orbit.WebApi.Extensions.Owin.Externals;

namespace Orbit.WebApi.Extensions.Validation
{
    /// <summary>
    /// This does the Google validation.
    /// </summary>
    /// <seealso cref="Orbit.WebApi.Extensions.Owin.Externals.IExternalValidation" />
    public class GoogleValidation : IExternalValidation
    {
        /// <summary>
        /// Gets the base URI.
        /// </summary>
        /// <value>
        /// The base URI.
        /// </value>
        public string BaseUri
        {
            get
            {
                return "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}";
            }
        }

        /// <summary>
        /// Verifies the external access token, if true then gives the valid user id and app id.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="appId">The application identifier.</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool VerifyExternalAccessToken(string accessToken, out string userId, out string appId)
        {
            string tokenBaseEndPoint = string.Format(BaseUri, accessToken);

            string result = Http.Get(tokenBaseEndPoint);

            return ValidateTokenResult(result, out userId, out appId);
        }

        /// <summary>
        /// Validates the token result, if true then gives the valid user id and app id.
        /// </summary>
        /// <param name="resultJson">The result in JSON, which comes from the external login providers.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="appId">The application identifier.</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool ValidateTokenResult(string resultJson, out string userId, out string appId)
        {
            dynamic jsonResult = (JObject)JsonConvert.DeserializeObject(resultJson);

            userId = jsonResult["user_id"];
            appId = jsonResult["audience"];

            if (!string.Equals(Startup.GoogleAuthOptions.ClientId, appId, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}