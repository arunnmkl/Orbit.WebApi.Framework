using System;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Orbit.WebApi.Core.Exceptions;

namespace Orbit.WebApi.Extensions.Owin.Externals
{
    /// <summary>
    /// This is a temp class which should be removed after the real implementation.
    /// </summary>
    public class ExternalData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalData" /> class.
        /// </summary>
        public ExternalData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalData" /> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <exception cref="WebApiException">
        /// Invalid access.
        /// or
        /// Invalid provider key/Issuer.
        /// or
        /// Issuer is default.
        /// </exception>
        public ExternalData(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                throw new WebApiException("Invalid access.");
            }

            Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
            {
                throw new WebApiException("Invalid provider key/Issuer.");
            }

            if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
            {
                throw new WebApiException("Issuer is default.");
            }

            LoginProvider = providerKeyClaim.Issuer;
            ProviderKey = providerKeyClaim.Value;
            UserName = identity.FindFirstValue(ClaimTypes.Name);
            ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken");
        }

        /// <summary>
        /// Gets or sets the login provider.
        /// </summary>
        /// <value>
        /// The login provider.
        /// </value>
        public string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        /// <value>
        /// The provider key.
        /// </value>
        public string ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the external access token.
        /// </summary>
        /// <value>
        /// The external access token.
        /// </value>
        public string ExternalAccessToken { get; set; }

        /// <summary>
        /// Gets or sets the local bearer token.
        /// </summary>
        /// <value>
        /// The local bearer token.
        /// </value>
        public string LocalBearerToken { get; set; }
    }
}