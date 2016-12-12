using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Orbit.WebApi.Core.Security;

namespace Orbit.Messaging
{
    public class ChatContext
    {
        /// <summary>
        /// Gets the api principal.
        /// </summary>
        /// <value>
        /// The api principal.
        /// </value>
        public static ApiPrincipal ApiPrincipal
        {
            get
            {
                return new ApiPrincipal(Principal);
            }
        }

        /// <summary>
        /// Gets the principal.
        /// </summary>
        /// <value>
        /// The principal.
        /// </value>
        public static ClaimsPrincipal Principal
        {
            get
            {
                return Thread.CurrentPrincipal as ClaimsPrincipal;
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
        /// Gets the roles.
        /// </summary>
        /// <value>
        /// The roles.
        /// </value>
        public static IList<string> Roles
        {
            get
            {
                if (ApiPrincipal != null)
                {
                    return (IList<string>)ApiPrincipal.Roles;
                }

                return null;
            }
        }
    }
}