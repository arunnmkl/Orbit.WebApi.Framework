using System;

namespace Orbit.WebApi.Security.Models.Chat
{
    /// <summary>
    /// Defines a history object for the chat system
    /// </summary>
    public class ChatHistory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatHistory" /> class
        /// </summary>
        /// <param name="to">The recipient string representation of the GUID</param>
        /// <param name="from">The sender string representation of the GUID</param>
        /// <param name="message">The message content</param>
        public ChatHistory(string to, string from, string message)
        {
            To = to;
            From = from;
            Message = message;

            DateTimestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatHistory" /> class
        /// </summary>
        public ChatHistory()
        {
        }

        /// <summary>
        /// Gets or sets the recipient string representation of the GUID
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the sender string representation of the GUID
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the DateTimestamp of the message
        /// </summary>
        /// <value>
        /// The date timestamp.
        /// </value>
        public DateTimeOffset DateTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the Message content
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }
    }
}
