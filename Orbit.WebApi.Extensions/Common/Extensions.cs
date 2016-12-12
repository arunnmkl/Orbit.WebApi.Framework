using System.Collections.Generic;
using Microsoft.Owin.Security.OAuth;

namespace Orbit.WebApi.Extensions.Common
{
    /// <summary>
    /// Common extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the value by key.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>additional values appended to the token response for the given key</returns>
        public static object GetValueByKey(this IDictionary<string, object> dictionary, string key)
        {
            object value = null;
            if (!dictionary.TryGetValue(key, out value))
            {
                return value;
            }

            return value;
        }

        /// <summary>
        /// Tries the get parameter values.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>
        /// parameter value(s)
        /// </returns>
        public static IList<string> TryGetParamValues(this OAuthValidateClientAuthenticationContext context, string paramName)
        {
            return context.Parameters.GetValues(paramName);
        }
    }
}
