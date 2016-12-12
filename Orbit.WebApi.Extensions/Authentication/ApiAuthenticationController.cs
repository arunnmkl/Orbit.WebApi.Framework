using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Orbit.WebApi.Core;
using Orbit.WebApi.Core.Filters;
using Orbit.WebApi.Extensions.Common;
using Orbit.WebApi.Extensions.Owin.Externals;
using Orbit.WebApi.Security;
using CoreSecurity = Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Extensions.Authentication
{
    /// <summary>
    /// Base controller for api authentication.
    /// </summary>
    /// <seealso cref="Orbit.WebApi.ApiControllerBase" />
    [RoutePrefix("api/Authentication")]
    public class ApiAuthenticationController : ApiControllerBase
    {
        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <returns></returns>
        [Route("claims")]
        [HttpGet]
        [ResponseType(typeof(AuthClaim))]
        public HttpResponseMessage GetDetails()
        {
            CoreSecurity.ApiPrincipal principal = Request.GetRequestContext().Principal as CoreSecurity.ApiPrincipal;

            if (principal != null && principal.Identity.IsAuthenticated)
            {
                var identity = principal.Identity as CoreSecurity.ApiIdentity;
                string name = identity.Name;
                string authenticationType = identity.AuthenticationType;
                string username = principal.Username;
                Guid securityId = principal.SecurityId;
                string userAuthTokenId = principal.UserAuthTokenId;
                string userAuthClient = principal.AuthClient;
                IEnumerable<string> roles = principal.Roles;
                string fullName = identity.FullName;
                long? impersonatingUserId = identity.ImpersonatingUserId;
                bool isImpersonated = identity.IsImpersonated;
                string claimType = identity.Label;
                string culture = identity.UserCulture;
                int timeInSeconds = 0;

                var userAuthToken = AuthenticationCommands.GetUserAuthTokenById(userAuthTokenId);

                if (userAuthToken != null)
                {
                    // Validate expiration time if present
                    DateTimeOffset currentUtc = Startup.OAuthBearerOptions.SystemClock.UtcNow;
                    timeInSeconds = (int)((userAuthToken.ExpiresUtc - currentUtc).TotalSeconds);
                }


                object responseMessage = new AuthClaim
                {
                    Name = name,
                    AuthenticationType = authenticationType,
                    Username = username,
                    SecurityId = securityId,
                    Roles = roles,
                    UserAuthToken = userAuthTokenId,
                    TimeInSeconds = timeInSeconds,
                    AuthClient = userAuthClient,
                    FullName = fullName,
                    ImpersonatingUserId = impersonatingUserId,
                    IsImpersonated = isImpersonated,
                    ClaimTypeLabel = claimType,
                    UserCulture = culture
                };

                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
                return Request.CreateResponse(new ObjectContent<object>(responseMessage, jsonFormatter));
            }


            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Token Expires");
        }

        /// <summary>
        /// Impersonates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("ImpersonateUser")]
        public async Task<HttpResponseMessage> ImpersonateUser(string username)
        {
            using (AuthorizationScope authScope = new AuthorizationScope("Impersonate", "Impersonate"))
            {
                var userId = AuthContext.UserId;
                var clientId = AuthContext.ClientId;

                var tokenServiceResponse = await Impersonate(username, userId, clientId);
                if (tokenServiceResponse.IsSuccessStatusCode)
                {
                    SignOut();
                    var accessToken = await tokenServiceResponse.Content.ReadAsAsync<Token>();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, accessToken);
                    if (CoreSecurity.Configuration.Current.CookieAuthenticationEnabled)
                    {
                        response.SetAuthentication(accessToken.AccessToken, false, DateTimeOffset.Parse(accessToken.Expires));
                    }

                    return response;
                }
                else
                {
                    var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();

                    HttpResponseMessage response = Request.CreateResponse(tokenServiceResponse.StatusCode);
                    response.Content = new StringContent(responseString, Encoding.UTF8, "application/json");
                    return response;
                }
            }
        }

        /// <summary>
        /// Impersonates the user.
        /// </summary>
        /// <param name="impersonateId">The impersonate identifier.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Impersonate/{impersonateId:long}/identifier")]
        public async Task<HttpResponseMessage> ImpersonateUser(long impersonateId)
        {
            using (AuthorizationScope authScope = new AuthorizationScope("Impersonate", "Impersonate"))
            {
                var userId = AuthContext.UserId;
                var clientId = AuthContext.ClientId;

                var tokenServiceResponse = await Impersonate(string.Empty, userId, clientId, impersonateId);
                if (tokenServiceResponse.IsSuccessStatusCode)
                {
                    SignOut();
                    var accessToken = await tokenServiceResponse.Content.ReadAsAsync<Token>();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, accessToken);
                    if (CoreSecurity.Configuration.Current.CookieAuthenticationEnabled)
                    {
                        response.SetAuthentication(accessToken.AccessToken, false, DateTimeOffset.Parse(accessToken.Expires));
                    }

                    return response;
                }
                else
                {
                    var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();

                    HttpResponseMessage response = Request.CreateResponse(tokenServiceResponse.StatusCode);
                    response.Content = new StringContent(responseString, Encoding.UTF8, "application/json");
                    return response;
                }
            }
        }

        /// <summary>
        /// Revokes the impersonation.
        /// </summary>
        /// <returns>
        /// the authentication token
        /// </returns>
        [HttpPost]
        [Route("RevokeImpersonation")]
        [ResponseType(typeof(Token))]
        public async Task<HttpResponseMessage> RevokeImpersonation()
        {
            if (AuthContext.ApiIdentity.IsImpersonated)
            {
                var userId = AuthContext.ApiIdentity.ImpersonatingUserId;
                var clientId = AuthContext.ClientId;

                var tokenServiceResponse = await RevokeImpersonation(userId.Value, clientId);
                if (tokenServiceResponse.IsSuccessStatusCode)
                {
                    SignOut();
                    var accessToken = await tokenServiceResponse.Content.ReadAsAsync<Token>();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, accessToken);
                    if (CoreSecurity.Configuration.Current.CookieAuthenticationEnabled)
                    {
                        response.SetAuthentication(accessToken.AccessToken, false, DateTimeOffset.Parse(accessToken.Expires));
                    }

                    return response;
                }
                else
                {
                    var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();

                    HttpResponseMessage response = Request.CreateResponse(tokenServiceResponse.StatusCode);
                    response.Content = new StringContent(responseString, Encoding.UTF8, "application/json");
                    return response;
                }
            }
            else
            {
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, "Unable to remove impersonation because there is no impersonation");
            }
        }

        /// <summary>
        /// Logins the specified bearer login.
        /// </summary>
        /// <param name="bearerLogin">The bearer login.</param>
        /// <returns>
        /// login response
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<HttpResponseMessage> Login(ApiLogin bearerLogin)
        {
            string loginMessage = string.Empty;
            try
            {
                if (bearerLogin == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid user data");
                }

                var tokenServiceResponse = await Authenticate(bearerLogin);
                if (tokenServiceResponse.IsSuccessStatusCode)
                {
                    var accessToken = await tokenServiceResponse.Content.ReadAsAsync<Token>();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, accessToken);
                    if (CoreSecurity.Configuration.Current.CookieAuthenticationEnabled)
                    {
                        response.SetAuthentication(accessToken.AccessToken, bearerLogin.RememberMe, DateTimeOffset.Parse(accessToken.Expires));
                    }

                    return response;
                }
                else
                {
                    var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();

                    HttpResponseMessage response = Request.CreateResponse(tokenServiceResponse.StatusCode);
                    response.Content = new StringContent(responseString, Encoding.UTF8, "application/json");
                    return response;
                }
            }
            catch (Exception ex)
            {
                loginMessage = ex.ToString();
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized, loginMessage);
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        /// <returns>
        /// HTTP response message including the status code and data
        /// </returns>
        [HttpPut]
        [Route("Logout")]
        [AllowAnonymous]
        [NoResponseCookie]
        public IHttpActionResult Logout()
        {
            SignOut();
            return this.Ok(new
            {
                Message = "Logout successful."
            });
        }

        /// <summary>
        /// Represents an event that is raised when the sign-out operation is complete.
        /// </summary>
        private void SignOut()
        {
            ExternalProvider.SignOut(this.Request, HttpContext.Current.User.Identity.AuthenticationType);
            AuthenticationCommands.SetTokenExpires();
        }

        /// <summary>
        /// Authenticates the specified login model.
        /// </summary>
        /// <param name="loginModel">The login model.</param>
        /// <returns>
        /// client http response
        /// </returns>
        private async Task<HttpResponseMessage> Authenticate(ApiLogin loginModel)
        {
            // Ugly hack: I use a server-side HTTP POST because I cannot directly invoke the service (it is deeply hidden in the OAuthAuthorizationServerHandler class)
            HttpRequest request = HttpContext.Current.Request;
            string tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/Token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", loginModel.Username),
                    new KeyValuePair<string, string>("password", loginModel.Password)
                };

                if (string.IsNullOrEmpty(loginModel.ClientId) == false)
                {
                    requestParams.Add(new KeyValuePair<string, string>("client_id", loginModel.ClientId));
                }

                if (loginModel.ForceLogin)
                {
                    requestParams.Add(new KeyValuePair<string, string>("forceLogin", loginModel.ForceLogin.ToString()));
                }

                if (loginModel.RememberMe)
                {
                    requestParams.Add(new KeyValuePair<string, string>("rememberMe", loginModel.RememberMe.ToString()));
                }

                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                return await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
            }
        }

        /// <summary>
        /// Impersonates the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="impersonateId">The impersonate identifier.</param>
        /// <returns>
        /// client http response
        /// </returns>
        private async Task<HttpResponseMessage> Impersonate(string username, long userId, string clientId, long? impersonateId = null)
        {
            Random r = new Random(99999);
            var newR = r.Next();
            // Ugly hack: I use a server-side HTTP POST because I cannot directly invoke the service (it is deeply hidden in the OAuthAuthorizationServerHandler class)
            HttpRequest request = HttpContext.Current.Request;
            string tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/Token";
            using (var client = new HttpClient())
            {
                var impersonatingUserId = userId * newR;
                var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("impersonate", "true"),
                    new KeyValuePair<string, string>("impersonatingUserId", CoreSecurity.HashEncryptor.Encrypt(Convert.ToString(impersonatingUserId))),
                    new KeyValuePair<string, string>("random", CoreSecurity.HashEncryptor.Encrypt(Convert.ToString(newR)))
                };

                if (string.IsNullOrEmpty(AuthContext.ClientId) == false)
                {
                    requestParams.Add(new KeyValuePair<string, string>("client_id", clientId));
                }

                if (impersonateId != null && impersonateId.HasValue)
                {
                    requestParams.Add(new KeyValuePair<string, string>("user_id", Convert.ToString(impersonateId.Value)));
                }

                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                return await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
            }
        }

        /// <summary>
        /// Revokes the impersonation.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> RevokeImpersonation(long userId, string clientId)
        {
            // Ugly hack: I use a server-side HTTP POST because I cannot directly invoke the service (it is deeply hidden in the OAuthAuthorizationServerHandler class)
            HttpRequest request = HttpContext.Current.Request;
            string tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/Token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("user_id",Convert.ToString(userId)),
                    new KeyValuePair<string, string>("revoke_impesonation", "true"),
                    new KeyValuePair<string, string>("client_id", clientId)
                };

                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                return await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class AuthClaim
        {
            /// <summary>
            /// Gets or sets the authentication client.
            /// </summary>
            /// <value>
            /// The authentication client.
            /// </value>
            public string AuthClient { get; set; }
            /// <summary>
            /// Gets or sets the type of the authentication.
            /// </summary>
            /// <value>
            /// The type of the authentication.
            /// </value>
            public string AuthenticationType { get; set; }
            /// <summary>
            /// Gets or sets the claim type label.
            /// </summary>
            /// <value>
            /// The claim type label.
            /// </value>
            public string ClaimTypeLabel { get; set; }
            /// <summary>
            /// Gets or sets the full name.
            /// </summary>
            /// <value>
            /// The full name.
            /// </value>
            public string FullName { get; set; }
            /// <summary>
            /// Gets or sets the impersonating user identifier.
            /// </summary>
            /// <value>
            /// The impersonating user identifier.
            /// </value>
            public long? ImpersonatingUserId { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether this instance is impersonated.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is impersonated; otherwise, <c>false</c>.
            /// </value>
            public bool IsImpersonated { get; set; }
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the roles.
            /// </summary>
            /// <value>
            /// The roles.
            /// </value>
            public IEnumerable<string> Roles { get; set; }
            /// <summary>
            /// Gets or sets the security identifier.
            /// </summary>
            /// <value>
            /// The security identifier.
            /// </value>
            public Guid SecurityId { get; set; }
            /// <summary>
            /// Gets or sets the time in seconds.
            /// </summary>
            /// <value>
            /// The time in seconds.
            /// </value>
            public int TimeInSeconds { get; set; }
            /// <summary>
            /// Gets or sets the user authentication token.
            /// </summary>
            /// <value>
            /// The user authentication token.
            /// </value>
            public string UserAuthToken { get; set; }
            /// <summary>
            /// Gets or sets the user culture.
            /// </summary>
            /// <value>
            /// The user culture.
            /// </value>
            public string UserCulture { get; set; }
            /// <summary>
            /// Gets or sets the username.
            /// </summary>
            /// <value>
            /// The username.
            /// </value>
            public string Username { get; set; }
        }
    }
}
