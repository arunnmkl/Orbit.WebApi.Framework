using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.AspNet.SignalR;
using Orbit.Messaging.Models;

namespace Orbit.Messaging
{
    public class PingClient
    {
        /// <summary>
        /// The instance
        /// </summary>
        private static readonly PingClient instance = instance ?? new PingClient();

        /// <summary>
        /// The ping timer
        /// </summary>
        private Timer pingTimer;

        /// <summary>
        /// The chat connections
        /// </summary>
        private readonly Dictionary<string, User> chatConnections;

        /// <summary>
        /// The timer minute
        /// </summary>
        private int timerMinute;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static PingClient Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Disconnects the specified connection identifier.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        internal void Disconnect(string connectionId)
        {
            if (chatConnections.Keys.Count > 0)
            {
                foreach (var currentConnection in chatConnections.Keys)
                {
                    if (currentConnection.Equals(connectionId))
                    {
                        chatConnections.Remove(connectionId);
                        break;
                    }
                }
            }

            if (chatConnections.Count == 0)
            {
                pingTimer.Enabled = false;
                pingTimer.Stop();
            }
        }

        /// <summary>
        /// Connects the specified connection identifier.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="user">The user.</param>
        internal void Connect(string connectionId, User user)
        {
            if (chatConnections.Keys.Count > 0)
            {
                if (!chatConnections.Keys.Contains(connectionId))
                {
                    CreateConnection(connectionId, user);
                }
                else
                {
                    var existingUser = chatConnections[connectionId];
                    existingUser.UserName = string.IsNullOrEmpty(user.UserName) ? existingUser.UserName : user.UserName;

                    if (user.Groups != null)
                    {
                        foreach (var groupName in user.Groups)
                        {
                            if (string.IsNullOrEmpty(groupName) == false)
                            {
                                existingUser.Groups = existingUser.Groups ?? new List<string>();
                                if (existingUser.Groups != null && existingUser.Groups.Contains(groupName) == false)
                                {
                                    existingUser.Groups.Add(groupName);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                CreateConnection(connectionId, user);
            }
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <returns>
        /// user info
        /// </returns>
        internal User GetUser(string connectionId)
        {
            if (chatConnections.Count > 0)
            {
                return chatConnections[connectionId];
            }

            return null;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <returns>
        /// user info
        /// </returns>
        internal User GetUser(Guid securityId)
        {
            if (chatConnections.Count > 0)
            {
                return chatConnections.Values.FirstOrDefault(cc => cc.SecurityId == securityId);
            }

            return null;
        }

        /// <summary>
        /// Gets the online user count.
        /// </summary>
        /// <returns>
        /// the online user count
        /// </returns>
        internal int GetOnlineUserCount()
        {
            return chatConnections.Count;
        }

        /// <summary>
        /// Composes the user.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="securityId">The security identifier.</param>
        /// <param name="groups">The groups.</param>
        /// <returns>
        /// composed user object.
        /// </returns>
        internal static User ComposeUser(string connectionId, long userId, string username, Guid securityId, IList<string> groups = null)
        {
            var user = new User
            {
                ConnectionId = connectionId,
                SessionId = DateTime.Now.Ticks,
                UserId = userId,
                UserName = username,
                SecurityId = securityId
            };

            if (groups != null)
            {
                foreach (var groupName in groups)
                {
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        user.Groups = new List<string>() { groupName };
                    }
                }
            }

            return user;
        }

        /// <summary>
        /// Sets the timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        internal void SetTimer(int timer)
        {
            if (timer > 0)
            {
                timerMinute = timer;

                pingTimer = pingTimer ?? new Timer();
                pingTimer.Interval = (1000 * timerMinute);
                pingTimer.Elapsed += OnTimerElapsed;
                pingTimer.AutoReset = true;
            }
            else
            {
                timerMinute = 0;
                pingTimer = pingTimer ?? new Timer();
                pingTimer.Close();
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="PingClient" /> class from being created.
        /// </summary>
        /// <param name="useTimer">if set to <c>true</c> [use timer].</param>
        private PingClient(bool useTimer = false)
        {
            chatConnections = new Dictionary<string, User>();
            pingTimer = new Timer();
            if (useTimer && timerMinute > 0)
            {
                pingTimer.Interval = (1000 * timerMinute);
                pingTimer.Elapsed += OnTimerElapsed;
                pingTimer.AutoReset = true;
            }
            else
            {
                pingTimer.Close();
            }
        }

        /// <summary>
        /// Called when [timer elapsed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            PingClients();
        }

        /// <summary>
        /// Pings the clients.
        /// </summary>
        private void PingClients()
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            foreach (var connectionId in chatConnections.Keys)
            {
                TimeSpan span = new TimeSpan(DateTime.Now.Ticks);
                hubContext.Clients.Client(connectionId).tick(connectionId, chatConnections[connectionId].UserName, span.ToString());
                if (chatConnections[connectionId].Groups != null)
                {
                    foreach (var groupname in chatConnections[connectionId].Groups)
                    {
                        if (!string.IsNullOrEmpty(groupname))
                        {
                            hubContext.Clients.Group(groupname).tick(connectionId, groupname, span.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="user">The user.</param>
        private void CreateConnection(string connectionId, User user)
        {
            if (!pingTimer.Enabled)
            {
                pingTimer.Enabled = true;
                pingTimer.Start();
            }

            chatConnections.Add(connectionId, user);
        }
    }
}
