Imports System.Configuration
Imports System.Web.Mvc
Imports NHaml.Samples.Mvc.VisualBasic.Models

Namespace Controllers
    Public Class ProductsController
        Inherits Controller
        Private ReadOnly northwind As New NorthwindDataContext(ConfigurationManager.ConnectionStrings("NorthwindConnectionString").ConnectionString)

        '
        ' Products/Category/1

        Public Function Category(ByVal id As Integer) As ActionResult
            'INSTANT VB NOTE: The local variable category was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
            Dim category_Renamed = northwind.GetCategoryById(id)

            Return View("List", category_Renamed)
        End Function

        '
        ' Products/New

        Public Function [New]() As ActionResult
            Dim viewData = New ProductsNewViewData With {.Suppliers = New SelectList(northwind.GetSuppliers(), "SupplierID", "CompanyName"), .Categories = New SelectList(northwind.GetCategories(), "CategoryID", "CategoryName")}

            Return View("New", viewData)
        End Function

        '
        ' Products/Create

        Public Function Create() As ActionResult
            Dim product = New Product()
            UpdateModel(product, Request.Form.AllKeys)

            northwind.AddProduct(product)
            northwind.SubmitChanges()

            Return RedirectToAction("Category", New With {Key .ID = product.CategoryID})
        End Function

        '
        ' Products/Edit/5

        Public Function Edit(ByVal id As Integer) As ActionResult
            Dim viewData = New ProductsEditViewData With {.Product = northwind.GetProductById(id)}

            viewData.Categories = New SelectList(northwind.GetCategories(), "CategoryID", "CategoryName", viewData.Product.CategoryID)
            viewData.Suppliers = New SelectList(northwind.GetSuppliers(), "SupplierID", "CompanyName", viewData.Product.SupplierID)

            Return View("Edit", viewData)
        End Function

        '
        ' Products/Update/5

        Public Function Update(ByVal id As Integer) As ActionResult
            Dim product = northwind.GetProductById(id)
            UpdateModel(product, Request.Form.AllKeys)

            northwind.SubmitChanges()

            Return RedirectToAction("Category", New With {Key .Action = "Category", Key .ID = product.CategoryID})
        End Function
    End Class
End Namespace