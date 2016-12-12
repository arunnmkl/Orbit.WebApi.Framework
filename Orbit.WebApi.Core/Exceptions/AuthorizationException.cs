using System;
using System.Security;

namespace Orbit.WebApi.Core.Exceptions
{
    /// <summary>
    ///  Class to encapsulate authorization exception
    /// </summary>
    /// <seealso cref="SecurityException" />
    [Serializable]
    public class AuthorizationException : SecurityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AuthorizationException(string message) : base(message)
        {
        }
    }
}
