using System;
using System.Linq;
using System.Web.Http;
using Orbit.WebApi.Security;
using Orbit.WebApi.Security.Models.Chat;

namespace Orbit.WebApi.Api.Controllers
{
    [RoutePrefix("api/chat")]
    public class ChatController : ApiController
    {
        [Route("history")]
        public IHttpActionResult Post(ChatHistory history)
        {
            if (history == null)
            {
                return BadRequest("history item is empty");
            }

            using (UserManager um = new UserManager())
            {
                um.SaveChatHistory(history);

                return Ok(true);
            }
        }

        [Route("history/{toUser}")]
        public IHttpActionResult Get(Guid toUser, string searchText = null)
        {
            if (toUser == null)
            {
                return BadRequest("toUser is empty");
            }

            using (UserManager um = new UserManager())
            {
                var history = um.GetChatHistory(AuthContext.SecurityId.ToString(), toUser.ToString(), searchText);
                if (history != null)
                {
                    history.ToList().ForEach(h =>
                    {
                        if (h.From == AuthContext.SecurityId.ToString())
                        {
                            h.Username = "You";
                        }
                    });
                }

                return Ok(history);
            }
        }

        [Route("associatedusers")]
        public IHttpActionResult Get()
        {
            using (UserManager um = new UserManager())
            {
                var users = um.GetAssociatedChatUsers(AuthContext.SecurityId);

                return Ok(users);
            }
        }
    }
}
