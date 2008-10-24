using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using NHaml.Coco;
using NHaml.Exceptions;
using NHaml.Properties;

namespace NHaml.Backends.CSharp2
{
  [SuppressMessage("Microsoft.Naming", "CA1722")]
  public sealed class CSharp2AttributeRenderer : IAttributeRenderer
  {
    public void Render(CompilationContext compilationContext, string attributes)
    {
      var stream = new MemoryStream(Encoding.UTF8.GetBytes("class _ {object " + attributes + ";}"));
      var scanner = new Scanner(stream);
      var parser = new Parser(scanner);

      parser.Parse();

      if (parser.errors.count > 0)
      {
        SyntaxException.Throw(compilationContext.CurrentInputLine, Resources.AttributesParseError);
      }

      if (parser.variables.Count > 0)
      {
        AppendAttribute(compilationContext, parser.variables[0], null);

        for (var i = 1; i < parser.variables.Count; i++)
        {
          AppendAttribute(compilationContext, parser.variables[i], " ");
        }
      }
    }

    private static void AppendAttribute(CompilationContext compilationContext,
      DictionaryEntry variable, string separator)
    {
      compilationContext.TemplateClassBuilder.AppendAttributeCode(
        separator + variable.Key.ToString().Replace('_', '-').Replace("@", ""),
        variable.Value.ToString());
    }
  }
}