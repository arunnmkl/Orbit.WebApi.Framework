using System.Web.Mvc;

namespace Orbit.WebApi.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Orbit Web API";

            return View();
        }
    }
}
