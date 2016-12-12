using System.Net.Http;
using System.Web.Http;
using Orbit.WebApi.Extensions.Authentication;

namespace Orbit.WebApi.Api.Controllers
{
    [RoutePrefix("api/ResourcePermission")]
    public class UserPermissionController : ApiController
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        [Route("User")]
        public HttpResponseMessage GetUserPermissions()
        {
            var permissions = AuthenticationCommands.GetUserPermissions();
            return Request.CreateResponse(permissions);
        }
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        [Route("UserGroup")]
        public HttpResponseMessage GetUserResourcePermission()
        {
            var permissions = AuthenticationCommands.GetUserResourcePermission();
            return Request.CreateResponse(permissions);
        }
    }
}
