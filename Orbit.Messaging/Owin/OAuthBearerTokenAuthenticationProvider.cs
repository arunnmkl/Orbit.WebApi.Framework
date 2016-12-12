using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace Orbit.Messaging.Owin
{
    /// <summary>
    /// Class to encapsulate the oauth bearer token authentication.
    /// </summary>
    /// <seealso cref="OAuthBearerAuthenticationProvider" />
    internal class OAuthBearerTokenAuthenticationProvider : OAuthBearerAuthenticationProvider
    {
        /// <summary>
        /// Handles processing OAuth bearer token.
        /// </summary>
        /// <param name="context">oauth request token context.</param>
        /// <returns></returns>
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            string cookieToken = null;
            string queryStringToken = null;
            string headerToken = null;

            try
            {
                cookieToken = context.OwinContext.Request.Cookies[Security.Configuration.Current.AuthCookieName];
            }
            catch (NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("The cookie does not contain the bearer token");
            }

            try
            {
                queryStringToken = context.OwinContext.Request.Query[Security.Configuration.Current.AuthQueryStringName].ToString();
            }
            catch (NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("The query string does not contain the bearer token");
            }

            try
            {
                var auth = context.OwinContext.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(auth))
                {

                    AuthenticationHeaderValue authorization = AuthenticationHeaderValue.Parse(auth);
                    // check if there is any authorization token in header.
                    if (authorization != null)
                    {
                        // If there are authorization token, filter the recognized authentication scheme.
                        if (authorization.Scheme.ToUpperInvariant() == Security.Configuration.Current.AuthHeaderSchemaName.ToUpperInvariant())
                        {
                            headerToken = authorization.Parameter;
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("The connection header does not contain the bearer token");
            }

            // setting up the cookie token
            if (!string.IsNullOrEmpty(cookieToken))
            {
                context.Token = cookieToken;
            }
            // setting up the query string token
            else if (!string.IsNullOrEmpty(queryStringToken))
            {
                context.Token = queryStringToken;
            }
            // setting up the header authorization token
            else if (!string.IsNullOrEmpty(headerToken))
            {
                context.Token = headerToken;
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Handles validating the identity produced from an OAuth bearer token.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            return base.ValidateIdentity(context);
        }
    }
}