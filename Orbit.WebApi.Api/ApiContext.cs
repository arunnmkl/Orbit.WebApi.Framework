using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Api
{
    /// <summary>
    /// API context object.
    /// </summary>
    public class ApiContext
    {
        /// <summary>
        /// Gets the Api principal.
        /// </summary>
        /// <value>
        /// The Api principal.
        /// </value>
        public static ApiPrincipal ApiPrincipal
        {
            get
            {
                return Thread.CurrentPrincipal as ApiPrincipal;
            }
        }

        /// <summary>
        /// Gets the security ids.
        /// </summary>
        /// <value>
        /// The security ids.
        /// </value>
        public static IList<Guid> SecurityIds
        {
            get
            {
                if (ApiPrincipal != null)
                {
                    return ApiPrincipal.SecurityIds.ToList();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the security identifier.
        /// </summary>
        /// <value>
        /// The security identifier.
        /// </value>
        public static Guid SecurityId
        {
            get
            {
                if (ApiPrincipal != null)
                {
                    return ApiPrincipal.SecurityId;
                }

                return Guid.Empty;
            }
        }

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public static long UserId
        {
            get
            {
                if (ApiPrincipal != null)
                {
                    return ApiPrincipal.UserId;
                }

                return default(long);
            }
        }

        /// <summary>
        /// Gets the user authentication token identifier.
        /// </summary>
        /// <value>
        /// The user authentication token identifier.
        /// </value>
        public static string UserAuthTokenId
        {
            get
            {
                if (ApiPrincipal != null)
                {
                    return ApiPrincipal.UserAuthTokenId;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public static string Username
        {
            get
            {
                if (ApiPrincipal != null)
                {
                    return ApiPrincipal.Username;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is authenticated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
        /// </value>
        public static bool IsAuthenticated
        {
            get
            {
                if (ApiPrincipal != null)
                {
                    return ApiPrincipal.Identity.IsAuthenticated;
                }

                return default(bool);
            }
        }

        /// <summary>
        /// Gets my custom property.
        /// </summary>
        /// <value>
        /// My custom property.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public static object MyCustomProperty
        {
            get
            {
                // use the UserId / Username / SecurityId to get you own object and assigned it here.

                throw new System.NotImplementedException();
            }
        }
    }
}