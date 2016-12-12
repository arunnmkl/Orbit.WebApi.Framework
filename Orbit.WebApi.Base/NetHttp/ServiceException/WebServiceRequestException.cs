using System;

namespace Orbit.WebApi.Base.NetHttp.ServiceException
{
    /// <summary>
    /// Class WebServiceRequestException.
    /// </summary>
    public class WebServiceRequestException : Exception
    {
        /// <summary>
        /// Gets the verb.
        /// </summary>
        /// <value>The verb.</value>
        public string Verb { get; private set; }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceRequestException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="verb">The verb.</param>
        /// <param name="uri">The URI.</param>
        public WebServiceRequestException(string message, string verb, string uri)
            : base(message)
        {
            Verb = verb;
            Uri = uri;
        }
    }
}