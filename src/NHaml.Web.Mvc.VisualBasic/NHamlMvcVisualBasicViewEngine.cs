using System.Security.Permissions;
using System.Web;
using NHaml.Compilers.VisualBasic;

namespace NHaml.Web.Mvc.VisualBasic
{
    [AspNetHostingPermission( SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    [AspNetHostingPermission( SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal )]
    public class NHamlMvcVisualBasicViewEngine : NHamlMvcViewEngine
    {
        public NHamlMvcVisualBasicViewEngine()
        {
            TemplateEngine.Options.TemplateCompiler = new VisualBasicTemplateCompiler();

            TemplateEngine.Options.AddReference( typeof( NHamlMvcVisualBasicViewEngine ).Assembly );

        }

    }
}