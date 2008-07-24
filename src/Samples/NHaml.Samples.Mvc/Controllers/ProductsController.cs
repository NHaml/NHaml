using System.Configuration;
using System.Web.Mvc;

using NHaml.Samples.Mvc.Models;

namespace NHaml.Samples.Mvc.Controllers
{
  public class ProductsController : Controller
  {
    private readonly NorthwindDataContext northwind = new NorthwindDataContext(
      ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString);

    //
    // Products/Category/1

    public ActionResult Category(int id)
    {
      var category = northwind.GetCategoryById(id);

      return View("List", category);
    }

    //
    // Products/New

    public ActionResult New()
    {
      var viewData = new ProductsNewViewData();

      viewData.Suppliers = new SelectList(northwind.GetSuppliers(), "SupplierID", "CompanyName");
      viewData.Categories = new SelectList(northwind.GetCategories(), "CategoryID", "CategoryName");

      return View("New", viewData);
    }

    //
    // Products/Create

    public ActionResult Create()
    {
      var product = new Product();
      BindingHelperExtensions.UpdateFrom(product, Request.Form);

      northwind.AddProduct(product);
      northwind.SubmitChanges();

      return RedirectToAction("Category", new {ID = product.CategoryID});
    }

    //
    // Products/Edit/5

    public ActionResult Edit(int id)
    {
      var viewData = new ProductsEditViewData();

      viewData.Product = northwind.GetProductById(id);
      viewData.Categories = new SelectList(northwind.GetCategories(), "CategoryID", "CategoryName", viewData.Product.CategoryID);
      viewData.Suppliers = new SelectList(northwind.GetSuppliers(), "SupplierID", "CompanyName", viewData.Product.SupplierID);

      return View("Edit", viewData);
    }

    //
    // Products/Update/5

    public ActionResult Update(int id)
    {
      var product = northwind.GetProductById(id);
      BindingHelperExtensions.UpdateFrom(product, Request.Form);

      northwind.SubmitChanges();

      return RedirectToAction("Category", new {Action = "Category", ID = product.CategoryID});
    }
  }
}