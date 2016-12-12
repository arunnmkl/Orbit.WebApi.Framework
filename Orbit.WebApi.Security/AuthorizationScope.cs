using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Orbit.WebApi.Core.Exceptions;
using Orbit.WebApi.Core.Security;
using Orbit.WebApi.Security.Models;

namespace Orbit.WebApi.Security
{
    /// <summary>
    /// Authorization scope class.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class AuthorizationScope : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationScope"/> class.
        /// </summary>
        /// <param name="logicalResource">The logical resource.</param>
        /// <param name="permission">The permission.</param>
        private AuthorizationScope(Resource logicalResource, Permission permission)
            : this(logicalResource.ResourceId, permission.PermissionId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationScope"/> class.
        /// </summary>
        /// <param name="logicalResourceName">Name of the logical resource.</param>
        /// <param name="permissionName">Name of the permission.</param>
        public AuthorizationScope(string logicalResourceName, string permissionName)
            : this(AuthorizationCommands.GetResource(logicalResourceName), AuthorizationCommands.GetPermission(permissionName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationScope"/> class.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <param name="permissionId">The permission identifier.</param>
        public AuthorizationScope(Guid resourceId, int permissionId)
        {
            string securityName = "Anonymous";
            IList<Guid> securityIds = new List<Guid>();
            try
            {
                try
                {
                    ApiPrincipal current = ApiPrincipal.Current;
                    securityName = current.Username;
                    securityIds = current.SecurityIds.ToList();
                }
                catch (SecurityException ex)
                {
                    if (ex.Message != "No current principal")
                    {
                        throw ex;
                    }
                }

                var authorizationType = AuthorizationCommands.CheckAuthorization(resourceId, securityIds, permissionId);
                if (!authorizationType.HasFlag((Enum)Models.Enums.AuthorizationType.Access))
                {
                    var resource = AuthorizationCommands.GetResource(resourceId);
                    var permission = AuthorizationCommands.GetPermission(permissionId);
                    throw new AccessException(resource.Name, securityName, permission.Name);
                }
            }
            finally { }
        }

        #region Interface members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //// Dispose any managed objects
            }
        }
        #endregion
    }
}
