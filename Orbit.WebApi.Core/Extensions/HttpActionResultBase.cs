using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Orbit.WebApi.Core
{
    /// <summary>
    /// Class HttpActionResultBase.
    /// </summary>
    public class HttpActionResultBase : IHttpActionResult
    {
        /// <summary>
        /// The _value
        /// </summary>
        private object _value;
        /// <summary>
        /// The _request
        /// </summary>
        private HttpRequestMessage _request;
        /// <summary>
        /// The _status code
        /// </summary>
        private HttpStatusCode _statusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpActionResultBase"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="value">The value.</param>
        /// <param name="statusCode">The status code.</param>
        public HttpActionResultBase(HttpRequestMessage request, object value,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _value = value;
            _request = request;
            _statusCode = statusCode;
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = _request.CreateResponse(_statusCode, _value);

            return Task.FromResult(response);
        }
    }
}
