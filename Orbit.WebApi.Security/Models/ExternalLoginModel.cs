using System.ComponentModel.DataAnnotations;

namespace Orbit.WebApi.Security.Models
{
    /// <summary>
    /// Parsed external access token
    /// </summary>
    public class ParsedExternalAccessToken
    {
        /// <summary>
        /// Gets or sets the user_id.
        /// </summary>
        /// <value>
        /// The user_id.
        /// </value>
        public string user_id { get; set; }

        /// <summary>
        /// Gets or sets the app_id.
        /// </summary>
        /// <value>
        /// The app_id.
        /// </value>
        public string app_id { get; set; }
    }

    /// <summary>
    /// Register external binding model
    /// </summary>
    public class RegisterExternalBindingModel
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        [Required]
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the external access token.
        /// </summary>
        /// <value>
        /// The external access token.
        /// </value>
        [Required]
        public string ExternalAccessToken { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
