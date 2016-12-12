using System;
using System.ComponentModel.DataAnnotations;

namespace Orbit.WebApi.Security.Models
{
    /// <summary>
    /// Refresh token
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Gets or sets the refresh token identifier.
        /// </summary>
        /// <value>
        /// The refresh token identifier.
        /// </value>
        [Key]
        public string RefreshTokenId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the authentication client identifier.
        /// </summary>
        /// <value>
        /// The authentication client identifier.
        /// </value>
        [Required]
        [MaxLength(128)]
        public string AuthClientId { get; set; }

        /// <summary>
        /// Gets or sets the issued UTC.
        /// </summary>
        /// <value>
        /// The issued UTC.
        /// </value>
        public DateTimeOffset IssuedUtc { get; set; }

        /// <summary>
        /// Gets or sets the expires UTC.
        /// </summary>
        /// <value>
        /// The expires UTC.
        /// </value>
        public DateTimeOffset ExpiresUtc { get; set; }

        /// <summary>
        /// Gets or sets the protected ticket.
        /// </summary>
        /// <value>
        /// The protected ticket.
        /// </value>
        [Required]
        public string ProtectedTicket { get; set; }

        /// <summary>
        /// Gets or sets the user authentication token identifier.
        /// </summary>
        /// <value>
        /// The user authentication token identifier.
        /// </value>
        public string UserAuthTokenId { get; set; }
    }
}
