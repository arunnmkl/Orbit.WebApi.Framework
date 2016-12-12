using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using Orbit.Messaging.Models;
using Orbit.Messaging.SignalR;

namespace Orbit.Messaging
{
    public class ChatHub : BaseHub
    {
        /// <summary>
        /// Called when the connection connects to this hub instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" />
        /// </returns>
        public override Task OnConnected()
        {
            Debug.WriteLine("Hub OnConnected {0}\n", Context.ConnectionId);
            var pClient = PingClient.Instance;

            var user = PingClient.ComposeUser(Context.ConnectionId, ChatContext.UserId, ChatContext.Username, ChatContext.SecurityId, ChatContext.Roles);
            pClient.Connect(Context.ConnectionId, user);
            Clients.Caller.UserId = ChatContext.UserId;
            Clients.Caller.UserName = ChatContext.Username;
            Clients.Caller.Group = ChatContext.Roles;

            Clients.All.onlineUserCount(pClient.GetOnlineUserCount());

            return (base.OnConnected());
        }

        /// <summary>
        /// Called when a connection disconnects from this hub gracefully or due to a timeout.
        /// </summary>
        /// <param name="stopCalled">true, if stop was called on the client closing the connection gracefully;
        /// false, if the connection has been lost for longer than the
        /// <see cref="P:Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout" />.
        /// Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" />
        /// </returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            Debug.WriteLine("Hub OnDisconnected {0}\n", Context.ConnectionId);
            var pClient = PingClient.Instance;
            User result = UserInfo(pClient);

            pClient.Disconnect(Context.ConnectionId);
            Clients.AllExcept(Context.ConnectionId).onlineUserCount(pClient.GetOnlineUserCount());
            Clients.All.offline(result);
            return (base.OnDisconnected(stopCalled));
        }

        /// <summary>
        /// Called when the connection reconnects to this hub instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" />
        /// </returns>
        public override Task OnReconnected()
        {
            Debug.WriteLine("Hub OnReconnected {0}\n", Context.ConnectionId);
            Clients.AllExcept(Context.ConnectionId).onlineUserCount(PingClient.Instance.GetOnlineUserCount());
            return (base.OnDisconnected(true));
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="message">The message.</param>
        public async Task AddMessage(string name, string message, Guid? userSecurityId = null)
        {
            Debug.WriteLine("Hub AddMessage {0} {1} {2}\n", name, message, userSecurityId);
            if (userSecurityId.HasValue)
            {
                var pongUser = PingClient.Instance.GetUser(userSecurityId.Value);
                if (pongUser != null)
                {
                    Clients.Client(pongUser.ConnectionId).addMessage(name, message);

                    var history = new History
                    {
                        From = ChatContext.SecurityId.ToString(),
                        To = userSecurityId.Value.ToString(),
                        Message = message
                    };

                    var result = await Http.Client.PostAsync<History, bool>("chat/history", history, AccessToken);
                }
            }
            else
            {
                Clients.All.addMessage(name, message);
            }
        }

        /// <summary>
        /// Heartbeats this instance.
        /// </summary>
        public void Heartbeat()
        {
            Debug.WriteLine("Hub Heartbeat\n");
            Clients.All.heartbeat();
        }

        /// <summary>
        /// Sends the hello object.
        /// </summary>
        /// <param name="hello">The hello.</param>
        public void SendHelloObject(HelloModel hello)
        {
            Debug.WriteLine("Hub hello {0} {1}\n", hello.Molly, hello.Age);
            Clients.All.sendHelloObject(hello);
        }

        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <returns></returns>
        public User Register()
        {
            var user = PingClient.ComposeUser(Context.ConnectionId, ChatContext.UserId, ChatContext.Username, ChatContext.SecurityId, ChatContext.Roles);
            var pClient = PingClient.Instance;
            pClient.Connect(Context.ConnectionId, user);
            foreach (var groupname in ChatContext.Roles)
            {
                Groups.Add(Context.ConnectionId, groupname);
            }

            User result = UserInfo(pClient);
            Clients.AllExcept(Context.ConnectionId).online(result);

            return result;
        }

        /// <summary>
        /// Groups the hand shake.
        /// </summary>
        public void GroupHandShake()
        {
            var user = PingClient.Instance.GetUser(Context.ConnectionId);
            Clients.Groups(user.Groups).groupHandShake(user.UserName);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        [AllowAnonymous]
        public void Close()
        {
            OnDisconnected(true);
        }

        /// <summary>
        /// Sets the timer.
        /// </summary>
        /// <param name="timeInSeconds">The time in seconds.</param>
        public void SetTimer(int timeInSeconds)
        {
            PingClient.Instance.SetTimer(timeInSeconds);
        }

        /// <summary>
        /// Users the information.
        /// </summary>
        /// <param name="pClient">The p client.</param>
        /// <returns>user information</returns>
        private User UserInfo(PingClient pClient)
        {
            return pClient.GetUser(Context.ConnectionId);
        }
    }
}