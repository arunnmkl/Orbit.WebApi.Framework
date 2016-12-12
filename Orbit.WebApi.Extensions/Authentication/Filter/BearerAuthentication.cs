using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Orbit.WebApi.Core.Common;
using Orbit.WebApi.Core.Dependency;
using Orbit.WebApi.Core.Interfaces;
using Orbit.WebApi.Core.Results;
using Orbit.WebApi.Core.Security;
using Orbit.WebApi.Security.Models;

namespace Orbit.WebApi.Extensions.Authentication.Filter
{
    /// <summary>
    /// Bearer authentication filter
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute" />
    /// <seealso cref="System.Web.Http.Filters.IAuthenticationFilter" />
    public class BearerAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        /// Authenticates the request.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A Task that will perform authentication.
        /// </returns>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // skip allow anonymous calls
            IAuthorization Authentication = DependencyResolverContainer.Resolve<IAuthorization>();
            if (Authentication != null)
            {
                if (Authentication.SkipAuthorization(context.ActionContext))
                {
                    return;
                }
            }

            // 1. Look for token in the request.
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;

            // 2. If there are no authorization token in header, do nothing.
            if (authorization == null)
            {
                return;
            }

            // 3. If there are authorization token but the filter does not recognize the 
            //    authentication scheme, do nothing.
            if (authorization.Scheme != "Bearer")
            {
                context.ErrorResult = new AuthenticationFailureResult(context.Request, AuthorizeResponseMessage.RequireAuthorization);
                return;
            }

            // 4. If there are authorization token that the filter understands, try to validate them.
            // 5. If the authorization token are empty/bad, set the error result.
            if (String.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult(request, AuthorizeResponseMessage.MissingAccessToken);
                return;
            }

            // convert token into authentication ticket
            AuthenticationTicket authTicket = await AuthenticateAsync(authorization.Parameter, cancellationToken);
            if (authTicket == null)
            {
                context.ErrorResult = new AuthenticationFailureResult(request, AuthorizeResponseMessage.InvalidBearerToken);
                return;
            }

            // Validate expiration time if present
            DateTimeOffset currentUtc = Startup.OAuthBearerOptions.SystemClock.UtcNow;

            if (authTicket.Properties.ExpiresUtc.HasValue && authTicket.Properties.ExpiresUtc.Value < currentUtc)
            {
                context.ErrorResult = new AuthenticationFailureResult(request, AuthorizeResponseMessage.TokenExpired);
                return;
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
                context.ErrorResult = new AuthenticationFailureResult(request, AuthorizeResponseMessage.InvalidBearerToken);
                return;
            }

            if (Configuration.Current.DBTokenValidationEnabled)
            {
                var userAuthTokenReq = new UserAuthToken(authorization.Parameter)
                {
                    UserId = Convert.ToInt64(authTicket.Identity.FindFirst(ApiIdentity.UserIdClaimType).Value)
                };

                var userAuthTokenRes = AuthenticationCommands.GetUserAuthToken(userAuthTokenReq);

                if (userAuthTokenRes == null
                    || userAuthTokenRes.IsLoggedIn == false
                    || userAuthTokenRes.IsExpired == true)
                {
                    context.ErrorResult = new AuthenticationFailureResult(request, AuthorizeResponseMessage.UserSessionExpired);
                    return;
                }
            }

            // 6. If the token is valid, set principal.
            var claimsPrincipal = context.Principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult(request, AuthorizeResponseMessage.NoPrincipal);
                return;
            }
            else if (!(claimsPrincipal is ApiPrincipal))
            {
                context.Principal = new ApiPrincipal(claimsPrincipal);
            }
        }

        /// <summary>
        /// Authenticates the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task<AuthenticationTicket> AuthenticateAsync(string token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, IClaimsIdenityFactory doesn't support CancellationTokens.
            return await Task.Run(() =>
            {
                return AuthenticationCommands.ConvertTokenAsAuthTicket(token);
            });
        }

        /// <summary>
        /// Challenges the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Bearer");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }
    }
}
