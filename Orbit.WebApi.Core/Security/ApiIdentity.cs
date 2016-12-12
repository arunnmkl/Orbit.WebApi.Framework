using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Orbit.WebApi.Core.Security
{
    /// <summary>
    /// Class to encapsulate the Orbit identity.
    /// </summary>
    /// <seealso cref="System.Security.Claims.ClaimsIdentity" />
    /// <seealso cref="ClaimsIdentity" />
    public class ApiIdentity : ClaimsIdentity
    {
        /// <summary>
        /// The roles claim type
        /// </summary>
        public const string RolesClaimType = "http://schemas.dreamorbit.com/Orbit.Security.Role";

        /// <summary>
        /// The user identifier claim type
        /// </summary>
        public const string UserIdClaimType = "http://schemas.dreamorbit.com/Orbit.Security.Id";

        /// <summary>
        /// The security ids claim type
        /// </summary>
        public const string SecurityIdsClaimType = "http://schemas.dreamorbit.com/Orbit.Security.SIDs";

        /// <summary>
        /// The authentication token claim type
        /// </summary>
        public const string AuthTokenClaimType = "http://schemas.dreamorbit.com/Orbit.Security.UserAuthToken";

        /// <summary>
        /// The username claim type
        /// </summary>
        public const string UsernameClaimType = "http://schemas.dreamorbit.com/Orbit.Security.UserName";

        /// <summary>
        /// The sid claim type
        /// </summary>
        public const string SIDClaimType = "http://schemas.dreamorbit.com/Orbit.Security.SID";

        /// <summary>
        /// The authentication client claim type
        /// </summary>
        public const string AuthClientClaimType = "http://schemas.dreamorbit.com/Orbit.Security.AuthClient";

        /// <summary>
        /// The password timestamp claim type
        /// </summary>
        public const string PasswordTimestampClaimType = "http://schemas.dreamorbit.com/Orbit.Security.PasswordTimestamp";

        /// <summary>
        /// The user full name claim type
        /// </summary>
        public const string UserFullNameClaimType = "http://schemas.dreamorbit.com/Orbit.Security.UserFullName";

        /// <summary>
        /// The impersonating user identifier claim type
        /// </summary>
        public const string ImpersonatingUserIdClaimType = "http://schemas.dreamorbit.com/Orbit.Security.ImpersonatingUserId";

        /// <summary>
        /// The user culture claim type
        /// </summary>
        public const string UserCultureClaimType = "http://schemas.dreamorbit.com/Orbit.Security.UserCulture";

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiIdentity" /> class.
        /// </summary>
        /// <param name="claims">The claims with which to populate the claims identity.</param>
        /// <param name="authenticationType">The type of authentication used.</param>
        public ApiIdentity(IEnumerable<Claim> claims, string authenticationType) : base(authenticationType: authenticationType)
        {
            if (claims != null)
            {
                // Username
                AddClaims(from name in claims where name.Type == UsernameClaimType select name);

                // UserId
                AddClaims(from userId in claims where userId.Type == UserIdClaimType select userId);

                // Security Id's
                AddClaims(from sids in claims where sids.Type == SecurityIdsClaimType select sids);

                // SID
                AddClaims(from sid in claims where sid.Type == SIDClaimType select sid);

                // Authentication Token
                AddClaims(from authToken in claims where authToken.Type == AuthTokenClaimType select authToken);

                // Roles
                AddClaims(from role in claims where role.Type == RolesClaimType select role);

                // AuthClient
                AddClaims(from authClient in claims where authClient.Type == AuthClientClaimType select authClient);

                // PasswordTimestamp
                AddClaims(from timeStamp in claims where timeStamp.Type == PasswordTimestampClaimType select timeStamp);

                // UserFullName
                AddClaims(from fullname in claims where fullname.Type == UserFullNameClaimType select fullname);

                // ImpersonatingUserId
                AddClaims(from impersonatingUserId in claims where impersonatingUserId.Type == ImpersonatingUserIdClaimType select impersonatingUserId);

                // UserCulture
                AddClaims(from userCulture in claims where userCulture.Type == UserCultureClaimType select userCulture);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiIdentity" /> class.
        /// </summary>
        /// <param name="identity">The identity from which to base the new claims identity.</param>
        public ApiIdentity(IIdentity identity) : base(identity) { }

        /// <summary>
        /// Gets the name of this claims identity.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name
        {
            get
            {
                if (this.IsAuthenticated)
                {
                    return Convert.ToString(FindFirst(UsernameClaimType).Value);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName
        {
            get
            {
                if (this.IsAuthenticated)
                {
                    return Convert.ToString(FindFirst(UserFullNameClaimType).Value);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the impersonating user identifier.
        /// </summary>
        /// <value>
        /// The impersonating user identifier.
        /// </value>
        public long? ImpersonatingUserId
        {
            get
            {
                if (this.IsAuthenticated)
                {
                    long impId = 0;
                    var value = FindFirst(ImpersonatingUserIdClaimType).Value;
                    return long.TryParse(value, out impId) ? impId : (long?)null;
                }

                return (long?)null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is impersonated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is impersonated; otherwise, <c>false</c>.
        /// </value>
        public bool IsImpersonated
        {
            get
            {
                return this.ImpersonatingUserId.HasValue;
            }
        }

        /// <summary>
        /// Gets the user culture.
        /// </summary>
        /// <value>
        /// The user culture.
        /// </value>
        public string UserCulture
        {
            get
            {
                if (this.IsAuthenticated)
                {
                    return Convert.ToString(FindFirst(UserCultureClaimType).Value);
                }

                return string.Empty;
            }
        }
    }
}