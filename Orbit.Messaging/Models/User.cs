using System;
using System.Collections.Generic;

namespace Orbit.Messaging.Models
{
    public class User
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public long SessionId { get; set; }

        /// <summary>
        /// Gets or sets the connection identifier.
        /// </summary>
        /// <value>
        /// The connection identifier.
        /// </value>
        public string ConnectionId { get; set; }

        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        public IList<string> Groups { get; set; }

        /// <summary>
        /// Gets or sets the security identifier.
        /// </summary>
        /// <value>
        /// The security identifier.
        /// </value>
        public Guid SecurityId { get; set; }
    }
}