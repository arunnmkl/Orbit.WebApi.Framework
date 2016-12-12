using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;

namespace Orbit.WebApi.Extensions.Common
{
    /// <summary>
    /// Atlas URL Configurations
    /// </summary>
    public static class ComponentPaths
    {
        /// <summary>
        /// Stores all the values given in the config file.
        /// </summary>
        private static IEnumerable<ComponentPathConfigElement> _values;

        /// <summary>
        /// Gets the values of all the components configure in the config.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        private static IEnumerable<ComponentPathConfigElement> Values
        {
            get
            {
                if (_values == null)
                {
                    var config = ConfigurationManager.GetSection(ComponentPathsConfigurationSection.ConfigName) as ComponentPathsConfigurationSection;

                    if (config != null)
                    {
                        _values = config.Instances.Cast<ComponentPathConfigElement>();
                    }
                }

                return _values;
            }
        }

        /// <summary>
        /// Gets the value by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// This will be the URL of the given key
        /// </returns>
        public static string GetComponentPathByKey(string key)
        {
            var value = Values.FirstOrDefault(item => item.Name.Equals(key));

            if (value != null)
            {
                return value.Url;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the cookie container by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="currentCookieName">Name of the current cookie.</param>
        /// <returns>
        /// The cookie container with settings.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// currentCookieName
        /// or
        /// Http Context Current is null
        /// or
        /// </exception>
        public static CookieContainer GetCookieContainerByKey(string key, string currentCookieName)
        {
            if (string.IsNullOrEmpty(currentCookieName))
            {
                throw new ArgumentNullException("currentCookieName");
            }

            if (HttpContext.Current == null)
            {
                throw new ArgumentNullException("Http Context Current is null");
            }

            var value = Values.FirstOrDefault(item => item.Name.Equals(key));

            if (value == null)
            {
                throw new ArgumentNullException(string.Format("There is no entry for this key: {0}", key));
            }

            if (!string.IsNullOrEmpty(value.UserName) && !string.IsNullOrEmpty(value.Password))
            {
                return GetCookieValueByConfigValues(currentCookieName, value);
            }
            else
            {
                Cookie cookie = GetCookie(value, currentCookieName);

                if (cookie != null)
                {
                    CookieContainer cookieCollection = new CookieContainer();
                    cookieCollection.Add(cookie);
                    return cookieCollection;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the cookie container by key and credentials.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="currentCookieName">Name of the current cookie.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The cookie container with one cookie.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// currentCookieName
        /// or
        /// Credentials
        /// or
        /// Http Context Current is null
        /// or
        /// </exception>
        public static CookieContainer GetCookieContainerByKeyAndCredentials(string key, string currentCookieName, string userName, string password)
        {
            if (string.IsNullOrEmpty(currentCookieName))
            {
                throw new ArgumentNullException("currentCookieName");
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Credentials");
            }

            if (HttpContext.Current == null)
            {
                throw new ArgumentNullException("Http Context Current is null");
            }

            var value = Values.FirstOrDefault(item => item.Name.Equals(key));

            if (value == null)
            {
                throw new ArgumentNullException(string.Format("There is no entry for this key: {0}", key));
            }

            return GetCookieValueByConfigValues(currentCookieName, value, userName, password);
        }

        /// <summary>
        /// Gets the cookie value by configuration values.
        /// </summary>
        /// <param name="currentCookieName">Name of the current cookie.</param>
        /// <param name="value">The value.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The cookie container which have once cookie
        /// </returns>
        private static CookieContainer GetCookieValueByConfigValues(string currentCookieName, ComponentPathConfigElement value, string userName = null, string password = null)
        {
            userName = userName ?? value.UserName;
            password = password ?? value.Password;
            IPrincipal principalObject = null;

            // This can be the tickets or any thing we want
            string ticket = principalObject.Identity.Name;

            Cookie cookie = GetCookie(value, currentCookieName);

            if (cookie != null)
            {
                cookie.Value = ticket;

                CookieContainer cookieCollection = new CookieContainer();
                cookieCollection.Add(cookie);
                return cookieCollection;
            }

            return null;
        }

        /// <summary>
        /// Gets the cookie.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="currentCookieName">Name of the current cookie.</param>
        /// <returns>
        /// The cookie with the same domain
        /// </returns>
        private static Cookie GetCookie(ComponentPathConfigElement value, string currentCookieName)
        {
            HttpRequest request = HttpContext.Current.Request;
            HttpCookie authCookie = request.Cookies[currentCookieName];

            if (authCookie != null)
            {
                Cookie cookie = new Cookie();

                cookie.Domain = string.IsNullOrEmpty(value.CookieDomain) ? request.Url.Host : value.CookieDomain;
                cookie.Expires = authCookie.Expires;
                cookie.Name = string.IsNullOrEmpty(value.CookieName) ? authCookie.Name : value.CookieName;
                cookie.Path = authCookie.Path;
                cookie.Secure = authCookie.Secure;
                cookie.Value = authCookie.Value;

                return cookie;
            }

            return null;
        }
    }
}