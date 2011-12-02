using System.Diagnostics.CodeAnalysis;
using NHaml.Compilers;
using NHaml.Parser;

namespace NHaml.Rules
{
    [SuppressMessage( "Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly" )]
    public class PlainTextMarkupRule : MarkupRule
    {
        [SuppressMessage( "Microsoft.Security", "CA2104" )]
        public static readonly PlainTextMarkupRule Instance = new PlainTextMarkupRule();

        private PlainTextMarkupRule()
        {
        }

        public override string Signifier
        {
            get { return string.Empty; }
        }

        public override BlockClosingAction Render(HamlNode node, TemplateOptions options, TemplateClassBuilder builder)
        {
            //var inputLine = viewSourceReader.CurrentInputLine;
            //builder.AppendOutput(inputLine.Indent);
            
            //AppendText(inputLine.NormalizedText, builder, options);

            //builder.AppendOutputLine();

            return EmptyClosingAction;
        }

     
    }

  
}