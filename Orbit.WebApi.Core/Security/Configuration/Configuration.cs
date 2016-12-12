using System.Configuration;

namespace Orbit.WebApi.Core.Security
{
    /// <summary>
    /// Class Configuration.
    /// </summary>
    public class Configuration : ConfigurationSection
    {
        #region Private Members

        /// <summary>
        /// Stores the configuration section name
        /// </summary>
        private const string ConfigurationSectionName = "apiSecurity";

        /// <summary>
        /// Stores the enable authentication
        /// </summary>
        private const string EnableAuthentication = "enableAuthentication";

        /// <summary>
        /// Stores the enable basic authentication
        /// </summary>
        private const string EnableBasicAuthentication = "enableBasicAuthentication";

        /// <summary>
        /// Stores the enable o authentication authentication
        /// </summary>
        private const string EnableOAuthAuthentication = "enableOAuthAuthentication";

        /// <summary>
        /// Stores the enable cookie authentication
        /// </summary>
        private const string EnableCookieAuthentication = "enableCookieAuthentication";

        /// <summary>
        /// Stores the enable API key validation
        /// </summary>
        private const string EnableApiKeyValidation = "enableApiKeyValidation";

        /// <summary>
        /// Stores the enable orbit authorization
        /// </summary>
        private const string EnableOrbitAuthorization = "enableOrbitAuthorization";

        /// <summary>
        /// Stores the enable prevents CSRF Attack
        /// </summary>
        private const string PreventCSRFAttack = "preventCSRFAttack";

        /// <summary>
        /// Stores the enable simplex authentication cookie name
        /// </summary>
        private const string CookieNameCSRF = "csrfCookieName";

        /// <summary>
        /// Stores the enable simplex authentication CSRF header name
        /// </summary>
        private const string HeaderNameCSRF = "csrfHeaderName";

        /// <summary>
        /// Stores the path of the Deployment Path where configurations get deployed AuthCookie
        /// </summary>
        private const string Path = "path";

        /// <summary>
        /// Stores the name of the auth cookie name
        /// </summary>
        private const string AuthCookie = "authCookieName";

        /// <summary>
        /// Stores the name of the auth cookie name
        /// </summary>
        private const string Version = "apiVersion";

        /// <summary>
        /// Stores the enable Suppress Exception
        /// </summary>
        private const string SuppressException = "suppressException";

        /// <summary>
        /// The enable database token validation
        /// </summary>
        private const string EnableDBTokenValidation = "enableDBTokenValidation";

        /// <summary>
        /// The enable multiple instance
        /// </summary>
        private const string EnableMultipleInstance = "enableMultipleInstance";

        /// <summary>
        /// The handle un handled exception
        /// </summary>
        private const string HandleUnHandledException = "handleUnHandledException";

        /// <summary>
        /// The password change validation
        /// </summary>
        private const string PasswordChangeValidation = "passwordChangeValidation";

        /// <summary>
        /// The by pass orbit authorization
        /// </summary>
        private const string ByPassOrbitAuthorization = "byPassOrbitAuthorization";

        /// <summary>
        /// Stores the current configuration section for api settings
        /// </summary>
        private static Configuration _current;

        #endregion Private Members

        #region ItSelf

        /// <summary>
        /// Gets the current configuration section.
        /// </summary>
        /// <value>The current.</value>
        public static Configuration Current
        {
            get
            {
                return _current ?? (_current = ConfigurationManager.GetSection(ConfigurationSectionName) as Configuration
                      ?? new Configuration());
            }
        }

