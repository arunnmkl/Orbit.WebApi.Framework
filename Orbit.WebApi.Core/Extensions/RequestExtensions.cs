using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Core
{
    /// <summary>
    /// This class provides extension methods used by Http request messages.
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="result">The result.</param>
        /// <param name="statusCode">The status code.</param>
        public static void GetResponse(this HttpRequestMessage request, object result, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
        }

        /// <summary>
        /// Returns a dictionary of QueryStrings that's easier to work with than GetQueryNameValuePairs KevValuePairs collection.
        /// If you need to pull a few single values use GetQueryString instead.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> GetQueryStrings(this HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs()
                          .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns an individual query string value
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public static string GetQueryString(this HttpRequestMessage request, string key)
        {
            // IEnumerable<KeyValuePair<string,string>> - right!
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null)
                return null;

            var match = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, key, true) == 0);
            if (string.IsNullOrEmpty(match.Value))
                return null;

            return match.Value;
        }

        /// <summary>
        /// Returns an individual HTTP Header value
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public static string GetHeader(this HttpRequestMessage request, string key)
        {
            IEnumerable<string> keys = null;
            if (!request.Headers.TryGetValues(key, out keys))
                return null;

            return keys.First();
        }

        /// <summary>
        /// Retrieves an individual cookie from the cookies collection
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <returns>System.String.</returns>
        public static string GetCookie(this HttpRequestMessage request, string cookieName)
        {
            CookieHeaderValue cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
            if (cookie != null)
                return cookie[cookieName].Value;

            return null;
        }

        /// <summary>
        /// Adds the cookie.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <param name="value">The value.</param>
        public static void AddCookie(this HttpRequestMessage request, string cookieName, string value)
        {
            request.Headers.Add("Cookie", string.Format("{0}={1};", cookieName, value));
        }

        /// <summary>
        /// Sets the authentication.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="secureTicket">The secure ticket.</param>
        public static void SetAuthentication(this HttpRequestMessage request, string secureTicket)
        {
            request.AddCookie(Configuration.Current.AuthCookieName, secureTicket);
        }

        /// <summary>
        /// Sets the authentication.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <param name="secureTicket">The secure ticket.</param>
        public static void SetAuthentication(this HttpRequestMessage request, string cookieName, string secureTicket)
        {
            request.AddCookie(string.IsNullOrEmpty(cookieName) ? Configuration.Current.AuthCookieName : cookieName, secureTicket);
        }

        /// <summary>
        /// Creates the HTTP response message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="returnValue">The return value.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <returns>HttpActionResultBase.</returns>
        public static HttpActionResultBase CreateHttpResponseMessage(this HttpRequestMessage request, object returnValue, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            return new HttpActionResultBase(request, returnValue, httpStatusCode);
        }
    }
}