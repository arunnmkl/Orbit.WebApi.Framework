using System.Web.Http;
using Orbit.WebApi.Extensions.Authentication;

namespace Orbit.WebApi.Api.Controllers
{
    [RoutePrefix("api/RefreshTokens")]
    public class RefreshTokenController : ApiController
    {
        [Authorize(Users = "child")]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(AuthenticationCommands.GetAllRefreshTokens());
        }

        //[Authorize(Users = "Admin")]
        [AllowAnonymous]
        [Route("")]
        public IHttpActionResult Delete(string tokenId)
        {
            var result = AuthenticationCommands.RemoveRefreshToken(tokenId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Token Id does not exist");

        }
    }
}
