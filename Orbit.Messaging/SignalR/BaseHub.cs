using System.Net.Http.Headers;
using Orbit.Messaging.Security;
using Microsoft.AspNet.SignalR;
namespace Orbit.Messaging.SignalR
{
    /// <summary>
    /// Base object fro SignalR Hub.
    /// </summary>
    /// <seealso cref="Hub" />
    public class BaseHub : Hub
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken
        {
            get
            {
                var auth = Context.Request.Headers["Authorization"];
                if (!string.IsNullOrEmpty(auth))
                {

                    AuthenticationHeaderValue authorization = AuthenticationHeaderValue.Parse(auth);
                    // If there are no authorization token in header, do nothing.
                    if (authorization != null)
                    {
                        // If there are authorization token but the filter does not recognize the 
                        // authentication scheme, do nothing.
                        if (authorization.Scheme.ToUpperInvariant() == Configuration.Current.AuthHeaderSchemaName.ToUpperInvariant())
                        {
                            return authorization.Parameter;
                        }
                    }
                }

                return Context.QueryString[Configuration.Current.AuthQueryStringName];
            }
        }
    }
}