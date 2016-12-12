using System;
using System.Net;

namespace Orbit.WebApi.Core.Exceptions
{
    /// <summary>
    /// Api exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ApiException : Exception
    {
        /// <summary>
        /// The status code
        /// </summary>
        private readonly HttpStatusCode statusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public ApiException(HttpStatusCode statusCode, string message, Exception ex)
            : base(message, ex)
        {
            this.statusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The message.</param>
        public ApiException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            this.statusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public ApiException(HttpStatusCode statusCode)
        {
            this.statusCode = statusCode;
        }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode StatusCode
        {
            get { return statusCode; }
        }
    }
}
