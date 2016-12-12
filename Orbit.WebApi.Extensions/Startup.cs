using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Orbit.WebApi.Extensions.Authentication;
using Orbit.WebApi.Extensions.Owin;
using Orbit.WebApi.Extensions.Owin.Externals;
using Owin;

[assembly: OwinStartup(typeof(Orbit.WebApi.Extensions.Startup))]

namespace Orbit.WebApi.Extensions
{
    /// <summary>
    /// Startups of the for the
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private static HttpConfiguration config;

        /// <summary>
        /// Gets the o authentication bearer options.
        /// </summary>
        /// <value>
        /// The o authentication bearer options.
        /// </value>
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        /// <summary>
        /// Gets the o authentication server options.
        /// </summary>
        /// <value>
        /// The o authentication server options.
        /// </value>
        public static OAuthAuthorizationServerOptions OAuthServerOptions { get; private set; }

        /// <summary>
        /// Gets the Google authentication options.
        /// </summary>
        /// <value>
        /// The Google authentication options.
        /// </value>
        public static GoogleOAuth2AuthenticationOptions GoogleAuthOptions { get; private set; }

        /// <summary>
        /// Gets the facebook authentication options.
        /// </summary>
        /// <value>
        /// The facebook authentication options.
        /// </value>
        public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public static HttpConfiguration Config
        {
            get
            {
                if (config == null)
                {
                    config = new HttpConfiguration();
                }

                return config;
            }
        }

        /// <summary>
        /// Configurations the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);

            ApiConfiguration.Register(Config);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(Config);
        }

        /// <summary>
        /// Configures the o authentication.
        /// </summary>
        /// <param name="app">The application.</param>
        public void ConfigureOAuth(IAppBuilder app)
        {
            // to register this custom OwinMiddleware
            app.Use<AuthenticationMiddleware>();

            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Core.Security.Configuration.Current.AuthCookieName);

            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new AuthorizationServerProvider(),
                RefreshTokenProvider = new RefreshTokenProvider(),
                ApplicationCanDisplayErrors = true
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            // Enable this for cookie support Authentication
            app.SetDefaultSignInAsAuthenticationType(Core.Security.Configuration.Current.AuthCookieName);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            GoogleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = ConfigurationManager.AppSettings["GoogleClientId"] ?? string.Empty,
                ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"] ?? string.Empty,
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(GoogleAuthOptions);

            FacebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = ConfigurationManager.AppSettings["FBClientId"] ?? string.Empty,
                AppSecret = ConfigurationManager.AppSettings["FBClientId"] ?? string.Empty,
                Provider = new FacebookAuthProvider()
            };
            app.UseFacebookAuthentication(FacebookAuthOptions);
        }
    }
}