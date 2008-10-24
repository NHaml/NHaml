using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using NHaml.Web.Mvc;

namespace NHaml.Samples.Mvc
{
  public class Global : HttpApplication
  {
    protected void Application_Start(object sender, EventArgs e)
    {
      AppDomain.CurrentDomain.SetData("SQLServerCompactEditionUnderWebHosting", true);

      // Change Url= to Url="[controller].mvc/[action]/[id]" to enable 
      // automatic support on IIS6 

      RouteTable.Routes.Add(new Route("{controller}.mvc/{action}/{id}", new MvcRouteHandler())
                              {
                                Defaults = new RouteValueDictionary(
                                  new
                                    {
                                      action = "index", 
                                      id = ""
                                    })
                              });

      RouteTable.Routes.Add(new Route("Default.aspx", new MvcRouteHandler())
                              {
                                Defaults = new RouteValueDictionary(
                                  new
                                    {
                                      controller = "Home", 
                                      action = "index", id = ""
                                    })
                              });

      ViewEngines.Engines.Add(new NHamlViewMvcEngine());
    }
  }
}