using System;
using System.Collections.Generic;

using NHaml.Utils;

namespace NHaml.Backends.CSharp2
{
  public sealed class CSharp2TemplateClassBuilder : TemplateClassBuilderBase
  {
    public CSharp2TemplateClassBuilder(Type viewBaseType,
      string className, params Type[] genericArguments)
      : base(className)
    {
      Preamble.AppendLine(
        Utility.FormatInvariant("public class {0} : {1} {{readonly TemplateOutputWriter _output = new TemplateOutputWriter();",
          ClassName,
          MakeBaseTypeName(viewBaseType, genericArguments)));

      Preamble.AppendLine("protected override TemplateOutputWriter Output {get{return _output;}}");
      Preamble.AppendLine("public override void Render(TextWriter textWriter){");
      Preamble.AppendLine("Output.TextWriter = textWriter;");
      Preamble.AppendLine("base.Render(textWriter);");
    }

    public override void AppendOutput(string value, bool newLine)
    {
      if (value == null)
      {
        return;
      }

      var method = newLine ? "WriteLine" : "Write";

      value = value.Replace("\"", "\"\"");

      if (Depth > 0)
      {
        if (value.StartsWith(string.Empty.PadLeft(Depth * 2), StringComparison.Ordinal))
        {
          value = value.Remove(0, Depth * 2);
        }
      }

      Output.AppendLine("textWriter." + method + "(@\"" + value + "\");");
    }

    public override void AppendCode(string code, bool newLine)
    {
      if (code != null)
      {
        var action = newLine ? "WriteLine" : "Write";
        Output.AppendLine("textWriter." + action + "(Convert.ToString(" + code + "));");
      }
    }

    public override void AppendChangeOutputDepth(int depth)
    {
      if (BlockDepth != depth)
      {
        Output.AppendLine("Output.Depth = " + depth + ";");
        BlockDepth = depth;
      }
    }

    public override void AppendSilentCode(string code, bool closeStatement)
    {
      if (code != null)
      {
        code = code.Trim();

        if (closeStatement && !code.EndsWith(";", StringComparison.Ordinal))
        {
          code += ';';
        }

        Output.AppendLine(code);
      }
    }

    public override void AppendAttributeCode(string name, string code)
    {
      var varName = "a" + AttributeCount++;

      AppendSilentCode("string " + varName + "=Convert.ToString(" + code + ")", true);
      AppendSilentCode("if (!string.IsNullOrEmpty(" + varName + ")){", false);
      AppendOutput(name + "=\"");
      Output.AppendLine("textWriter.Write(" + varName + ");");
      AppendOutput("\"");
      AppendSilentCode("}", false);
    }

    public override void BeginCodeBlock()
    {
      Depth++;
      Output.AppendLine("{");
    }

    public override void EndCodeBlock()
    {
      Output.AppendLine("}");
      Depth--;
    }

    public override void AppendPreambleCode(string code)
    {
      Preamble.AppendLine(code + ';');
    }

    public override string Build()
    {
      Output.Append("}}");

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
        tname += "<";

        var parameters = new List<string>();

        foreach (var t in baseType.GetGenericArguments())
        {
          parameters.Add(MakeBaseTypeName(t, null));
        }

        tname += string.Join(",", parameters.ToArray());

        tname += ">";
      }

      return tname;
    }
  }
}