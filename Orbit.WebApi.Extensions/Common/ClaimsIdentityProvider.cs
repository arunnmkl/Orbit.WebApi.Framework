using System;
using System.Collections.Generic;
using System.Security.Claims;
using Orbit.WebApi.Core.Security;
using Orbit.WebApi.Security.Models;

namespace Orbit.WebApi.Extensions.Common
{
    /// <summary>
    /// Claims identity provider.
    /// </summary>
    public class ClaimsIdentityProvider
    {
        /// <summary>
        /// Gets the API claims identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <param name="authenticationType">Type of the authentication.</param>
        /// <returns>
        /// api identity
        /// </returns>
        public static ApiIdentity GetApiClaimsIdentity(UserIdentity userIdentity, string authenticationType)
        {
            return GetApiClaimsIdentity(userIdentity, authenticationType, null); ;
        }

        /// <summary>
        /// Gets the API claims identity.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <param name="authenticationType">Type of the authentication.</param>
        /// <param name="impersonatingUserId">The impersonating user identifier.</param>
        /// <returns>
        /// api identity
        /// </returns>
        public static ApiIdentity GetApiClaimsIdentity(UserIdentity userIdentity, string authenticationType, long? impersonatingUserId = null)
        {
            ApiIdentity identity = new ApiIdentity(ConvertToClaims(userIdentity, impersonatingUserId), authenticationType);
            return identity;
        }

        /// <summary>
        /// Converts to claims.
        /// </summary>
        /// <param name="userIdentity">The user identity.</param>
        /// <param name="impersonatingUserId">The impersonating user identifier.</param>
        /// <returns>
        /// claim collection
        /// </returns>
        private static IEnumerable<Claim> ConvertToClaims(UserIdentity userIdentity, long? impersonatingUserId = null)
        {
            IList<Claim> claims = new List<Claim>();
            // Username
            claims.Add(new Claim(ApiIdentity.UsernameClaimType, userIdentity.Username));

            // UserId
            claims.Add(new Claim(ApiIdentity.UserIdClaimType, userIdentity.UserId.ToString()));

            // SID
            claims.Add(new Claim(ApiIdentity.SIDClaimType, userIdentity.SecurityId.ToString()));

            // Authentication Token
            if (string.IsNullOrEmpty(userIdentity.UserAuthTokenId) == false)
            {
                claims.Add(new Claim(ApiIdentity.AuthTokenClaimType, userIdentity.UserAuthTokenId));
            }

            if (userIdentity.Roles != null)
            {
                foreach (var role in userIdentity.Roles)
                {
                    // Roles
                    claims.Add(new Claim(ApiIdentity.RolesClaimType, role.Name));

                    // Security Id's
                    claims.Add(new Claim(ApiIdentity.SecurityIdsClaimType, role.SecurityId.ToString()));
                }
            }

            // PasswordTimestamp
            claims.Add(new Claim(ApiIdentity.PasswordTimestampClaimType, userIdentity.PasswordTimestamp.ToString()));

            // UserFullName
            claims.Add(new Claim(ApiIdentity.UserFullNameClaimType, userIdentity.FullName ?? string.Empty));

            // ImpersonatingUserId
            claims.Add(new Claim(ApiIdentity.ImpersonatingUserIdClaimType, Convert.ToString(impersonatingUserId)));

            // UserCulture
            claims.Add(new Claim(ApiIdentity.UserCultureClaimType, userIdentity.UserCulture ?? string.Empty));

            return claims;
        }
    }
}
