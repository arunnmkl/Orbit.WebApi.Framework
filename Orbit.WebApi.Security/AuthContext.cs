using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Orbit.WebApi.Base.SqlSerializer;
using Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Security
{
    /// <summary>
    /// Authentication context.
    /// </summary>
    public class AuthContext
    {
        /// <summary>
        /// The authentication dal
        /// </summary>
        [ThreadStatic]
        private static SqlSerializer authDal;

        /// <summary>
        /// Gets the security schema.
        /// </summary>
        /// <value>
        /// The security schema.
        /// </value>
        public static string SecuritySchema
        {
            get
            {
                return ConfigurationManager.AppSettings["SecuritySchema"] ?? "[dbo]";
            }
        }

        /// <summary>
        /// Gets the chat schema.
        /// </summary>
        /// <value>
        /// The chat schema.
        /// </value>
        public static string ChatSchema
        {
            get
            {
                return ConfigurationManager.AppSettings["ChatSchema"] ?? "[chat]";
            }
        }

        /// <summary>
        /// Gets or sets the authentication dal.
        /// </summary>
        /// <value>
        /// The authentication dal.
        /// </value>
        public static SqlSerializer AuthDal
        {
            get
            {
                return authDal ?? SqlSerializer.ByName("AuthContext");
            }

            set
            {
                authDal = value;
            }
        }

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
        /// Gets the API identity.
        /// </summary>
        /// <value>
        /// The API identity.
        /// </value>
        public static ApiIdentity ApiIdentity
        {
            get
            {
                if (ApiPrincipal != null)
                {
                    return ApiPrincipal.Identity as ApiIdentity;
                }

                return null;
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
                    if (ApiPrincipal.Current.Identity != null)
                    {
                        return ApiPrincipal.Current.Identity.IsAuthenticated;
                    }
                }

                return default(bool);
            }
        }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public static string ClientId
        {
            get
            {
                if (ApiPrincipal != null)
                {
                    return ApiPrincipal.AuthClient;
                }

                return string.Empty;
            }
        }
    }
}
