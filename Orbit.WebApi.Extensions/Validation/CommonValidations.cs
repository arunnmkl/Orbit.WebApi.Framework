using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Orbit.WebApi.Core;
using Orbit.WebApi.Extensions.Authentication;
using Orbit.WebApi.Extensions.Owin.Externals;
using Orbit.WebApi.Security.Models;

namespace Orbit.WebApi.Extensions.Validation
{
    /// <summary>
    /// Validates the common things like the token and the URI and all.
    /// </summary>
    public static class CommonValidations
    {
        /// <summary>
        /// Verifies the external access token.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns>parsed external access token details</returns>
        public static async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            string userId = string.Empty;
            string appId = string.Empty;

            IExternalValidation validator = ExternalValidationFactory.GetExternalValidation(provider);

            var parsedExternalAccessToken = await Task.Run(() =>
            {
                try
                {
                    if (validator.VerifyExternalAccessToken(accessToken, out userId, out appId) == true)
                    {
                        ParsedExternalAccessToken parsedToken = new ParsedExternalAccessToken();
                        parsedToken.app_id = appId;
                        parsedToken.user_id = userId;
                        return parsedToken;
                    }

                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            });

            return parsedExternalAccessToken;
        }

        /// <summary>
        /// Tries the parse redirect URI.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="redirectUriString">The redirect URI string.</param>
        /// <param name="queryStringName">Name of the query string.</param>
        /// <returns>true, if the redirect URI is valid, else false</returns>
        public static bool TryParseRedirectUri(HttpRequestMessage request, out string redirectUriString, string queryStringName = "redirect_uri")
        {
            Uri redirectUri;

            redirectUriString = request.GetQueryString(queryStringName);

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                redirectUriString = string.Empty;
                return false;
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                redirectUriString = string.Empty;
                return false;
            }

            var clientId = request.GetQueryString("client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                redirectUriString = "client_Id is required";
                return false;
            }

            var client = AuthenticationCommands.FindAuthClient(clientId);

            if (client == null)
            {
                redirectUriString = string.Format("Client_id '{0}' is not registered in the system.", clientId);
                return false;
            }

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                redirectUriString = string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
                return false;
            }

            redirectUriString = redirectUri.AbsoluteUri;

            return true;
        }

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// the query string value
        /// </returns>
        private static string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }
    }
}