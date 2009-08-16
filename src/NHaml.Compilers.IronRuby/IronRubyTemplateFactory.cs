using System.IO;

using Microsoft.Scripting.Hosting;

namespace NHaml.Compilers.IronRuby
{
    public class IronRubyTemplateFactory : TemplateFactory
    {

        public IronRubyTemplateFactory( ScriptEngine scriptEngine, string className )
        {
            ScriptEngine = scriptEngine;

            var format = string.Format("{0}.new.method(:render)", className);
            var scriptSource = scriptEngine.CreateScriptSourceFromString(format);
            RenderAction = scriptSource.Execute();
        }

        public ScriptEngine ScriptEngine{get; private set;}

        public object RenderAction{ get;private set;}

        public override Template CreateTemplate()
        {
            return new DlrShimTemplate( ScriptEngine, RenderAction );
        }

        protected class DlrShimTemplate : Template
        {

            public DlrShimTemplate( ScriptEngine scriptEngine, object renderAction )
            {
                ScriptEngine = scriptEngine;
                RenderAction = renderAction;
            }

            public ScriptEngine ScriptEngine{get; private set;}

            public object RenderAction {get;private set;}

            protected override void CoreRender( TextWriter textWriter )
            {
                ScriptEngine.Operations.Call( RenderAction, textWriter );
            }
        }
    }
}