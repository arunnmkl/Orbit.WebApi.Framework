using Orbit.WebApi.Core.Enums;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orbit.WebApi.Core.Results
{
    /// <summary>
    /// This class make sure that of caching the service of the requests.
    /// This is can be used for two classes IHttpActionResult, HttpResponseMessage till now.
    /// </summary>
    /// <typeparam name="T">type is IHttpActionResult, HttpResponseMessage</typeparam>
    /// <seealso cref="System.Web.Http.IHttpActionResult" />
    public class CachedResult<T> : IHttpActionResult
    {
        /// <summary>
        /// Gets the cache ability.
        /// </summary>
        /// <value>
        /// The cache ability.
        /// </value>
        public Cacheability Cacheability { get; private set; }

        /// <summary>
        /// Gets the e tag.
        /// </summary>
        /// <value>
        /// The e tag.
        /// </value>
        public string ETag { get; private set; }

        /// <summary>
        /// Gets the expires.
        /// </summary>
        /// <value>
        /// The expires.
        /// </value>
        public DateTimeOffset? Expires { get; private set; }

        /// <summary>
        /// Gets the inner result.
        /// </summary>
        /// <value>
        /// The inner result.
        /// </value>
        public T InnerResult { get; private set; }

        /// <summary>
        /// Gets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
        public DateTimeOffset? LastModified { get; private set; }

        /// <summary>
        /// Gets the maximum age.
        /// </summary>
        /// <value>
        /// The maximum age.
        /// </value>
        public TimeSpan? MaxAge { get; private set; }

        /// <summary>
        /// Gets the no store.
        /// </summary>
        /// <value>
        /// The no store.
        /// </value>
        public bool? NoStore { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedResult{T}"/> class.
        /// </summary>
        /// <param name="innerResult">The inner result.</param>
        /// <param name="cacheability">The Cache ability.</param>
        /// <param name="eTag">The e tag.</param>
        /// <param name="expires">The expires.</param>
        /// <param name="lastModified">The last modified.</param>
        /// <param name="maxAge">The maximum age.</param>
        /// <param name="noStore">The no store.</param>
        public CachedResult(
            T innerResult,
            Cacheability cacheability,
            string eTag,
            DateTimeOffset? expires,
            DateTimeOffset? lastModified,
            TimeSpan? maxAge,
            bool? noStore)
        {
            Cacheability = cacheability;
            ETag = eTag;
            Expires = expires;
            InnerResult = innerResult;
            LastModified = lastModified;
            MaxAge = maxAge;
            NoStore = noStore;

            HttpResponseMessage response = InnerResult as HttpResponseMessage;
            if (response != null) CreateResponse(response);
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.
        /// </returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            IHttpActionResult actionResult = InnerResult as IHttpActionResult;
            if (actionResult != null) response = await actionResult.ExecuteAsync(cancellationToken);
            if (response == null) return response;

            return CreateResponse(response);
        }

        /// <summary>
        /// Creates the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The response message</returns>
        private HttpResponseMessage CreateResponse(HttpResponseMessage response)
        {
            if (!response.Headers.Date.HasValue)
                response.Headers.Date = DateTimeOffset.UtcNow;

            response.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = Cacheability == Cacheability.NoCache,
                Private = Cacheability == Cacheability.Private,
                Public = Cacheability == Cacheability.Public
            };

            if (response.Headers.CacheControl.NoCache)
            {
                response.Headers.Pragma.TryParseAdd("no-cache");
                return response;
            }

            response.Content.Headers.Expires = Expires;
            response.Content.Headers.LastModified = LastModified;
            response.Headers.CacheControl.MaxAge = MaxAge;

            if (!String.IsNullOrWhiteSpace(ETag))
                response.Headers.ETag = new EntityTagHeaderValue(String.Format("\"{0}\"", ETag));

            if (NoStore.HasValue)
                response.Headers.CacheControl.NoStore = NoStore.Value;

            return response;
        }
    }
}