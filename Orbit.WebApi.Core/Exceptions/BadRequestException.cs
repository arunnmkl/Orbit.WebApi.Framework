using System.Net;

namespace Orbit.WebApi.Core.Exceptions
{
    /// <summary>
    /// Bad request exception
    /// </summary>
    /// <seealso cref="ApiException" />
    public class BadRequestException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class.
        /// </summary>
        public BadRequestException() : base(HttpStatusCode.BadRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message)
        {
        }
    }
}
