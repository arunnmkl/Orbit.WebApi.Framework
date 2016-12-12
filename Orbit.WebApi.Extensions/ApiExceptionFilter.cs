using System.Net.Http;
using System.Web.Http.Filters;
using Orbit.WebApi.Core.Exceptions;

namespace Orbit.WebApi.Extensions
{
    /// <summary>
    /// Api exception filter attribute
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ExceptionFilterAttribute" />
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Called when [exception].
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception as ApiException;
            if (exception != null)
            {
                context.Response = context.Request.CreateErrorResponse(exception.StatusCode, exception.Message);
            }
        }
    }
}
