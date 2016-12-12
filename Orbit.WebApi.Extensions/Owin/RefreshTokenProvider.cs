using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using Orbit.WebApi.Security;
using Orbit.WebApi.Security.Models;

namespace Orbit.WebApi.Extensions.Owin
{
    /// <summary>
    /// Used in the refresh the token and all.
    /// </summary>
    /// <seealso cref="Microsoft.Owin.Security.Infrastructure.IAuthenticationTokenProvider" />
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            // Get it from database or any other source.
            var refreshTokenId = Guid.NewGuid().ToString("n");

            using (UserManager um = new UserManager())
            {
                var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");
                var userAuthToken = context.OwinContext.Get<string>("as:UserAuthToken");

                var token = new RefreshToken()
                {
                    RefreshTokenId = Helper.GetHash(refreshTokenId),
                    AuthClientId = clientid,
                    Username = context.Ticket.Identity.Name,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime)),
                    UserAuthTokenId = userAuthToken
                };

                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

                token.ProtectedTicket = context.SerializeTicket();

                var result = await Task.Run(() =>
                {
                    return um.AddRefreshToken(token);
                });

                if (result)
                {
                    context.SetToken(refreshTokenId);
                }
            }
        }

        /// <summary>
        /// Receives the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = Helper.GetHash(context.Token);

            using (UserManager um = new UserManager())
            {
                var refreshToken = await Task.Run(() =>
                {
                    return um.FindRefreshToken(hashedTokenId);
                });

                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    if (context.OwinContext.Get<string>("as:UserAuthToken") == null)
                    {
                        context.OwinContext.Set<string>("as:UserAuthToken", refreshToken.UserAuthTokenId);
                    }

                    var result = await Task.Run(() => { return um.RemoveRefreshToken(hashedTokenId); });
                }
            }
        }

        /// <summary>
        /// Creates the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Receives the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}