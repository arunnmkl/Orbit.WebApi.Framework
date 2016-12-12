using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.WebApi.Core.Exceptions
{
    /// <summary>
    /// Un authorized exception
    /// </summary>
    /// <seealso cref="ApiException" />
    public class UnauthorizedException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        public UnauthorizedException() : base(HttpStatusCode.Unauthorized)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message)
        {
        }
    }
}
