using System.Web.Mvc;
using NHaml.Samples.Mvc.Models;

namespace NHaml.Samples.Mvc.Controllers
{
  public class CategoriesController : ControllerBase
  {
    public ActionResult Index()
    {
      return View(Northwind.Categories);
    }

    public ActionResult Show(int id)
    {
      return View(Northwind.GetCategoryById(id));
    }

    public ActionResult New()
    {
      return View(new Category());
    }

    public ActionResult Create()
    {
      var category = new Category();

      UpdateModel(category, Request.Form.AllKeys);

      Northwind.AddCategory(category);
      Northwind.SubmitChanges();

      return this.RedirectToAction(c => c.Show(category.Id));
    }

    public ActionResult Edit(int id)
    {
      return View(Northwind.GetCategoryById(id));
    }

    public ActionResult Update(int id)
    {
      var category = Northwind.GetCategoryById(id);

      UpdateModel(category, Request.Form.AllKeys);

      Northwind.SubmitChanges();

      return this.RedirectToAction(c => c.Show(category.Id));
    }

    public ActionResult Destroy(int id)
    {
      Northwind.RemoveCategory(id);
      Northwind.SubmitChanges();

      return this.RedirectToAction(c => c.Index());
    }
  }
}