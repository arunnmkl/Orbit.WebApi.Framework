using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using Orbit.WebApi.Core;
using Orbit.WebApi.Core.Dependency;
using Orbit.WebApi.Core.Enums;
using Orbit.WebApi.Core.Filters;
using Orbit.WebApi.Core.Handlers;
using Orbit.WebApi.Core.Interfaces;
using Orbit.WebApi.Core.Services;
using Orbit.WebApi.Extensions.Authentication;
using Orbit.WebApi.Extensions.Validation;
using Orbit.WebApi.Security;
using ApiSecurity = Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Extensions
{
    /// <summary>
    /// A class to configure the Configuration using API.
    /// </summary>
    public static class ApiConfiguration
    {
        /// <summary>
        /// Configurations the specified configuration, to the web api calls.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="routingConfig">The routing configuration.</param>
        public static void Configure(this HttpConfiguration config, RoutingConfig routingConfig = RoutingConfig.Namespace)
        {
            // Web API configuration and services
            RegisterDefaultValues();

            Config.Configure(config, routingConfig);

            if (ApiSecurity.Configuration.Current.OAuthAuthenticationEnabled)
            {
                config.Filters.Add(new HostAuthenticationAttribute("bearer"));
                config.Filters.Add(new BearerAuthenticationFilter());

                if (ApiSecurity.Configuration.Current.OrbitAuthorizationEnabled)
                {
                    config.Filters.Add(new OrbitAuthorizationAttribute());
                }
            }

            if (ApiSecurity.Configuration.Current.CSRFAttackPrevented)
            {
                config.MessageHandlers.Add(new CSRFHandler());
            }

            if (ApiSecurity.Configuration.Current.AuthenticationEnabled)
            {
                config.Filters.Add(new AuthenticationAttribute());

                if (ApiSecurity.Configuration.Current.OrbitAuthorizationEnabled)
                {
                    config.Filters.Add(new OrbitAuthorizationAttribute());
                }
            }

            config.Services.Replace(typeof(IExceptionHandler), new GeneralExceptionHandler());
        }

        /// <summary>
        /// Configurations the specified configuration, for MVC web application.
        /// </summary>
        /// <param name="filterCollection">The filter collection.</param>
        public static void ConfigureMVC(this GlobalFilterCollection filterCollection)
        {
            // Web MVC configuration and services
            DependencyResolverContainer.RegisterInstance<IAuthenticationCommand>(new AuthenticationCommand());

            // Web MVC web application routes
            filterCollection.Add(new MVCAuthenticationAttribute());
        }

        /// <summary>
        /// Registers the default values.
        /// </summary>
        private static void RegisterDefaultValues()
        {
            if (ApiSecurity.Configuration.Current.OAuthAuthenticationEnabled)
            {
                DependencyResolverContainer.RegisterInstance<IBearerAuthenticationCommand>(new BearerAuthenticationCommand());

                if (ApiSecurity.Configuration.Current.OrbitAuthorizationEnabled)
                {
                    DependencyResolverContainer.RegisterInstance<IAuthorization>(new AuthorizationController());
                }
            }

            if (ApiSecurity.Configuration.Current.CSRFAttackPrevented)
            {
                DependencyResolverContainer.RegisterInstance<ICSRFValidation>(new CSRFValidation());
            }

            if (ApiSecurity.Configuration.Current.AuthenticationEnabled)
            {
                DependencyResolverContainer.RegisterInstance<IAuthenticationCommand>(new AuthenticationCommand());

                if (ApiSecurity.Configuration.Current.OrbitAuthorizationEnabled)
                {
                    DependencyResolverContainer.RegisterInstance<IAuthorization>(new AuthorizationController());
                }
            }

            DependencyResolverContainer.RegisterInstance<ILog>(new Logging.Logging());

            DependencyResolverContainer.RegisterInstance<ISecurityCommand>(new SecurityCommand());
        }

        /// <summary>
        /// Registers the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public static HttpConfiguration Register(HttpConfiguration config)
        {
            config.Configure(RoutingConfig.Default);

            return config;
        }
    }
}