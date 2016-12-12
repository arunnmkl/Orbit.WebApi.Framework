using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Orbit.WebApi.Core.Security;
using Orbit.WebApi.Extensions.Authentication;

namespace Orbit.WebApi.Api.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiControllerBase
    {
        [AllowAnonymous]
        public HttpResponseMessage Get(
            string external_access_token = ""
            , string provider = ""
            , string registered_user = ""
            , string external_user_name = ""
            , string local_Bare_token = "")
        {
            var result = string.Format(@"external_access_token={0}
                                        &provider={1}
                                        &registered_user={2}
                                        &external_user_name={3},
                                        &Logged_User={4},
                                        &Is_Authenticated={5},
                                        &local_bearer_token={6}",
                                            external_access_token,
                                            provider,
                                            registered_user,
                                            external_user_name,
                                            User.Identity.Name,
                                            User.Identity.IsAuthenticated,
                                            local_Bare_token);

            return Request.CreateResponse(result);
        }

        [Route("details")]
        public HttpResponseMessage GetDetails()
        {
            ApiPrincipal principal = Request.GetRequestContext().Principal as ApiPrincipal;

            if (principal != null && principal.Identity.IsAuthenticated)
            {
                string name = principal.Identity.Name;
                string authenticationType = principal.Identity.AuthenticationType;
                string username = principal.Username;
                Guid securityId = principal.SecurityId;
                string userAuthTokenId = principal.UserAuthTokenId;
                string userAuthClient = principal.AuthClient;
                IEnumerable<string> roles = principal.Roles;
                int timeInSeconds = 0;

                var userAuthToken = AuthenticationCommands.GetUserAuthTokenById(userAuthTokenId);

                if (userAuthToken != null)
                {
                    // Validate expiration time if present
                    DateTimeOffset currentUtc = Extensions.Startup.OAuthBearerOptions.SystemClock.UtcNow;
                    timeInSeconds = (int)((userAuthToken.ExpiresUtc - currentUtc).TotalSeconds);
                }


                object responseMessage = new
                {
                    Name = name,
                    AuthenticationType = authenticationType,
                    Username = username,
                    SecurityId = securityId,
                    Roles = roles,
                    UserAuthToken = userAuthTokenId,
                    TimeInSeconds = timeInSeconds,
                    AuthClient = userAuthClient
                };

                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
                return Request.CreateResponse(new ObjectContent<object>(responseMessage, jsonFormatter));
            }


            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Token Expires");
        }
    }
}
