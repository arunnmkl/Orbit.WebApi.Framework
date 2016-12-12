using System.Net.Http;
using Orbit.WebApi.Extensions.Authentication;
using Orbit.WebApi.Security.Models;

namespace Orbit.WebApi.Extensions.Owin.Externals
{
    /// <summary>
    /// This class is responsible for the all kind of the external communication, and data provider.
    /// </summary>
    public static class ExternalProvider
    {
        /// <summary>
        /// Signs the out.
        /// </summary>
        /// <param name="Request">The request.</param>
        /// <param name="authenticationType">Type of the authentication.</param>
        public static void SignOut(HttpRequestMessage Request, string authenticationType)
        {
            Request.GetOwinContext().Authentication.SignOut(authenticationType);
            if (Core.Security.Configuration.Current.CookieAuthenticationEnabled)
            {
                Request.GetOwinContext().Authentication.SignOut(Core.Security.Configuration.Current.AuthCookieName);
            }
        }

        /// <summary>
        /// Determines whether [is user exists] [the specified login provider].
        /// </summary>
        /// <param name="loginProvider">The login provider.</param>
        /// <param name="providerKey">The provider key.</param>
        /// <returns>
        /// True if the user exits.
        /// </returns>
        public static bool IsUserExists(string loginProvider, string providerKey)
        {
            var user = AuthenticationCommands.FindLoginProvider(loginProvider, providerKey);
            return user != null;
        }

        /// <summary>
        /// Gets the complete redirect URI.
        /// </summary>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="externalLoginData">The external login data.</param>
        /// <returns>
        /// The data which is returned from the external login.
        /// </returns>
        public static string GetCompleteRedirectUri(string redirectUri, ExternalData externalLoginData)
        {
            bool isUserExists = IsUserExists(externalLoginData.LoginProvider, externalLoginData.ProviderKey);

            return string.Format("{0}#external_access_token={1}&provider={2}&registered_user={3}&external_user_name={4}&local_Bare_token={5}",
                                            redirectUri,
                                            externalLoginData.ExternalAccessToken,
                                            externalLoginData.LoginProvider,
                                            isUserExists.ToString(),
                                            externalLoginData.UserName,
                                            externalLoginData.LocalBearerToken);
        }
    }
}