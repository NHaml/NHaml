using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using NHaml.Compilers.CSharp2;

namespace NHaml.Compilers.CSharp3
{
    public sealed class CSharp3TemplateCompiler : CSharp2TemplateCompiler
    {
        public override string TranslateLambda( string codeLine, Match lambdaMatch )
        {
            return codeLine.Substring( 0, lambdaMatch.Groups[1].Length - 2 )
              + (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith( "()", StringComparison.OrdinalIgnoreCase ) ? null : ", ")
                + lambdaMatch.Groups[2].Captures[0].Value + " => {";
        }

        protected override void RenderAttributesCore( TemplateParser templateParser, string attributes )
        {
            var method = string.Format( "{0}.RenderAttributesAnonymousObject(new {{{1}}})",
                GetType().FullName,
                attributes );

            templateParser.TemplateClassBuilder
                .AppendCode( method );
        }
        
        public static string RenderAttributesAnonymousObject( object attributeSource )
        {
            if( attributeSource != null )
            {
                var properties = TypeDescriptor.GetProperties( attributeSource );

                if( properties.Count > 0 )
                {
                    var attributes = new StringBuilder();

                    AppendAttribute( attributeSource, properties[0], attributes, null );

                    for( var i = 1; i < properties.Count; i++ )
                    {
                        AppendAttribute( attributeSource, properties[i], attributes, " " );
                    }

                    return attributes.ToString();
                }
            }

            return null;
        }

        private static void AppendAttribute( object obj, PropertyDescriptor propertyDescriptor,
          StringBuilder attributes, string separator )
        {
            var value = propertyDescriptor.GetValue( obj );
            var name = propertyDescriptor.Name.Replace( '_', '-' );
            
            var invariantName = Convert.ToString( name, CultureInfo.InvariantCulture );

            if( value != null )
            {
                var invariantValue = Convert.ToString( value, CultureInfo.InvariantCulture );

                attributes.Append( separator + invariantName + "=\"" + invariantValue + "\"" );
            }
        }

        internal override CSharp2TemplateTypeBuilder CreateTemplateTypeBuilder( TemplateEngine templateEngine )
        {
            return new CSharp3TemplateTypeBuilder( templateEngine );
        }
    }
}