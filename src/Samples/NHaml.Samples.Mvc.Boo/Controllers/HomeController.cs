using System.Web.Mvc;

namespace NHaml.Samples.Mvc.Boo.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View("Index");
    }

    public ActionResult About()
    {
      return View("About");
    }
  }
}