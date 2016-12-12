using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Orbit.WebApi.Core.Common;
using Orbit.WebApi.Core.Security;
using Orbit.WebApi.Extensions.Authentication;
using Orbit.WebApi.Security.Models;

namespace Orbit.WebApi.Extensions.Validation
{
    /// <summary>
    /// Encapsulate the object in token authentication validator.
    /// </summary>
    internal static class AuthenticationTokenValidator
    {
        /// <summary>
        /// Authenticates the token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// true whether the token is valid else return false.
        /// </returns>
        public static async Task<TokenAuthenticationResult> AuthenticateToken(string accessToken, HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // convert token into authentication ticket
            AuthenticationTicket authTicket = await AuthenticateAsync(accessToken, cancellationToken);
            if (authTicket == null)
            {
                return new TokenAuthenticationResult()
                {
                    ErrorResponse = AuthorizeResponseMessage.InvalidBearerToken,
                    IsValid = false
                };
            }

            // Validate expiration time if present
            DateTimeOffset currentUtc = Startup.OAuthBearerOptions.SystemClock.UtcNow;

            if (authTicket.Properties.ExpiresUtc.HasValue && authTicket.Properties.ExpiresUtc.Value < currentUtc)
            {
                return new TokenAuthenticationResult()
                {
                    ErrorResponse = AuthorizeResponseMessage.TokenExpired,
                    IsValid = false
                };
            }

            // Give application final opportunity to override results
            var authContext = new OAuthValidateIdentityContext(context.Request.GetOwinContext(), Startup.OAuthBearerOptions, authTicket);
            if (authTicket != null && authTicket.Identity != null && authTicket.Identity.IsAuthenticated)
            {
                // bearer token with identity starts validated
                authContext.Validated();
            }

            if (Startup.OAuthBearerOptions.Provider != null)
            {
                await Startup.OAuthBearerOptions.Provider.ValidateIdentity(authContext);
            }

            if (!authContext.IsValidated)
            {
                return new TokenAuthenticationResult()
                {
                    ErrorResponse = AuthorizeResponseMessage.InvalidBearerToken,
                    IsValid = false
                };
            }

            if (Configuration.Current.ValidatePasswordChange)
            {
                long userId = Convert.ToInt64(authTicket.Identity.FindFirst(ApiIdentity.UserIdClaimType).Value);
                long claimsTimeStamp = Convert.ToInt64(authTicket.Identity.FindFirst(ApiIdentity.PasswordTimestampClaimType).Value);
                long timeStamp = AuthenticationCommands.GetPasswordTimestamp(userId);

                if (claimsTimeStamp != timeStamp)
                {
                    return new TokenAuthenticationResult()
                    {
                        ErrorResponse = AuthorizeResponseMessage.AlteredCredential,
                        IsValid = false
                    };
                }
            }

            if (Configuration.Current.DBTokenValidationEnabled)
            {
                var userAuthTokenReq =
                    new UserAuthToken(accessToken)
                    {
                        UserId = Convert.ToInt64(authTicket.Identity.FindFirst(ApiIdentity.UserIdClaimType).Value)
                    };

                var userAuthTokenRes = AuthenticationCommands.GetUserAuthToken(userAuthTokenReq);

                if (userAuthTokenRes == null
                    || userAuthTokenRes.IsLoggedIn == false
                    || userAuthTokenRes.IsExpired == true)
                {
                    return new TokenAuthenticationResult()
                    {
                        ErrorResponse = AuthorizeResponseMessage.UserSessionExpired,
                        IsValid = false
                    };
                }
            }

            return new TokenAuthenticationResult()
            {
                IsValid = true
            };
        }
        /// <summary>
        /// Authenticates the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private static async Task<AuthenticationTicket> AuthenticateAsync(string token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, IClaimsIdenityFactory doesn't support CancellationTokens.
            return await Task.Run(() =>
            {
                return AuthenticationCommands.ConvertTokenAsAuthTicket(token);
            });
        }
    }

    /// <summary>
    /// Encapsulates the objects in token authentication result.
    /// </summary>
    internal class TokenAuthenticationResult
    {
        /// <summary>
        /// Gets or sets the error response.
        /// </summary>
        /// <value>
        /// The error response.
        /// </value>
        public ResponseError ErrorResponse { get; set; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid { get; set; }
    }
}
