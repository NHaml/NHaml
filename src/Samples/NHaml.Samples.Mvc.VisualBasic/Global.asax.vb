Imports System.Web.SessionState
Imports System.Web.Mvc
Imports System.Web.Routing
Imports NHaml.Web.Mvc.VisualBasic

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        AppDomain.CurrentDomain.SetData("SQLServerCompactEditionUnderWebHosting", True)

        ' Change Url= to Url="[controller].mvc/[action]/[id]" to enable 
        ' automatic support on IIS6 

        RouteTable.Routes.Add(New Route("{controller}.mvc/{action}/{id}", New MvcRouteHandler()) With {.Defaults = New RouteValueDictionary(New With {Key .action = "index", Key .id = ""})})

        RouteTable.Routes.Add(New Route("Default.aspx", New MvcRouteHandler()) With {.Defaults = New RouteValueDictionary(New With {Key .controller = "Home", Key .action = "index", Key .id = ""})})

        ViewEngines.Engines.Add(New NHamlMvcVisualBasicViewEngine())
    End Sub


End Class