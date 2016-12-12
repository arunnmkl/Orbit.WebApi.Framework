using System;

namespace Orbit.WebApi.Core.Exceptions
{
    /// <summary>
    /// Class to encapsulate access exception
    /// </summary>
    /// <seealso cref="AuthorizationException" />
    [Serializable]
    public class AccessException : AuthorizationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessException"/> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="securityName">Name of the security.</param>
        /// <param name="permissionName">Name of the permission.</param>
        public AccessException(string resourceName, string securityName, string permissionName)
          : base(string.Format("Access Violation on {0} by {1} for {2}", (object)resourceName, (object)securityName, (object)permissionName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessException"/> class.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <param name="securityName">Name of the security.</param>
        /// <param name="permissionId">The permission identifier.</param>
        public AccessException(Guid resourceId, string securityName, int permissionId)
          : base(string.Format("Access Violation on {0} by {1} for {2}", (object)resourceId, (object)securityName, (object)permissionId))
        {
        }
    }
}
