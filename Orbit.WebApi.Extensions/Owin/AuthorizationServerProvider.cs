using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Orbit.WebApi.Extensions.Authentication;
using Orbit.WebApi.Extensions.Common;
using Orbit.WebApi.Security.Models;
using CoreSecurity = Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Extensions.Owin
{
    /// <summary>
    /// This is the authentication provider which is responsible for all kind of validation for the client 
    /// and the user name and password as well, when /token means main token service will get called.
    /// </summary>
    /// <seealso cref="Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerProvider" />
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// Called to validate that the origin of the request is a registered "client_id", and that the correct credentials for that client are
        /// present on the request. If the web application accepts Basic authentication credentials,
        /// context.TryGetBasicCredentials(out clientId, out clientSecret) may be called to acquire those values if present in the request header. If the web
        /// application accepts "client_id" and "client_secret" as form encoded POST parameters,
        /// context.TryGetFormCredentials(out clientId, out clientSecret) may be called to acquire those values if present in the request body.
        /// If context.Validated is not called the request will not proceed further.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            AuthClient client = null;

            var forceLogin = context.TryGetParamValues("forcelogin");
            if (forceLogin != null)
            {
                context.OwinContext.Set("as:ForceLogin", Convert.ToBoolean(forceLogin.FirstOrDefault()));
            }
            else
            {
                context.OwinContext.Set("as:ForceLogin", Convert.ToBoolean("false"));
            }

            var rememberMe = context.TryGetParamValues("rememberMe");
            if (rememberMe != null)
            {
                context.OwinContext.Set("as:RememberMe", Convert.ToBoolean(rememberMe.FirstOrDefault()));
            }
            else
            {
                context.OwinContext.Set("as:RememberMe", Convert.ToBoolean("false"));
            }

            var impersonate = context.TryGetParamValues("impersonate");
            if (impersonate != null)
            {
                context.OwinContext.Set("as:Impersonate", Convert.ToBoolean(impersonate.FirstOrDefault()));
            }
            else
            {
                context.OwinContext.Set("as:Impersonate", Convert.ToBoolean("false"));
            }

            var impersonatingId = context.TryGetParamValues("impersonatingUserId");
            var random = context.TryGetParamValues("random");
            if (random != null && impersonatingId != null)
            {
                var impersonatingUserId = Convert.ToInt64(CoreSecurity.HashEncryptor.Decrypt(impersonatingId.FirstOrDefault()));
                var randomNumber = Convert.ToInt32(CoreSecurity.HashEncryptor.Decrypt(random.FirstOrDefault()));
                context.OwinContext.Set("as:ImpersonatingUserId", Convert.ToInt64(impersonatingUserId / randomNumber));
            }
            else
            {
                context.OwinContext.Set("as:ImpersonatingUserId", default(long?));
            }

            var revokeImpersonate = context.TryGetParamValues("revoke_impesonation");
            if (revokeImpersonate != null)
            {
                context.OwinContext.Set("as:Revoke_Impersonate", Convert.ToBoolean(revokeImpersonate.FirstOrDefault()));
            }
            else
            {
                context.OwinContext.Set("as:Revoke_Impersonate", Convert.ToBoolean("false"));
            }

            var userId = context.TryGetParamValues("user_id");
            if (userId != null)
            {
                context.OwinContext.Set("as:UserId", Convert.ToInt64(userId.FirstOrDefault()));
            }

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //context.Validated();
                SetValidateClientError(context, "invalid_clientId", "Client id should be sent.");
                return Task.FromResult<object>(null);
            }

            client = AuthenticationCommands.FindAuthClient(context.ClientId);

            if (client == null)
            {
                SetValidateClientError(context, "invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            if (client.ApplicationType == ApplicationTypes.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    SetValidateClientError(context, "invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    if (client.Secret != Security.Helper.GetHash(clientSecret))
                    {
                        SetValidateClientError(context, "invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.IsActive)
            {
                SetValidateClientError(context, "invalid_clientId", string.Format("Client {0} is inactive.", client.AuthClientId));
                return Task.FromResult<object>(null);
            }

            if (client.AccessTokenExpireTimeSpan.HasValue)
            {
                context.Options.AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(client.AccessTokenExpireTimeSpan.Value);
            }
            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());
            context.OwinContext.Set<AuthClient>("as:client", client);

            context.Validated(context.ClientId);
            return base.ValidateClientAuthentication(context);
        }

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "password". This occurs when the user has provided name and password
        /// credentials directly into the client application's user interface, and the client application is using those to acquire an "access_token" and
        /// optional "refresh_token". If the web application supports the
        /// resource owner credentials grant type it must validate the context.Username and context.Password as appropriate. To issue an
        /// access token the context.Validated must be called with a new ticket containing the claims about the resource owner which should be associated
        /// with the access token. The application should take appropriate measures to ensure that the endpoint isn’t abused by malicious callers.
        /// The default behavior is to reject this grant type.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            var forceLogin = context.OwinContext.Get<bool>("as:ForceLogin");
            var rememberMe = context.OwinContext.Get<bool>("as:RememberMe");
            var impersonate = context.OwinContext.Get<bool>("as:Impersonate");
            var impersonatingUserId = context.OwinContext.Get<long?>("as:ImpersonatingUserId");
            var revokeImpersonate = context.OwinContext.Get<bool>("as:Revoke_Impersonate");
            var userId = context.OwinContext.Get<long?>("as:UserId");

            CoreSecurity.ApiIdentity identity = null;

            if (allowedOrigin == null)
            {
                allowedOrigin = "*";
            }

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            if (impersonate == false)
            {
                if (revokeImpersonate && userId != null)
                {
                    identity = await Task.Run(() =>
                    {
                        return AuthenticationCommands.GenerateApiIdentityByUserId(context, userId.Value);
                    });
                }
                else
                {
                    identity = await Task.Run(() =>
                    {
                        return AuthenticationCommands.AuthenticateUsernamePassword(context);
                    });
                }

                if (identity == null)
                {
                    context.Rejected();
                    context.SetError("invalid_grant", "Invalid user name or password.");
                }
            }
            else
            {
                if (impersonatingUserId != null && impersonatingUserId.Value > 0)
                {
                    identity = await Task.Run(() =>
                    {
                        return AuthenticationCommands.ImpersonateUser(context, impersonatingUserId.Value);
                    });

                    if (identity == null)
                    {
                        context.Rejected();
                        context.SetError("invalid_grant", "Failed to impersonate.");
                    }
                }
                else
                {
                    context.Rejected();
                    context.SetError("invalid_grant", "Invalid attempt to impersonate.");
                }
            }

            if (identity == null)
            {
                context.Response.Headers.Add(Constants.OrbitChallengeFlag, new[] { ((int)HttpStatusCode.Unauthorized).ToString() }); //Little trick to get this to throw 401, refer to AuthenticationMiddleware for more  
                return;
            }

            string authToken = AuthenticationCommands.GenerateAuthToken(identity.Name, !CoreSecurity.Configuration.Current.MultipleInstanceEnabled, forceLogin);

            if (string.IsNullOrEmpty(authToken))
            {
                context.Rejected();
                context.SetError("session_rejected", string.Format("The user {0}, is already logged in with other device/machine.", context.UserName));
                context.Response.Headers.Add(Constants.OrbitChallengeFlag, new[] { ((int)HttpStatusCode.Forbidden).ToString() }); //Little trick to get this to throw 401, refer to AuthenticationMiddleware for more
                return;
            }
            else
            {
                var userAuthToken = identity.Claims.FirstOrDefault(c => c.Type == CoreSecurity.ApiIdentity.AuthTokenClaimType);
                if (userAuthToken != null)
                {
                    identity.RemoveClaim(userAuthToken);
                }

                identity.AddClaim(new Claim(CoreSecurity.ApiIdentity.AuthTokenClaimType, authToken));
                context.OwinContext.Set<string>("as:UserAuthToken", authToken);
            }

            if (context.ClientId != null)
            {
                identity.AddClaim(new Claim(CoreSecurity.ApiIdentity.AuthClientClaimType, context.ClientId));
            }

            var authDictonary = new Dictionary<string, string>
            {
                {
                    "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                },
                {
                    "userName", identity.Name
                },
                {
                    "loginName", context.UserName ?? identity.Name
                }
            };

            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties(authDictonary) { IsPersistent = rememberMe });
            context.Validated(ticket);

            if (CoreSecurity.Configuration.Current.CookieAuthenticationEnabled)
            {
                CoreSecurity.ApiIdentity cookiesIdentity = new CoreSecurity.ApiIdentity(identity.Claims, CoreSecurity.Configuration.Current.AuthCookieName);
                context.OwinContext.Authentication.SignOut(CoreSecurity.Configuration.Current.AuthCookieName);
                context.OwinContext.Authentication.SignIn(new AuthenticationProperties(authDictonary) { IsPersistent = rememberMe }, cookiesIdentity);
            }
        }

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "refresh_token". This occurs if your application has issued a "refresh_token"
        /// along with the "access_token", and the client is attempting to use the "refresh_token" to acquire a new "access_token", and possibly a new "refresh_token".
        /// To issue a refresh token the an Options.RefreshTokenProvider must be assigned to create the value which is returned. The claims and properties
        /// associated with the refresh token are present in the context.Ticket. The application must call context.Validated to instruct the
        /// Authorization Server middleware to issue an access token based on those claims and properties. The call to context.Validated may
        /// be given a different AuthenticationTicket or ClaimsIdentity in order to control which information flows from the refresh token to
        /// the access token. The default behavior when using the OAuthAuthorizationServerProvider is to flow information from the refresh token to
        /// the access token unmodified.
        /// See also http://tools.ietf.org/html/rfc6749#section-6
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change authentication ticket for refresh token requests
            var newIdentity = new CoreSecurity.ApiIdentity(context.Ticket.Identity);

            // TODO: to add new claims....

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Called at the final stage of a successful Token endpoint request. An application may implement this call in order to do any final
        /// modification of the claims being used to issue access or refresh tokens. This call may also be used in order to add additional
        /// response parameters to the Token endpoint's JSON response body.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Called before the TokenEndpoint redirects its response to the caller.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            var userAuthToken = new UserAuthToken(context.AccessToken)
            {
                AuthClientId = Convert.ToString(context.AdditionalResponseParameters.GetValueByKey("as:client_id")),
                ExpiresUtc = DateTimeOffset.Parse(context.AdditionalResponseParameters.GetValueByKey(".expires").ToString()),
                IssuedUtc = DateTimeOffset.Parse(context.AdditionalResponseParameters.GetValueByKey(".issued").ToString()),
                UserId = Convert.ToInt64(context.Identity.FindFirst(CoreSecurity.ApiIdentity.UserIdClaimType).Value),
                UserAuthTokenId = Convert.ToString(context.Identity.FindFirst(CoreSecurity.ApiIdentity.AuthTokenClaimType).Value),
                IsLoggedIn = true,
                IPAddress = context.OwinContext.Request.RemoteIpAddress,
                UserAgent = context.Request.Headers.Get("User-Agent")
            };

            bool isSaved = AuthenticationCommands.SaveUserAuthToken(userAuthToken);
            return base.TokenEndpointResponse(context);
        }

        /// <summary>
        /// Called for each request to the Token endpoint to determine if the request is valid and should continue.
        /// The default behavior when using the OAuthAuthorizationServerProvider is to assume well-formed requests, with
        /// validated client credentials, should continue processing. An application may add any additional constraints.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>
        /// Task to enable asynchronous execution
        /// </returns>
        public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            return base.ValidateTokenRequest(context);
        }

        /// <summary>
        /// Sets the validate client error.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        /// <param name="errorDescription">The error description.</param>
        private void SetValidateClientError(OAuthValidateClientAuthenticationContext context, string error, string errorDescription)
        {
            context.Rejected();
            context.SetError(error, errorDescription);

            if (context.OwinContext.Request.Headers.ContainsKey("Origin")
                && !context.OwinContext.Request.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                var origin = context.OwinContext.Request.Headers.GetValues("Origin");
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { origin.FirstOrDefault() });
            }
        }
    }
}