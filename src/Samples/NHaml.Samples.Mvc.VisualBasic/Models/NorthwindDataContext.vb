Imports System.Collections.Generic
Imports System.Linq

Namespace Models
    Partial Public Class NorthwindDataContext
        ' Retrieve All Category Objects

        Public Function GetCategories() As List(Of Category)
            Return Categories.ToList()
        End Function

        ' Retrieve all Suppliers

        Public Function GetSuppliers() As List(Of Supplier)
            Return Suppliers.ToList()
        End Function

        ' Retrieve a Specific Category

        Public Function GetCategoryById(ByVal categoryId As Integer) As Category
            Return Categories.First(Function(c) c.CategoryID = categoryId)
        End Function

        ' Retrieve a specific Product

        Public Function GetProductById(ByVal productId As Integer) As Product
            Return Products.Single(Function(p) p.ProductID = productId)
        End Function

        ' Add a New Product

        Public Sub AddProduct(ByVal product As Product)
            Products.InsertOnSubmit(product)
        End Sub
    End Class
End Namespace