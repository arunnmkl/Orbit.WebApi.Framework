using System.Web.Optimization;

namespace Orbit.Angular.MVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/angular.js",
                "~/Scripts/angular-route.js",
                "~/Scripts/angular-local-storage.js",
                "~/Scripts/loading-bar.js",
                "~/Scripts/angular-acl.js",
                "~/Scripts/angular-ui/ui-bootstrap-tpls.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/content/css/font-awesome.css",
                      "~/Content/loading-bar.css",
                      "~/Content/social-buttons.css",
                      "~/Content/ui-bootstrap-csp.css"));

            bundles.Add(new ScriptBundle("~/bundles/App").IncludeDirectory(
                "~/App", "*.js", false));

            bundles.Add(new ScriptBundle("~/bundles/Modules").IncludeDirectory(
                "~/App/Modules", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/Directives").IncludeDirectory(
                "~/App/Directives", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/Controllers").IncludeDirectory(
                "~/App/Controllers", "*.js", true));

            bundles.Add(new ScriptBundle("~/bundles/Services").IncludeDirectory(
                "~/App/Services", "*.js", true));
        }
    }
}
