using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Orbit.Messaging.Owin;
using Orbit.Messaging.Security;
using Owin;

[assembly: OwinStartup(typeof(Orbit.Messaging.Startup))]
namespace Orbit.Messaging
{
    /// <summary>
    /// Owin startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the o authentication bearer options.
        /// </summary>
        /// <value>
        /// The o authentication bearer options.
        /// </value>
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Configurations the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
        }

        /// <summary>
        /// Configures the o authentication.
        /// </summary>
        /// <param name="app">The application.</param>
        private void ConfigureOAuth(IAppBuilder app)
        {
            //Token Consumption
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions()
            {
                Provider = new OAuthBearerTokenAuthenticationProvider(),
            };

            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            // Branch the pipeline here for requests that start with "/signalr"
            app.Map(Security.Configuration.Current.PathMatchValue, map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                    Resolver = GlobalHost.DependencyResolver,
                    EnableDetailedErrors = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path

                hubConfiguration.EnableDetailedErrors = true;
                map.RunSignalR(hubConfiguration);
            });

            // Require authentication for all hubs
            var authorizer = new HubAuthorizeAttribute();
            GlobalHost.HubPipeline.AddModule(new HubAuthorizeModule());
            GlobalHost.HubPipeline.AddModule(new AuthorizeModule(authorizer, authorizer));
            GlobalHost.HubPipeline.RequireAuthentication();
        }
    }
}
