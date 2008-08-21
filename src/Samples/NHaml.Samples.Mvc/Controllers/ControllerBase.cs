using System.Configuration;
using System.Web.Mvc;

using NHaml.Samples.Mvc.Models;

namespace NHaml.Samples.Mvc.Controllers
{
  public abstract class ControllerBase : Controller
  {
    private readonly NorthwindDataContext _northwind = new NorthwindDataContext(
      ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString);

    public NorthwindDataContext Northwind
    {
      get { return _northwind; }
    }
  }
}