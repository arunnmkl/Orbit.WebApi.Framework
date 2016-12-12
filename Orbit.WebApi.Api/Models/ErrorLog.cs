namespace Orbit.WebApi.Api.Models
{
    /// <summary>
    /// contains all details of ErrorLog like ErrorLogId, UserName, Source, etc.
    /// </summary>
    public class ErrorLog
    {
        /// <summary>
        /// Gets or sets the ErrorLogId
        /// </summary>
        public int ErrorLogId { get; set; }

        /// <summary>
        /// Gets or sets the User Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the Source
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the StackTrace
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the ApplicationUrl
        /// </summary>
        public string ApplicationUrl { get; set; }

        /// <summary>
        /// Gets or sets the inner exception.
        /// </summary>
        /// <value>
        /// The inner exception.
        /// </value>
        public string InnerException { get; set; }

        /// <summary>
        /// Gets or sets the additional message.
        /// </summary>
        /// <value>
        /// The additional message.
        /// </value>
        public string AdditionalMessage { get; set; }
    }
}