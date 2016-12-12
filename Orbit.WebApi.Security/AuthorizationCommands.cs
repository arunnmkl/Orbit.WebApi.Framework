using System;
using System.Collections.Generic;
using System.Linq;
using Orbit.WebApi.Core.Exceptions;
using Orbit.WebApi.Security.Models;

namespace Orbit.WebApi.Security
{
    /// <summary>
    /// Authorization commands
    /// </summary>
    internal class AuthorizationCommands
    {
        /// <summary>
        /// The authentication repo
        /// </summary>
        private static AuthSqlRepository authRepo;

        /// <summary>
        /// The permissions
        /// </summary>
        private static Dictionary<int, Permission> permissions;

        /// <summary>
        /// The permissions by name
        /// </summary>
        private static Dictionary<string, Permission> permissionsByName;

        /// <summary>
        /// The resources
        /// </summary>
        private static Dictionary<Guid, Resource> resources;

        /// <summary>
        /// The resources by name
        /// </summary>
        private static Dictionary<string, Resource> resourcesByName;

        /// <summary>
        /// Gets the authentication SQL.
        /// </summary>
        /// <value>
        /// The authentication SQL.
        /// </value>
        private static AuthSqlRepository AuthSql
        {
            get
            {
                return authRepo ?? (authRepo = new AuthSqlRepository(AuthContext.AuthDal));
            }
        }

        /// <summary>
        /// Gets the permissions.
        /// </summary>
        /// <value>
        /// The permissions.
        /// </value>
        public static Dictionary<int, Permission> Permissions
        {
            get
            {
                Dictionary<int, Permission> dictionary = permissions;
                if (dictionary != null)
                {
                    return dictionary;
                }

                IList<Permission> list = AuthSql.GetPermissions();
                Func<Permission, int> keySelector = p => p.PermissionId;
                return permissions = Enumerable.ToDictionary(list, keySelector);
            }
        }

        /// <summary>
        /// Gets the name of the permissions by.
        /// </summary>
        /// <value>
        /// The name of the permissions by.
        /// </value>
        public static Dictionary<string, Permission> PermissionsByName
        {
            get
            {
                Dictionary<string, Permission> dictionary = permissionsByName;
                if (dictionary != null)
                {
                    return dictionary;
                }

                Dictionary<int, Permission>.ValueCollection values = Permissions.Values;
                Func<Permission, string> keySelector = (p => p.Name);
                return permissionsByName = Enumerable.ToDictionary(values, keySelector);
            }
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public static Dictionary<Guid, Resource> Resources
        {
            get
            {
                Dictionary<Guid, Resource> dictionary = resources;
                if (dictionary != null)
                {
                    return dictionary;
                }

                IList<Resource> list = AuthSql.GetResources();
                Func<Resource, Guid> keySelector = (r => r.ResourceId);
                return resources = Enumerable.ToDictionary(list, keySelector);
            }
        }

        /// <summary>
        /// Gets the name of the resources by.
        /// </summary>
        /// <value>
        /// The name of the resources by.
        /// </value>
        public static Dictionary<string, Resource> ResourcesByName
        {
            get
            {
                Dictionary<string, Resource> dictionary = resourcesByName;
                if (dictionary != null)
                {
                    return dictionary;
                }

                Dictionary<Guid, Resource>.ValueCollection values = Resources.Values;
                Func<Resource, string> keySelector = (r => r.Name);
                return resourcesByName = Enumerable.ToDictionary(values, keySelector);
            }
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="AuthorizationException">Invalid Logical Resource Id</exception>
        public static Resource GetResource(Guid id)
        {
            Resource logicalResource;
            if (!Resources.TryGetValue(id, out logicalResource))
            {
                throw new AuthorizationException(string.Format("Invalid Logical Resource Id: {0}", id));
            }

            return logicalResource;
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="AuthorizationException">Invalid Resource Name</exception>
        public static Resource GetResource(string name)
        {
            Resource logicalResource;
            if (!ResourcesByName.TryGetValue(name, out logicalResource))
            {
                throw new AuthorizationException(string.Format("Invalid Resource Name: {0}", name));
            }

            return logicalResource;
        }

        /// <summary>
        /// Gets the permission.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="AuthorizationException">Invalid Permission Id</exception>
        public static Permission GetPermission(int id)
        {
            Permission permission;
            if (!Permissions.TryGetValue(id, out permission))
            {
                throw new AuthorizationException(string.Format("Invalid Permission Id: {0}", id));
            }

            return permission;
        }

        /// <summary>
        /// Gets the permission.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="AuthorizationException">Invalid Permission Name</exception>
        public static Permission GetPermission(string name)
        {
            Permission permission;
            if (!PermissionsByName.TryGetValue(name, out permission))
            {
                throw new AuthorizationException(string.Format("Invalid Permission Name: {0}", name));
            }

            return permission;
        }

        /// <summary>
        /// Checks the authorization.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <param name="securityIds">The security ids.</param>
        /// <param name="permissionId">The permission identifier.</param>
        /// <returns>authorization type</returns>
        public static Models.Enums.AuthorizationType CheckAuthorization(Guid resourceId, IList<string> securityIds, int permissionId)
        {
            return AuthSql.CheckAuthorization(resourceId, securityIds, permissionId);
        }

        /// <summary>
        /// Checks the authorization.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <param name="securityIds">The security ids.</param>
        /// <param name="permissionId">The permission identifier.</param>
        /// <returns></returns>
        public static Models.Enums.AuthorizationType CheckAuthorization(Guid resourceId, IList<Guid> securityIds, int permissionId)
        {
            return AuthSql.CheckAuthorization(resourceId, securityIds, permissionId);
        }
    }
}
