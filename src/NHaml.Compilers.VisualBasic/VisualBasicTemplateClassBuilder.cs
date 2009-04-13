using System;

using NHaml.Utils;

namespace NHaml.Compilers.VisualBasic
{
    internal sealed class VisualBasicTemplateClassBuilder : TemplateClassBuilder
    {
        public VisualBasicTemplateClassBuilder(string className, Type baseType)
            : base( className )
        {
            Preamble.AppendLine( Utility.FormatInvariant( "public Class {0} \r\nInherits {1} ",
                                                          ClassName, Utility.MakeBaseClassName( baseType, "(Of ", ")", "." ) ) );
            Preamble.AppendLine("Protected Overrides Sub CoreRender(ByVal textWriter As TextWriter)");
        }

        public override void AppendOutput( string value, bool newLine )
        {
            if( value == null )
            {
                return;
            }

            var method = newLine ? "WriteLine" : "Write";

            value = value.Replace( "\"", "\"\"" );

            if( Depth > 0 )
            {
                if( value.StartsWith( string.Empty.PadLeft( Depth * 2 ), StringComparison.Ordinal ) )
                {
                    value = value.Remove( 0, Depth * 2 );
                }
            }

            Output.AppendFormat("textWriter.{0}(\"{1}\")\r\n", method, value);
        }

        public override void AppendCode( string code, bool newLine, bool escapeHtml )
        {
            if( code != null )
            {

                code = "(Convert.ToString(" + code + "))";

                if( escapeHtml )
                {
                    code = "(HttpUtility.HtmlEncode" + code + ")";
                }

                Output.AppendLine( "textWriter." + (newLine ? "WriteLine" : "Write") + code );
            }
        }

        public override void AppendChangeOutputDepth( int depth )
        {
            if( BlockDepth != depth )
            {
                Output.AppendLine( "Output.Depth = " + depth );

                BlockDepth = depth;
            }
        }

        public override void AppendSilentCode( string code, bool closeStatement )
        {
            if( code != null )
            {
                code = code.Trim();

                //if( closeStatement && !code.EndsWith( ";", StringComparison.Ordinal ) )
                //{
                //    code += ';';
                //}

                Output.AppendLine( code );
            }
        }

        public override void AppendAttributeCode( string name, string code )
        {
            var varName = "a" + AttributeCount++;

            if (code.StartsWith("'") && code.EndsWith("'") && code.Length == 3)
            {
                AppendSilentCode(string.Format("Dim {0} As String=Convert.ToString(\"{1}\"c)", varName, code[1]), true);
            }
            else
            {
                AppendSilentCode(string.Format("Dim {0} As String=Convert.ToString({1})", varName, code), true);
    
            }
            AppendSilentCode(string.Format("If ({0} IsNot Nothing) Then", varName), false);
            AppendOutput( name + "=\"" );
            Output.AppendLine( string.Format("textWriter.Write({0})", varName) );
            AppendOutput( "\"" );
            AppendSilentCode( "End If", false );
        }

        public override void BeginCodeBlock()
        {
            Depth++;
          //  Output.AppendLine( "{" );
        }

        public override void EndCodeBlock()
        {
         //  Output.AppendLine( string.Empty);
            Depth--;
        }

        public override void AppendPreambleCode( string code )
        {
            Preamble.AppendLine( code  );
        }

        public override string Build()
        {
            Output.AppendLine( "End Sub" );
            Output.Append( "End Class" );

            Preamble.Append( Output );

            return Preamble.ToString();
        }
    }
}