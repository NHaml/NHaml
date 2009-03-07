using System.Configuration;
using Castle.MonoRail.Framework;
using NHaml.Samples.MonoRail.Models;

namespace NHaml.Samples.MonoRail.Controllers
{
    public class ProductsController : SmartDispatcherController
    {
        private readonly NorthwindDataContext northwind = new NorthwindDataContext(
            ConfigurationManager.ConnectionStrings["NorthwindConnectionString"].ConnectionString );

        //
        // Products/Category/1
        public void Category(int id)
        {
            var category = northwind.GetCategoryById(id);

            PropertyBag["category"] = category;
            base.RenderView("List");
        }

        //
        // Products/New
        public void New()
        {
            //var viewData = new ProductsNewViewData
            //                 {
            //                     Suppliers = new SelectList( northwind.GetSuppliers(), "SupplierID", "CompanyName" ),
            //                     Categories = new SelectList( northwind.GetCategories(), "CategoryID", "CategoryName" )
            //                 };

            //return View( "New", viewData );
        }

        //
        // Products/Create

        public void Create()
        {
            //var product = new Product();
            //UpdateModel( product, Request.Form.AllKeys );

            //northwind.AddProduct( product );
            //northwind.SubmitChanges();

            //return RedirectToAction( "Category", new { ID = product.CategoryID } );
        }

        //
        // Products/Edit/5

        public void Edit( int id )
        {
            //var viewData = new ProductsEditViewData { Product = northwind.GetProductById(id) };

            //viewData.Categories = new SelectList(northwind.GetCategories(), "CategoryID", "CategoryName", viewData.Product.CategoryID);
            //viewData.Suppliers = new SelectList(northwind.GetSuppliers(), "SupplierID", "CompanyName", viewData.Product.SupplierID);

            //return View("Edit", viewData);
        }

        //
        // Products/Update/5

        public void Update( int id )
        {
            //var product = northwind.GetProductById( id );
            //UpdateModel( product, Request.Form.AllKeys );

            //northwind.SubmitChanges();

            //return RedirectToAction( "Category", new { Action = "Category", ID = product.CategoryID } );
        }
    }
}