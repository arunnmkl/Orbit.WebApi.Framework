using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Orbit.WebApi.Api.Models;
using Orbit.WebApi.Core;
using Orbit.WebApi.Extensions;
using Orbit.WebApi.Extensions.Authentication;
using Orbit.WebApi.Extensions.Common;
using Orbit.WebApi.Extensions.Owin.Externals;
using Orbit.WebApi.Extensions.Validation;
using Orbit.WebApi.Security.Models;
using ApiSecurity = Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Api.Controllers
{
    /// <summary>
    /// This is a controller class that handles the login part.
    /// </summary>
    /// <seealso cref="Orbit.WebApi.ApiControllerBase" />
    [RoutePrefix("api/Account")]
    public class AccountController : ApiControllerBase
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(true);
        }

        /// <summary>
        /// Externals the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        [HttpGet]
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public IHttpActionResult ExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ExternalAuthenticationChallengeResult(provider, this);
            }

            string redirectUri;
            bool isValidUri = CommonValidations.TryParseRedirectUri(Request, out redirectUri);

            if (isValidUri == false)
            {
                if (string.IsNullOrEmpty(redirectUri))
                {
                    return BadRequest("Invalid redirect URI(redirect_uri).");
                }

                return BadRequest(redirectUri);
            }

            try
            {
                ExternalData externalLoginData = new ExternalData(User.Identity as ClaimsIdentity);
                if (externalLoginData == null)
                {
                    return InternalServerError();
                }

                if (externalLoginData.LoginProvider != provider)
                {
                    ExternalProvider.SignOut(Request, DefaultAuthenticationTypes.ExternalCookie);
                    return new ExternalAuthenticationChallengeResult(provider, this);
                }

                externalLoginData.LocalBearerToken = GenerateLocalAccessTokenResponse(User.Identity as ClaimsIdentity);

                redirectUri = ExternalProvider.GetCompleteRedirectUri(redirectUri, externalLoginData);
                return Redirect(redirectUri);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Posts the specified login object to authentication of the API.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="externalAccessToken">The external access token.</param>
        /// <returns>
        ///   <c>HttpStatusCode.OK</c> if [login success]; otherwise, <c>HttpStatusCode.Unauthorized</c>.
        /// </returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {

            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            {
                return BadRequest("Provider or external access token is not sent");
            }

            var verifiedAccessToken = await CommonValidations.VerifyExternalAccessToken(provider, externalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            UserIdentity userIdentity = AuthenticationCommands.FindLoginProvider(provider, verifiedAccessToken.user_id);

            bool hasRegistered = userIdentity != null;

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }

            string authToken = AuthenticationCommands.GenerateAuthToken(userIdentity.Username, !ApiSecurity.Configuration.Current.MultipleInstanceEnabled);

            if (string.IsNullOrEmpty(authToken))
            {
                return BadRequest(string.Format("The user {0}, is already logged in with other device/machine.", userIdentity.Username));
            }
            else
            {
                userIdentity.UserAuthTokenId = authToken;
            }

            //generate access token response
            var accessTokenResponse = Helper.GenerateLocalAccessTokenResponse(userIdentity);

            return Ok(accessTokenResponse);

        }

        /// <summary>
        /// Posts the specified login.
        /// </summary>
        /// <param name="loginModel">The login.</param>
        /// <returns></returns>  
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Post(Models.BearerLogin loginModel)
        {
            string loginMessage = string.Empty;
            try
            {
                if (loginModel == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid user data");
                }

                var tokenServiceResponse = await Authenticate(loginModel);
                if (tokenServiceResponse.IsSuccessStatusCode)
                {
                    var accessToken = await tokenServiceResponse.Content.ReadAsAsync<Token>();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, accessToken);
                    response.SetAuthentication(accessToken.AccessToken, loginModel.RememberMe, DateTimeOffset.Parse(accessToken.Expires));
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

        [AllowAnonymous]
        [Route("Register")]
        [ApiExceptionFilter]
        public IHttpActionResult Register(UserRegistration userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = AuthenticationCommands.AddUser(userModel.UserName, userModel.Password);

            if (result < 1)
            {
                return BadRequest("Unable to create user.");
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [AllowAnonymous]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await CommonValidations.VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            UserIdentity userIdentity = AuthenticationCommands.FindLoginProvider(model.Provider, verifiedAccessToken.user_id);

            bool hasRegistered = userIdentity != null;

            if (hasRegistered)
            {
                return BadRequest("External user is already registered");
            }

            var userId = AuthenticationCommands.AddUser(model.UserName, model.Password);

            if (userId < 1)
            {
                return BadRequest("Unable to create user.");
            }

            var authProvider = new AuthProvider
            {
                LoginProvider = model.Provider,
                ProviderKey = verifiedAccessToken.user_id,
                UserId = userId
            };

            var isInserted = AuthenticationCommands.AddNewUserLoginProvider(authProvider);

            //generate access token response
            var accessTokenResponse = Helper.GenerateLocalAccessTokenResponse(AuthenticationCommands.FindUserByUserId(userId));

            return Ok(accessTokenResponse);
        }

        /// <summary>
        /// Determines whether this instance is authorized.
        /// </summary>
        /// <returns>send whether the user is authorized or not</returns>
        [AllowAnonymous]
        [Route("IsAuthorized")]
        [HttpGet]
        public HttpResponseMessage IsAuthorized()
        {
            return Request.CreateResponse(User.Identity.IsAuthenticated);
        }

        /// <summary>
        /// Posts the specified login.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<HttpResponseMessage> Login(Models.BearerLogin model)
        {
            if (model == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid user data");
            }

            var tokenServiceResponse = await Authenticate(model);
            if (tokenServiceResponse.IsSuccessStatusCode)
            {
                var accessToken = await tokenServiceResponse.Content.ReadAsAsync<Token>();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, accessToken);
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

        /// <summary>
        /// Generates the local access token response.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// the local access token string
        /// </returns>
        private string GenerateLocalAccessTokenResponse(ClaimsIdentity identity)
        {
            var tokenExpiration = TimeSpan.FromDays(1);

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Helper.ProtectAccessToken(ticket);

            return accessToken;
        }

        /// <summary>
        /// Validates the client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="loginMessage">The login message.</param>
        /// <param name="client">The client.</param>
        /// <returns>is valid client</returns>
        private static bool ValidateClient(string clientId, ref string loginMessage, out AuthClient client)
        {
            client = null;

            if (string.IsNullOrEmpty(clientId))
            {
                loginMessage = "Client id should be sent.";
                return false;
            }
            else
            {
                client = AuthenticationCommands.FindAuthClient(clientId);
            }

            if (client == null)
            {
                loginMessage = string.Format("Client '{0}' is not registered in the system.", clientId);
                return false;
            }
            else
            {
                if (!client.IsActive)
                {
                    loginMessage = string.Format("Client {0} is inactive.", client.AuthClientId);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Authenticates the specified login model.
        /// </summary>
        /// <param name="loginModel">The login model.</param>
        /// <returns>client http response</returns>
        private static async Task<HttpResponseMessage> Authenticate(Models.BearerLogin loginModel)
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
    }
}
