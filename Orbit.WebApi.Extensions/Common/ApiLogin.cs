namespace Orbit.WebApi.Extensions.Common
{
    /// <summary>
    /// Class Login object.
    /// </summary>
    public class ApiLogin
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [remember me].
        /// </summary>
        /// <value><c>true</c> if [remember me]; otherwise, <c>false</c>.</value>
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [force login].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [force login]; otherwise, <c>false</c>.
        /// </value>
        public bool ForceLogin { get; set; }
    }
}
