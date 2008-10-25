using System;
using System.Collections.Generic;

using NHaml.Utils;

namespace NHaml.BackEnds.Boo
{
  public sealed class BooTemplateClassBuilder : TemplateClassBuilderBase
  {
    private bool _disableOutputIndentationShrink;

    public BooTemplateClassBuilder(Type viewBaseType,
      string className, params Type[] genericArguments)
      : base(className)
    {
      var init = Utility.FormatInvariant("class {0}({1}):", ClassName, MakeBaseTypeName(viewBaseType, genericArguments));
      Preamble.AppendLine(init);
      Preamble.AppendLine("  _output as TemplateOutputWriter");
      Preamble.AppendLine("  def constructor():");
      Preamble.AppendLine("    _output = TemplateOutputWriter()");

      Preamble.AppendLine("  override Output: ");
      Preamble.AppendLine("    get: ");
      Preamble.AppendLine("      return _output;");

      Preamble.AppendLine("  override def Render(textWriter as System.IO.TextWriter):");
      Preamble.AppendLine("    Output.TextWriter = textWriter");
      Preamble.AppendLine("    super(textWriter)");
    }

    public string IndentString
    {
      get { return new string(' ', 4 + ((Depth < 0 ? 0 : Depth) * 2)); }
    }

    public override void AppendOutput(string value, bool newLine)
    {
      if (value == null)
      {
        return;
      }

      var method = newLine ? "WriteLine" : "Write";

      if (!_disableOutputIndentationShrink && Depth > 0)
      {
        if (value.StartsWith(string.Empty.PadLeft(Depth * 2), StringComparison.Ordinal))
        {
          value = value.Remove(0, Depth * 2);
        }
      }

      // prevents problems with " at the end of the string
      value = value.Replace("\"", "\"\"\"+'\"'+\"\"\"");

      Output.AppendLine(Utility.FormatInvariant(IndentString + "textWriter.{0}(\"\"\"{1}\"\"\")", method, value));
    }

    public override void AppendCode(string code, bool newLine)
    {
      if (code != null)
      {
        var action = newLine ? "WriteLine" : "Write";

        Output.AppendLine(Utility.FormatInvariant(IndentString + "textWriter.{0}(Convert.ToString({1}))", action, code));
      }
    }

    public override void AppendChangeOutputDepth(int depth)
    {
      AppendChangeOutputDepth(depth, false);
    }

    public void AppendChangeOutputDepth(int depth,
      bool disableOutputIndentationShrink)
    {
      _disableOutputIndentationShrink = disableOutputIndentationShrink;

      if (BlockDepth != depth)
      {
        Output.AppendLine(IndentString + "Output.Depth = " + depth);
        BlockDepth = depth;
      }
    }

    public override void AppendSilentCode(string code, bool closeStatement)
    {
      if (code == null)
      {
        return;
      }

      Output.Append(IndentString + code.Trim());

      if (!closeStatement && !code.EndsWith(":", StringComparison.OrdinalIgnoreCase))
      {
        Output.Append(":");
      }

      Output.AppendLine();
    }

    public override void AppendAttributeCode(string name, string code)
    {
      var varName = "a" + AttributeCount++;

      Output.AppendLine(IndentString + varName + "=Convert.ToString(" + code + ")");
      Output.AppendLine(IndentString + "unless string.IsNullOrEmpty(" + varName + "):");
      BeginCodeBlock();
      AppendOutput(name + "=\"");
      AppendSilentCode("textWriter.Write(" + varName + ")", true);
      AppendOutput("\"");
      EndCodeBlock();
    }

    public override void BeginCodeBlock()
    {
      Depth++;
    }

    public override void EndCodeBlock()
    {
      Depth--;
    }

    public override void AppendPreambleCode(string code)
    {
      Preamble.AppendLine(new string(' ', 4) + (code).Trim());
    }

    public override string Build()
    {
      Preamble.Append(Output);

      return Preamble.ToString();
    }

    private static string MakeBaseTypeName(Type baseType, params Type[] genericArguments)
    {
      if ((genericArguments != null) && (genericArguments.Length > 0))
      {
        baseType = baseType.MakeGenericType(genericArguments);
      }

      var tname = baseType.FullName.Replace('+', '.');

      if (baseType.IsGenericType)
      {
        tname = tname.Substring(0, tname.IndexOf('`'));
        tname += "[";

        var parameters = new List<string>();

        foreach (var t in baseType.GetGenericArguments())
        {
          parameters.Add(MakeBaseTypeName(t, null));
        }

        tname += string.Join(",", parameters.ToArray());

        tname += "]";
      }

      return tname;
    }
  }
}