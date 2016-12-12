using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

namespace Orbit.WebApi.Core.Security
{
    /// <summary>
    /// Class to encapsulate the Api principal.
    /// </summary>
    /// <seealso cref="System.Security.Claims.ClaimsPrincipal" />
    public class ApiPrincipal : ClaimsPrincipal
    {
        /// <summary>
        /// The identity
        /// </summary>
        private readonly ApiIdentity identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiPrincipal" /> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public ApiPrincipal(ApiIdentity identity) : base(identity)
        {
            this.identity = identity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiPrincipal" /> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public ApiPrincipal(ClaimsIdentity identity) : base(identity)
        {
            this.identity = new ApiIdentity(identity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiPrincipal" /> class.
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal.</param>
        public ApiPrincipal(ClaimsPrincipal claimsPrincipal) : base(claimsPrincipal)
        {
            if (claimsPrincipal != null)
            {
                identity = new ApiIdentity(claimsPrincipal.Identity);
            }
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        /// <exception cref="System.Security.SecurityException">No current principal</exception>
        public static new ApiPrincipal Current
        {
            get
            {
                ApiPrincipal simplexPrincipal = Thread.CurrentPrincipal as ApiPrincipal;
                if (simplexPrincipal == null)
                {
                    throw new SecurityException("No current principal");
                }

                return simplexPrincipal;
            }
        }

        /// <summary>
        /// Gets the primary claims identity associated with this claims principal.
        /// </summary>
        public override IIdentity Identity
        {
            get
            {
                return identity;
            }
        }

        /// <summary>
        /// Gets the roles.
        /// </summary>
        /// <value>
        /// The roles.
        /// </value>
        public IEnumerable<string> Roles
        {
            get
            {
                return FindValues<string>(ApiIdentity.RolesClaimType);
            }
        }

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId
        {
            get
            {
                return FindFirstValue<long>(ApiIdentity.UserIdClaimType);
            }
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username
        {
            get
            {
                return FindFirstValue<string>(ApiIdentity.UsernameClaimType);
            }
        }

        /// <summary>
        /// Gets the name of this claims identity.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return Username;
            }
        }

        /// <summary>
        /// Gets the security identifier.
        /// </summary>
        /// <value>
        /// The security identifier.
        /// </value>
        public Guid SecurityId
        {
            get
            {
                return FindFirstValue<Guid>(ApiIdentity.SIDClaimType);
            }
        }

        /// <summary>
        /// Gets the user authentication token identifier.
        /// </summary>
        /// <value>
        /// The user authentication token identifier.
        /// </value>
        public string UserAuthTokenId
        {
            get
            {
                return FindFirstValue<string>(ApiIdentity.AuthTokenClaimType);
            }
        }

        /// <summary>
        /// Gets the security ids.
        /// </summary>
        /// <value>
        /// The security ids.
        /// </value>
        public IEnumerable<Guid> SecurityIds
        {
            get
            {
                return FindValues<Guid>(ApiIdentity.SecurityIdsClaimType);
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the entity (user) represented by this claims principal is in the specified role.
        /// </summary>
        /// <param name="role">The role for which to check.</param>
        /// <returns>
        /// true if claims principal is in the specified role; otherwise, false.
        /// </returns>
        public override bool IsInRole(string role)
        {
            return this.Roles.Contains(role);
        }

        /// <summary>
        /// Gets the authentication client.
        /// </summary>
        /// <value>
        /// The authentication client.
        /// </value>
        public string AuthClient
        {
            get
            {
                return FindFirstValue<string>(ApiIdentity.AuthClientClaimType);
            }
        }

        /// <summary>
        /// Finds the first value.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>type</returns>
        private T FindFirstValue<T>(string type)
        {
            return Claims
                .Where(p => p.Type == type)
                .Select(p => (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(p.Value))
                .FirstOrDefault();
        }

        /// <summary>
        /// Finds the values.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>type collection</returns>
        private IEnumerable<T> FindValues<T>(string type)
        {
            return Claims
                .Where(p => p.Type == type)
                .Select(p => (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(p.Value))
                .ToList();
        }
    }
}