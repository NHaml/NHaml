Imports System.Web.Mvc

Namespace Models
    Public Class ProductsNewViewData
        Private privateSuppliers As SelectList
        Public Property Suppliers() As SelectList
            Get
                Return privateSuppliers
            End Get
            Set(ByVal value As SelectList)
                privateSuppliers = value
            End Set
        End Property
        Private privateCategories As SelectList
        Public Property Categories() As SelectList
            Get
                Return privateCategories
            End Get
            Set(ByVal value As SelectList)
                privateCategories = value
            End Set
        End Property
    End Class
End Namespace