using System.IO;
using System.Web.Mvc;

using Microsoft.Scripting.Hosting;

using NHaml.Compilers.IronRuby;

namespace NHaml.Web.Mvc.IronRuby
{
    internal  class NHamlMvcIronRubyTemplateCompiler : IronRubyTemplateCompiler
    {
        protected override IronRubyTemplateFactory CreateTemplateFactory( ScriptEngine scriptEngine, string className )
        {
            return new NHamlMvcIronRubyTemplateFactory( scriptEngine, className );
        }

        private  class NHamlMvcIronRubyTemplateFactory : IronRubyTemplateFactory
        {
            public NHamlMvcIronRubyTemplateFactory( ScriptEngine scriptEngine, string className )
                : base( scriptEngine, className )
            {
            }

            public override Template CreateTemplate()
            {
                return new MvcDlrShimTemplate( ScriptEngine, RenderAction );
            }

            private class MvcDlrShimTemplate : DlrShimTemplate, IView
            {
                public MvcDlrShimTemplate( ScriptEngine scriptEngine, object renderAction )
                    : base( scriptEngine, renderAction )
                {
                }

                public void Render( ViewContext viewContext, TextWriter writer )
                {
                    ScriptEngine.Operations.Call( RenderAction, viewContext, new DlrTextWriterShim( writer ) );
                }

            }
        }
    }
}