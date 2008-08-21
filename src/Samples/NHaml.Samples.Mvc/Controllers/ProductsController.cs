using System.Web.Mvc;

using NHaml.Samples.Mvc.Models;

namespace NHaml.Samples.Mvc.Controllers
{
  public class ProductsController : ControllerBase
  {
    public ActionResult Index()
    {
      return View(Northwind.Products);
    }

    public ActionResult Show(int id)
    {
      return View(Northwind.GetProductById(id));
    }

    public ActionResult New()
    {
      return View(new Product());
    }

    public ActionResult Create()
    {
      var products = new Product();

      BindingHelperExtensions.UpdateFrom(products, Request.Form);

      Northwind.AddProduct(products);
      Northwind.SubmitChanges();

      return this.RedirectToAction(c => c.Show(products.Id));
    }

    public ActionResult Edit(int id)
    {
      return View(Northwind.GetProductById(id));
    }

    public ActionResult Update(int id)
    {
      var products = Northwind.GetProductById(id);

      BindingHelperExtensions.UpdateFrom(products, Request.Form);

      Northwind.SubmitChanges();

      return this.RedirectToAction(c => c.Show(products.Id));
    }

    public ActionResult Destroy(int id)
    {
      Northwind.RemoveProduct(id);
      Northwind.SubmitChanges();

      return this.RedirectToAction(c => c.Index());
    }
  }
}