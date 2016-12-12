using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;

namespace Orbit.WebApi
{
    /// <summary>
    /// Class ApiControllerBase.
    /// </summary>
    public class ApiControllerBase : ApiController
    {
        /// <summary>
        /// Creates the HTTP response message.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="returnValue">The return value.</param>
        /// <returns>HttpResponseMessage.</returns>
        protected HttpResponseMessage CreateHttpResponseMessage(HttpStatusCode httpStatusCode, object returnValue)
        {
            HttpResponseMessage response = Request.CreateResponse(httpStatusCode, returnValue);

            return response;
        }

        /// <summary>
        /// Gets the HTTP response message.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="principal">The principal.</param>
        /// <returns>HttpResponseMessage.</returns>
        protected HttpResponseMessage GetHttpResponseMessage(HttpStatusCode httpStatusCode, IPrincipal principal)
        {
            HttpResponseMessage response = new HttpResponseMessage(httpStatusCode);

            return response;
        }
    }
}