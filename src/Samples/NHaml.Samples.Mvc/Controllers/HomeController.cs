using System.Web.Mvc;

namespace NHaml.Samples.Mvc.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}