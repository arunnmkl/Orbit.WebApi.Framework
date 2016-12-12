namespace Orbit.Messaging.Models
{
    /// <summary>
    /// Defines a history object for the chat system
    /// </summary>
    public class History
    {
        /// <summary>
        /// Gets or sets the recipient string representation of the GUID
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the sender string representation of the GUID
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the Message content
        /// </summary>
        public string Message { get; set; }
    }
}