        #endregion ItSelf

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [basic authentication enabled].
        /// </summary>
        /// <value><c>true</c> if [basic authentication enabled]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(EnableAuthentication, DefaultValue = false, IsRequired = false, IsKey = false)]
        public bool AuthenticationEnabled
        {
            get { return (bool)this[EnableAuthentication]; }
            set { this[EnableAuthentication] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [basic authentication enabled].
        /// </summary>
        /// <value><c>true</c> if [basic authentication enabled]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(EnableBasicAuthentication, DefaultValue = false, IsRequired = false, IsKey = false)]
        public bool BasicAuthenticationEnabled
        {
            get { return (bool)this[EnableBasicAuthentication]; }
            set { this[EnableBasicAuthentication] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [o authentication authentication enabled].
        /// </summary>
        /// <value><c>true</c> if [o authentication authentication enabled]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(EnableOAuthAuthentication, DefaultValue = false, IsRequired = false, IsKey = false)]
        public bool OAuthAuthenticationEnabled
        {
            get { return (bool)this[EnableOAuthAuthentication]; }
            set { this[EnableOAuthAuthentication] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cookie authentication enabled].
        /// </summary>
        /// <value><c>true</c> if [cookie authentication enabled]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(EnableCookieAuthentication, DefaultValue = false, IsRequired = false, IsKey = false)]
        public bool CookieAuthenticationEnabled
        {
            get { return (bool)this[EnableCookieAuthentication]; }
            set { this[EnableCookieAuthentication] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [API key validation enabled].
        /// </summary>
        /// <value><c>true</c> if [API key validation enabled]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(EnableApiKeyValidation, DefaultValue = false, IsRequired = false, IsKey = false)]
        public bool ApiKeyValidationEnabled
        {
            get { return (bool)this[EnableApiKeyValidation]; }
            set { this[EnableApiKeyValidation] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [orbit authorization enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [orbit authorization enabled]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty(EnableOrbitAuthorization, DefaultValue = false, IsRequired = false, IsKey = false)]
        public bool OrbitAuthorizationEnabled
        {
            get { return (bool)this[EnableOrbitAuthorization]; }
            set { this[EnableOrbitAuthorization] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [simplex authorization enabled].
        /// </summary>
        /// <value><c>true</c> if [simplex authorization enabled]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(PreventCSRFAttack, DefaultValue = false, IsRequired = false, IsKey = false)]
        public bool CSRFAttackPrevented
        {
            get { return (bool)this[PreventCSRFAttack]; }
            set { this[PreventCSRFAttack] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the CSRF cookie.
        /// </summary>
        /// <value>The name of the CSRF cookie.</value>
        [ConfigurationProperty(CookieNameCSRF, DefaultValue = "RVT", IsRequired = false, IsKey = false)]
        public string CSRFCookieName
        {
            get { return (string)this[CookieNameCSRF]; }
            set { this[CookieNameCSRF] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the CSRF header.
        /// </summary>
        /// <value>The name of the CSRF header.</value>
        [ConfigurationProperty(HeaderNameCSRF, DefaultValue = "RVT", IsRequired = false, IsKey = false)]
        public string CSRFHeaderName
        {
            get { return (string)this[HeaderNameCSRF]; }
            set { this[HeaderNameCSRF] = value; }
        }

        /// <summary>
        /// Gets or sets the deployment path.
        /// </summary>
        /// <value>The deployment path where all the components get deployment.</value>
        [ConfigurationProperty(Path, IsRequired = false, IsKey = false)]
        public string DeploymentPath
        {
            get { return (string)this[Path]; }
            set { this[Path] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the authentication cookie. Version
        /// </summary>
        /// <value>The name of the authentication cookie.</value>
        [ConfigurationProperty(AuthCookie, IsRequired = false, DefaultValue = "AuthCookie", IsKey = false)]
        public string AuthCookieName
        {
            get { return (string)this[AuthCookie]; }
            set { this[AuthCookie] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the authentication cookie.
        /// </summary>
        /// <value>The name of the authentication cookie.</value>
        [ConfigurationProperty(Version, IsRequired = false, DefaultValue = "", IsKey = false)]
        public string ApiVersion
        {
            get { return (string)this[Version]; }
            set { this[Version] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [exception suppressed].
        /// </summary>
        /// <value><c>true</c> if [exception suppressed]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(SuppressException, IsRequired = false, DefaultValue = false)]
        public bool ExceptionSuppressed
        {
            get { return (bool)this[SuppressException]; }
            set { this[SuppressException] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [database token validation enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [database token validation enabled]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty(EnableDBTokenValidation, DefaultValue = false, IsRequired = false, IsKey = false)]
        public bool DBTokenValidationEnabled
        {
            get
            {
                return (bool)this[EnableDBTokenValidation];
            }

            set
            {
                this[EnableDBTokenValidation] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [multiple instance enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [multiple instance enabled]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty(EnableMultipleInstance, DefaultValue = true, IsRequired = false, IsKey = false)]
        public bool MultipleInstanceEnabled
        {
            get
            {
                return (bool)this[EnableMultipleInstance];
            }

            set
            {
                this[EnableMultipleInstance] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is handle un handled exception.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is handle un handled exception; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty(HandleUnHandledException, IsRequired = false, DefaultValue = false)]
        public bool IsHandleUnHandledException
        {
            get { return (bool)this[HandleUnHandledException]; }
            set { this[HandleUnHandledException] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [validate password change].
        /// </summary>
        /// <value>
        /// <c>true</c> if [validate password change]; otherwise, <c>false</c>.
        /// </value>  
        [ConfigurationProperty(PasswordChangeValidation, IsRequired = false, DefaultValue = false)]
        public bool ValidatePasswordChange
        {
            get { return (bool)this[PasswordChangeValidation]; }
            set { this[PasswordChangeValidation] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [orbit authorization by passed].
        /// </summary>
        /// <value>
        /// <c>true</c> if [orbit authorization by passed]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty(ByPassOrbitAuthorization, IsRequired = false, DefaultValue = false)]
        public bool OrbitAuthorizationByPassed
        {
            get { return (bool)this[ByPassOrbitAuthorization]; }
            set { this[ByPassOrbitAuthorization] = value; }
        }

        #endregion Properties
    }
}