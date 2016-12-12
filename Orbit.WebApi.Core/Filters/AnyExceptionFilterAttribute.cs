using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Orbit.WebApi.Core.Filters
{
    /// <summary>
    /// Class AnyExceptionFilterAttribute.
    /// </summary>
    public class AnyExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Called when [exception].
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is NotImplementedException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }
        }
    }
}