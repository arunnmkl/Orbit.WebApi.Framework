using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Orbit.WebApi.Extensions.Common
{
    /// <summary>
    /// OAuth bearer authentication extensions
    /// </summary>
    public static class OAuthBearerAuthenticationExtensions
    {
        /// <summary>
        /// Uses the o authentication bearer authentication extended.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IAppBuilder UseOAuthBearerAuthenticationExtended(this IAppBuilder app, OAuthBearerAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.Use(typeof(OAuthBearerAuthenticationMiddlewareExtended), app, options);
            app.UseStageMarker(PipelineStage.Authenticate);
            return app;
        }
    }

    /// <summary>
    /// OAuth bearer authentication middleware extended
    /// </summary>
    /// <seealso cref="AuthenticationHandler{OAuthBearerAuthenticationOptions}" />
    internal class OAuthBearerAuthenticationHandlerExtended : AuthenticationHandler<OAuthBearerAuthenticationOptions>
    {
        /// <summary>
        /// The _challenge
        /// </summary>
        private readonly string _challenge;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthBearerAuthenticationHandlerExtended"/> class.
        /// </summary>
        /// <param name="challenge">The challenge.</param>
        public OAuthBearerAuthenticationHandlerExtended(string challenge)
        {
            _challenge = challenge;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthBearerAuthenticationHandlerExtended"/> class.
        /// </summary>
        protected OAuthBearerAuthenticationHandlerExtended()
        {
        }

        /// <summary>
        /// The core authentication logic which must be provided by the handler. Will be invoked at most
        /// once per request. Do not call directly, call the wrapping Authenticate method instead.
        /// </summary>
        /// <returns>
        /// The ticket data provided by the authentication logic
        /// </returns>
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            try
            {
                // Find token in default location
                string requestToken = null;
                string authorization = Request.Headers.Get("Authorization");
                if (!string.IsNullOrEmpty(authorization))
                {
                    if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        requestToken = authorization.Substring("Bearer ".Length).Trim();
                    }
                }

                // Give application opportunity to find from a different location, adjust, or reject token
                var requestTokenContext = new OAuthRequestTokenContext(Context, requestToken);
                await Options.Provider.RequestToken(requestTokenContext);

                // If no token found, no further work possible
                if (string.IsNullOrEmpty(requestTokenContext.Token))
                {
                    return null;
                }

                // Call provider to process the token into data
                var tokenReceiveContext = new AuthenticationTokenReceiveContext(
                    Context,
                    Options.AccessTokenFormat,
                    requestTokenContext.Token);

                await Options.AccessTokenProvider.ReceiveAsync(tokenReceiveContext);
                if (tokenReceiveContext.Ticket == null)
                {
                    tokenReceiveContext.DeserializeTicket(tokenReceiveContext.Token);
                }

                AuthenticationTicket ticket = tokenReceiveContext.Ticket;
                if (ticket == null)
                {
                    Context.Set("oauth.token_invalid", true);
                    return null;
                }

                // Validate expiration time if present
                DateTimeOffset currentUtc = Options.SystemClock.UtcNow;

                if (ticket.Properties.ExpiresUtc.HasValue &&
                    ticket.Properties.ExpiresUtc.Value < currentUtc)
                {
                    Context.Set("oauth.token_expired", true);
                    return null;
                }

                // Give application final opportunity to override results
                var context = new OAuthValidateIdentityContext(Context, Options, ticket);
                if (ticket != null &&
                    ticket.Identity != null &&
                    ticket.Identity.IsAuthenticated)
                {
                    // bearer token with identity starts validated
                    context.Validated();
                }

                if (Options.Provider != null)
                {
                    await Options.Provider.ValidateIdentity(context);
                }

                if (!context.IsValidated)
                {
                    return null;
                }

                // resulting identity values go back to caller
                return context.Ticket;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Override this method to deal with 401 challenge concerns, if an authentication scheme in question
        /// deals an authentication interaction as part of it's request flow. (like adding a response header, or
        /// changing the 401 result to 302 of a login page or external sign-in location.)
        /// </summary>
        /// <returns></returns>
        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return Task.FromResult<object>(null);
            }

            AuthenticationResponseChallenge challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if (challenge != null)
            {
                OAuthChallengeContext challengeContext = new OAuthChallengeContext(Context, _challenge);
                Options.Provider.ApplyChallenge(challengeContext);
            }

            return Task.FromResult<object>(null);
        }
    }


    /// <summary>
    /// OAuth bearer authentication middleware extended
    /// </summary>
    /// <seealso cref="AuthenticationMiddleware{OAuthBearerAuthenticationOptions}" />
    public class OAuthBearerAuthenticationMiddlewareExtended : AuthenticationMiddleware<OAuthBearerAuthenticationOptions>
    {
        /// <summary>
        /// The _challenge
        /// </summary>
        private readonly string _challenge;

        /// <summary>
        /// Bearer authentication component which is added to an OWIN pipeline. This constructor is not
        /// called by application code directly, instead it is added by calling the the IAppBuilder UseOAuthBearerAuthentication
        /// extension method.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="app">The application.</param>
        /// <param name="options">The options.</param>
        public OAuthBearerAuthenticationMiddlewareExtended(OwinMiddleware next, IAppBuilder app, OAuthBearerAuthenticationOptions options)
          : base(next, options)
        {
            _challenge = string.IsNullOrWhiteSpace(Options.Challenge) ? (!string.IsNullOrWhiteSpace(Options.Realm) ? "Bearer realm=\"" + this.Options.Realm + "\"" : "Bearer") : this.Options.Challenge;

            if (Options.Provider == null)
            {
                Options.Provider = new OAuthBearerAuthenticationProvider();
            }

            if (Options.AccessTokenFormat == null)
            {
                Options.AccessTokenFormat = new TicketDataFormat(Microsoft.Owin.Security.DataProtection.AppBuilderExtensions.CreateDataProtector(app, typeof(OAuthBearerAuthenticationMiddleware).Namespace, "Access_Token", "v1"));
            }

            if (Options.AccessTokenProvider != null)
            {
                return;
            }

            Options.AccessTokenProvider = new AuthenticationTokenProvider();
        }

        /// <summary>
        /// Called by the AuthenticationMiddleware base class to create a per-request handler.
        /// </summary>
        /// <returns>
        /// A new instance of the request handler
        /// </returns>
        protected override AuthenticationHandler<OAuthBearerAuthenticationOptions> CreateHandler()
        {
            return new OAuthBearerAuthenticationHandlerExtended(_challenge);
        }
    }

    /// <summary>
    /// Authentication failure message
    /// </summary>
    /// <seealso cref="System.Net.Http.HttpResponseMessage" />
    public class AuthenticationFailureMessage : HttpResponseMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFailureMessage"/> class.
        /// </summary>
        /// <param name="reasonPhrase">The reason phrase.</param>
        /// <param name="request">The request.</param>
        /// <param name="responseMessage">The response message.</param>
        public AuthenticationFailureMessage(string reasonPhrase, HttpRequestMessage request, object responseMessage)
            : base(HttpStatusCode.Unauthorized)
        {
            MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();

            Content = new ObjectContent<object>(responseMessage, jsonFormatter);
            RequestMessage = request;
            ReasonPhrase = reasonPhrase;
        }
    }


    /// <summary>
    /// Authorize attribute
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.AuthorizationFilterAttribute" />
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// Calls when a process requests authorization.
        /// </summary>
        /// <param name="actionContext">The action context, which encapsulates information for using <see cref="T:System.Web.Http.Filters.AuthorizationFilterAttribute" />.</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);

            ////check authentication and return if not authorized
            if (actionContext != null)
            {
                //if (!WebSecurity.IsAuthenticated)
                //{
                //    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = actionContext.ControllerContext.Request };
                //    return;
                //}

            }
        }
    }
}
