using System.Web.Mvc;

namespace Orbit.WebApi.Api.Areas.HelpPage
{
    /// <summary>
    /// Class for Help Page
    /// </summary>
    public class HelpPageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HelpPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HelpPage_Default",
                "Help/{action}/{apiId}",
                new { controller = "Help", action = "Index", apiId = UrlParameter.Optional });

            HelpPageConfig.Register(Extensions.Startup.Config);
        }
    }
}