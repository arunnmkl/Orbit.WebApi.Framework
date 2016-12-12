using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using Orbit.WebApi.Api.Models;

namespace Orbit.WebApi.Api.Common
{
    /// <summary>
    /// This class is to handle and log exception to database
    /// </summary>
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// This method is used to log exception to database and return a generic error message 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {
            HttpContext ctx = HttpContext.Current;

            var message = ctx.Server.GetLastError() != null ? ctx.Server.GetLastError().Message.ToString(CultureInfo.InvariantCulture) : "";
            var source = ctx.Server.GetLastError() != null ? ctx.Server.GetLastError().Source.ToString(CultureInfo.InvariantCulture) : "";
            var StackTrace = ctx.Server.GetLastError() != null ? ctx.Server.GetLastError().Message.ToString(CultureInfo.InvariantCulture) : "";

            var errorLog = new ErrorLog
            {
                UserName = ctx.User.Identity.Name,
                ApplicationUrl = ctx.Request.Url + Environment.NewLine,
                Message = context.Exception.Message.ToString(CultureInfo.InvariantCulture),
                Source = (context.Exception).Source.ToString(CultureInfo.InvariantCulture),
                StackTrace = (context.Exception).StackTrace.ToString(CultureInfo.InvariantCulture)
            };

            // TODO:: Do a database log

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError
                , new { Message = string.Format("Opps! something went wrong, please try again, {0}.", errorLog.Message) }
                , context.ActionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }
    }
}