using Orbit.WebApi.Core.Enums;
using Orbit.WebApi.Core.Results;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Orbit.WebApi.Core.Extensions
{
    /// <summary>
    /// This class is used to cache the objects as API does not cache any of the calls.
    /// </summary>
    public static class HttpActionResultExtensions
    {
        /// <summary>
        /// Cached the specified cache ability.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionResult">The action result.</param>
        /// <param name="cacheability">The cache ability.</param>
        /// <param name="eTag">The e tag.</param>
        /// <param name="expires">The expires.</param>
        /// <param name="lastModified">The last modified.</param>
        /// <param name="maxAge">The maximum age.</param>
        /// <param name="noStore">The no store.</param>
        /// <returns></returns>
        public static CachedResult<T> Cached<T>(
            this T actionResult,
            Cacheability cacheability = Cacheability.Private,
            string eTag = null,
            DateTimeOffset? expires = null,
            DateTimeOffset? lastModified = null,
            TimeSpan? maxAge = null,
            bool? noStore = null) where T : IHttpActionResult
        {
            return new CachedResult<T>(actionResult, cacheability, eTag, expires, lastModified, maxAge, noStore);
        }

        /// <summary>
        /// Caches the response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionResult">The action result.</param>
        /// <param name="cacheability">The cache ability.</param>
        /// <param name="eTag">The e tag.</param>
        /// <param name="expires">The expires.</param>
        /// <param name="lastModified">The last modified.</param>
        /// <param name="maxAge">The maximum age.</param>
        /// <param name="noStore">The no store.</param>
        /// <returns></returns>
        public static T CacheResponse<T>(
            this T actionResult,
            Cacheability cacheability = Cacheability.Private,
            string eTag = null,
            DateTimeOffset? expires = null,
            DateTimeOffset? lastModified = null,
            TimeSpan? maxAge = null,
            bool? noStore = null) where T : HttpResponseMessage
        {
            return new CachedResult<T>(actionResult, cacheability, eTag, expires, lastModified, maxAge, noStore).InnerResult;
        }
    }
}