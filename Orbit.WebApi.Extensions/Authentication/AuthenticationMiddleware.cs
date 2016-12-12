using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Orbit.WebApi.Core.Exceptions;
using Orbit.WebApi.Extensions.Common;

namespace Orbit.WebApi.Extensions.Authentication
{
    /// <summary>
    /// It provides strongly typed access to the OWIN environment via IOwinContext.
    /// </summary>
    /// <seealso cref="OwinMiddleware" />
    public class AuthenticationMiddleware : OwinMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public AuthenticationMiddleware(OwinMiddleware next) : base(next) { }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);

                if (context.Response.StatusCode == 400 && context.Response.Headers.ContainsKey(Constants.OrbitChallengeFlag))
                {
                    var headerValues = context.Response.Headers.GetValues(Constants.OrbitChallengeFlag);
                    context.Response.StatusCode = Convert.ToInt16(headerValues.FirstOrDefault());
                    context.Response.Headers.Remove(Constants.OrbitChallengeFlag);
                }
            }
            catch (Exception ex)
            {
                if (ex is ApiException)
                {
                    var exception = ex as ApiException;
                    context.Response.StatusCode = (int)exception.StatusCode;
                    context.Response.ReasonPhrase = exception.StatusCode.ToString();
                    context.Response.ContentType = "application/json";
                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        error = "invalid_request",
                        error_description = exception.Message
                    }));
                }
                else
                {
                    throw;
                }
            }

        }
    }
}
