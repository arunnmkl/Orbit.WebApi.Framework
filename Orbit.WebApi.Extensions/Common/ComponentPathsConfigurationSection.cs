using System.Configuration;

namespace Orbit.WebApi.Extensions.Common
{
    /// <summary>
    /// All the atlas services URLs for external and internal
    /// </summary>
    public class ComponentPathsConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// The configuration name for the settings
        /// </summary>
        public static string ConfigName = "componentPaths";

        /// <summary>
        /// Gets or sets the instances.
        /// </summary>
        /// <value>
        /// The instances.
        /// </value>
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public ComponentPathConfigCollection Instances
        {
            get { return (ComponentPathConfigCollection)this[""]; }
        }
    }

    /// <summary>
    /// This is the collection of all the configuration section
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class ComponentPathConfigCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ComponentPathConfigElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            // set to whatever Element Property you want to use for a key
            return ((ComponentPathConfigElement)element).Name;
        }
    }

    /// <summary>
    /// Make sure to set IsKey=true for property exposed as the GetElementKey above
    /// </summary>
    public class ComponentPathConfigElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the domain.
        /// </summary>
        /// <value>
        /// The name of the domain.
        /// </value>
        [ConfigurationProperty("cookieDomain")]
        public string CookieDomain
        {
            get { return (string)base["cookieDomain"]; }
            set { base["cookieDomain"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the cookie.
        /// </summary>
        /// <value>
        /// The name of the cookie.
        /// </value>
        [ConfigurationProperty("cookieName")]
        public string CookieName
        {
            get { return (string)base["cookieName"]; }
            set { base["cookieName"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [ConfigurationProperty("username")]
        public string UserName
        {
            get { return (string)base["username"]; }
            set { base["username"] = value; }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [ConfigurationProperty("password")]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }
    }
}