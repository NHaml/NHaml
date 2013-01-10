using System.Configuration;
using System.Web.Mvc;
using NHaml.Samples.Mvc.CSharp.Models;
using System.Collections.Generic;

namespace NHaml.Samples.Mvc.CSharp.Controllers
{
    public class ProductsController : Controller
    {
        public ActionResult Category( int id )
        {
            var category = GetCategory();

            return View( "List", category );
        }

        private Category GetCategory()
        {
            return new Category
            {
                CategoryName = "CategoryName",
                Description = "Category description",
                Products = new List<Product>()
            };
        }

        ////
        //// Products/New

        //public ActionResult New()
        //{
        //    var viewData = new ProductsNewViewData
        //                       {
        //                           Suppliers = new SelectList( northwind.GetSuppliers(), "SupplierID", "CompanyName" ),
        //                           Categories = new SelectList( northwind.GetCategories(), "CategoryID", "CategoryName" )
        //                       };

        //    return View( "New", viewData );
        //}

        ////
        //// Products/Create

        //public ActionResult Create()
        //{
        //    var product = new Product();
        //    UpdateModel( product, Request.Form.AllKeys );

        //    northwind.AddProduct( product );
        //    northwind.SubmitChanges();

        //    return RedirectToAction( "Category", new { ID = product.CategoryID } );
        //}

        ////
        //// Products/Edit/5

        //public ActionResult Edit( int id )
        //{
        //    var viewData = new ProductsEditViewData { Product = northwind.GetProductById( id ) };

        //    viewData.Categories = new SelectList( northwind.GetCategories(), "CategoryID", "CategoryName", viewData.Product.CategoryID );
        //    viewData.Suppliers = new SelectList( northwind.GetSuppliers(), "SupplierID", "CompanyName", viewData.Product.SupplierID );

        //    return View( "Edit", viewData );
        //}

        ////
        //// Products/Update/5

        //public ActionResult Update( int id )
        //{
        //    var product = northwind.GetProductById( id );
        //    UpdateModel( product, Request.Form.AllKeys );

        //    northwind.SubmitChanges();

        //    return RedirectToAction( "Category", new { Action = "Category", ID = product.CategoryID } );
        //}
    }
}