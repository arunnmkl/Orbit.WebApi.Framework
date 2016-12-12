using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbit.WebApi.Security.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthClient
    {
        /// <summary>
        /// Gets or sets the authentication client identifier.
        /// </summary>
        /// <value>
        /// The authentication client identifier.
        /// </value>
        [Key]
        public string AuthClientId { get; set; }

        /// <summary>
        /// Gets or sets the secret.
        /// </summary>
        /// <value>
        /// The secret.
        /// </value>
        [Required]
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the application.
        /// </summary>
        /// <value>
        /// The type of the application.
        /// </value>
        public ApplicationTypes ApplicationType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the refresh token life time.
        /// </summary>
        /// <value>
        /// The refresh token life time.
        /// </value>
        public int RefreshTokenLifeTime { get; set; }

        /// <summary>
        /// Gets or sets the allowed origin.
        /// </summary>
        /// <value>
        /// The allowed origin.
        /// </value>
        [MaxLength(100)]
        public string AllowedOrigin { get; set; }

        /// <summary>
        /// Gets or sets the access token expire time span.
        /// </summary>
        /// <value>
        /// The access token expire time span.
        /// </value>
        public int? AccessTokenExpireTimeSpan { get; set; }
    }
}

namespace Orbit.WebApi.Security.Models
{
    /// <summary>
    /// 
    /// </summary>
    public enum ApplicationTypes
    {
        /// <summary>
        /// The java script
        /// </summary>
        JavaScript = 0,
        /// <summary>
        /// The native confidential
        /// </summary>
        NativeConfidential = 1
    }
}