using System.Configuration;

namespace Orbit.Messaging.Security
{
    /// <summary>
    /// Class to encapsulate the configuration.
    /// </summary>
    /// <seealso cref="System.Configuration.ConfigurationSection" />
    public class Configuration : ConfigurationSection
    {
        /// <summary>
        /// The configuration section name
        /// </summary>
        private const string ConfigurationSectionName = "messagingSecurity";

        /// <summary>
        /// The authentication cookie
        /// </summary>
        private const string AuthCookie = "authCookieName";

        /// <summary>
        /// The authentication query string
        /// </summary>
        private const string AuthQueryString = "authQueryStringName";

        /// <summary>
        /// The authentication header schema
        /// </summary>
        private const string AuthHeaderSchema = "authHeaderSchemaName";

        /// <summary>
        /// The path match
        /// </summary>
        private const string PathMatch = "pathMatchValue";

        /// <summary>
        /// The API URL
        /// </summary>
        private const string ApiUrl = "apiUrl";

        /// <summary>
        /// The current
        /// </summary>
        private static Configuration current;

        #region ItSelf

        /// <summary>
        /// Gets the current configuration section.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static Configuration Current
        {
            get
            {
                return current ?? (current = ConfigurationManager.GetSection(ConfigurationSectionName) as Configuration
                      ?? new Configuration());
            }
        }

        #endregion ItSelf

        /// <summary>
        /// Gets or sets the name of the authentication cookie.
        /// </summary>
        /// <value>
        /// The name of the authentication cookie.
        /// </value>
        [ConfigurationProperty(AuthCookie, IsRequired = false, DefaultValue = "AuthCookie", IsKey = false)]
        public string AuthCookieName
        {
            get
            {
                return (string)this[AuthCookie];
            }
            set
            {
                this[AuthCookie] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the authentication query string.
        /// </summary>
        /// <value>
        /// The name of the authentication query string.
        /// </value>
        [ConfigurationProperty(AuthQueryString, IsRequired = false, DefaultValue = "access_token", IsKey = false)]
        public string AuthQueryStringName
        {
            get
            {
                return (string)this[AuthQueryString];
            }
            set
            {
                this[AuthQueryString] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the authentication header schema.
        /// </summary>
        /// <value>
        /// The name of the authentication header schema.
        /// </value>
        [ConfigurationProperty(AuthHeaderSchema, IsRequired = false, DefaultValue = "bearer", IsKey = false)]
        public string AuthHeaderSchemaName
        {
            get
            {
                return (string)this[AuthHeaderSchema];
            }
            set
            {
                this[AuthHeaderSchema] = value;
            }
        }

        /// <summary>
        /// Gets or sets the path match value.
        /// </summary>
        /// <value>
        /// The path match value.
        /// </value>
        [ConfigurationProperty(PathMatch, IsRequired = false, DefaultValue = @"/signalr", IsKey = false)]
        public string PathMatchValue
        {
            get
            {
                return (string)this[PathMatch];
            }
            set
            {
                this[PathMatch] = value;
            }
        }

        /// <summary>
        /// Gets or sets the API URI.
        /// </summary>
        /// <value>
        /// The API URI.
        /// </value>
        [ConfigurationProperty(ApiUrl, IsRequired = false, DefaultValue = @"", IsKey = false)]
        public string ApiUri
        {
            get
            {
                return (string)this[ApiUrl];
            }
            set
            {
                this[ApiUrl] = value;
            }
        }
    }
}