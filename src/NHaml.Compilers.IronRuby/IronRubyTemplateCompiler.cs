using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using IronRuby;

using Microsoft.Scripting.Hosting;

using NHaml.Rules;

namespace NHaml.Compilers.IronRuby
{
    public class IronRubyTemplateCompiler : ITemplateCompiler
    {
        private static readonly List<string> MidBlockKeywords
          = new List<string> { "ELSE", "ELSIF", "RESCUE", "ENSURE", "WHEN" };

        private readonly ScriptEngine _scriptEngine = Ruby.CreateEngine();

        public TemplateClassBuilder CreateTemplateClassBuilder( string className, Type templateBaseType )
        {
            return new IronRubyTemplateClassBuilder( className, templateBaseType );
        }

        public TemplateFactory Compile( TemplateParser templateParser )
        {
            var ruby = new StringBuilder();

            foreach( var reference in templateParser.TemplateEngine.References )
            {
                ruby.AppendLine( string.Format("require '{0}'", reference) );
            }

            ruby.Append(templateParser.TemplateClassBuilder.Build(templateParser.TemplateEngine.Usings));

            var templateSource = ruby.ToString();

            Trace.WriteLine( templateSource );

            _scriptEngine.Execute( templateSource );

            return CreateTemplateFactory( _scriptEngine, templateParser.TemplateClassBuilder.ClassName );
        }

        protected virtual IronRubyTemplateFactory CreateTemplateFactory( ScriptEngine scriptEngine, string className )
        {
            return new IronRubyTemplateFactory( _scriptEngine, className );
        }

        public BlockClosingAction RenderSilentEval( TemplateParser templateParser )
        {
            var code = templateParser.CurrentInputLine.NormalizedText;

            templateParser.TemplateClassBuilder.AppendSilentCode( code, false );

            if( templateParser.IsBlock )
            {
                templateParser.TemplateClassBuilder.BeginCodeBlock();

                if( !templateParser.CurrentInputLine.NormalizedText.Trim().Split( ' ' )[0].ToUpperInvariant().Equals( "CASE" ) )
                {
                    return () =>
                      {
                          if( (templateParser.CurrentInputLine.Signifier == SilentEvalMarkupRule.SignifierChar) &&
                            MidBlockKeywords.Contains( templateParser.CurrentInputLine.NormalizedText.Trim().Split( ' ' )[0].ToUpperInvariant() ) )
                          {
                              templateParser.TemplateClassBuilder.Unindent();
                          }
                          else
                          {
                              templateParser.TemplateClassBuilder.EndCodeBlock();
                          }
                      };
                }
            }

            return null;
        }

        public void RenderAttributes( TemplateParser templateParser, string attributes )
        {
            templateParser.TemplateClassBuilder.AppendCode( string.Format("__a({0})", attributes) );
        }
    }
}