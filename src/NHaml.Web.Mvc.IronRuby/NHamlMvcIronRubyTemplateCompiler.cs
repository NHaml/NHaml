using System.IO;
using System.Text;
using System.Web.Mvc;

using Microsoft.Scripting.Hosting;

using NHaml.Compilers.IronRuby;

namespace NHaml.Web.Mvc.IronRuby
{
    internal sealed class NHamlMvcIronRubyTemplateCompiler : IronRubyTemplateCompiler
    {
        protected override IronRubyTemplateFactory CreateTemplateFactory( ScriptEngine scriptEngine, string className )
        {
            return new NHamlMvcIronRubyTemplateFactory( scriptEngine, className );
        }

        private sealed class NHamlMvcIronRubyTemplateFactory : IronRubyTemplateFactory
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

                protected class DlrTextWriterShim : TextWriter
                {
                    // because IronRuby CLS method resolution is still flakey.

                    private readonly TextWriter _textWriter;

                    public DlrTextWriterShim( TextWriter textWriter )
                        : base( textWriter.FormatProvider )
                    {
                        _textWriter = textWriter;
                    }

                    public override Encoding Encoding
                    {
                        get { return _textWriter.Encoding; }
                    }

                    public override void Write( char value )
                    {
                        _textWriter.Write( value );
                    }

                    public override void Write( char[] buffer, int index, int count )
                    {
                        _textWriter.Write( buffer, index, count );
                    }

                    public override void Write( string value )
                    {
                        _textWriter.Write( value );
                    }
                }
            }
        }
    }
}