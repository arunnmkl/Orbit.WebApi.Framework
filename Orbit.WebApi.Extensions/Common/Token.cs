using Newtonsoft.Json;

namespace Orbit.WebApi.Extensions.Common
{
    /// <summary>
    /// Class to encapsulates the token object.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets the expires.
        /// </summary>
        /// <value>
        /// The expires.
        /// </value>
        [JsonProperty(".expires")]
        public virtual string Expires { get; set; }

        /// <summary>
        /// Gets or sets the issued.
        /// </summary>
        /// <value>
        /// The issued.
        /// </value>
        [JsonProperty(".issued")]
        public virtual string Issued { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        [JsonProperty("access_token")]
        public virtual string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        [JsonProperty("as:client_id")]
        public virtual string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the expires in.
        /// </summary>
        /// <value>
        /// The expires in.
        /// </value>
        [JsonProperty("expires_in")]
        public virtual int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        [JsonProperty("refresh_token")]
        public virtual string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        [JsonProperty("token_type")]
        public virtual string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [JsonProperty("userName")]
        public virtual string Username { get; set; }
    }
}
