using System;

namespace Orbit.WebApi.Security.Models.Chat
{
    /// <summary>
    /// Chat user object.
    /// </summary>
    public class ChatUser
    {
        /// <summary>
        /// Gets or sets the security identifier.
        /// </summary>
        /// <value>
        /// The security identifier.
        /// </value>
        public Guid SecurityId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public string GroupName { get; set; }
    }
}
