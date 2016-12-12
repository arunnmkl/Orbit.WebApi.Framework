using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Orbit.WebApi.Api.Common;
using Orbit.WebApi.Core.Dependency;
using Orbit.WebApi.Security;

namespace Orbit.WebApi.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new GlobalExceptionFilterAttribute());

            DependencyResolverContainer.RegisterInstance<ISecurityCommand>(new SecurityCommand());
        }
    }
}
