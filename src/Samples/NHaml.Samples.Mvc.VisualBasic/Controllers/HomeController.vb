Imports System.Web.Mvc

Namespace Controllers
    Public Class HomeController
        Inherits Controller
        Public Function Index() As ActionResult
            Return View("Index")
        End Function

        Public Function About() As ActionResult
            Return View("About")
        End Function
    End Class
End Namespace