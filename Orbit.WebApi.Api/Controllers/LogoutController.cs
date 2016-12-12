using System.Web;
using System.Web.Http;
using Orbit.WebApi.Core.Filters;
using Orbit.WebApi.Extensions.Authentication;
using Orbit.WebApi.Extensions.Owin.Externals;

namespace Orbit.WebApi.Api.Controllers
{
    /// <summary>
    /// API controller of the common login resource which is generic in nature.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [AllowAnonymous]
    [RoutePrefix("API")]
    public class LogoutController : ApiController
    {
        /// <summary>
        /// Logouts this instance.
        /// </summary>
        /// <returns>HTTP response message including the status code and data</returns>
        [HttpPost]
        [Route("Logout")]
        [AllowAnonymous]
        [NoResponseCookie]
        public IHttpActionResult Logout()
        {
            ExternalProvider.SignOut(Request, HttpContext.Current.User.Identity.AuthenticationType);
            AuthenticationCommands.SetTokenExpires();
            return this.Ok(new
            {
                message = "Logout successful."
            });
        }
    }
}