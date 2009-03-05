using System;
using System.Web;

namespace NHaml.Samples.MonoRail
{
    public class Global : HttpApplication
    {
        protected void Application_Start( object sender, EventArgs e )
        {
            AppDomain.CurrentDomain.SetData( "SQLServerCompactEditionUnderWebHosting", true );

        }
    }
}