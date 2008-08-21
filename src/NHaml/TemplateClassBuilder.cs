using System;
using System.Collections.Generic;
using System.Text;

using NHaml.Utils;

namespace NHaml
{
  public sealed class TemplateClassBuilder
  {
    private readonly StringBuilder _preamble = new StringBuilder();
    private readonly StringBuilder _output = new StringBuilder();

    private readonly string _className;

    private int _depth;
    private int _blockDepth;

    private int _attributeCount;

    public TemplateClassBuilder(TemplateCompiler templateCompiler,
      string className, params Type[] genericArguments)
    {
      _className = className;

      _preamble.AppendLine(
        Utility.FormatInvariant("public class {0} : {1} {{readonly TemplateOutputWriter _output = new TemplateOutputWriter();", _className,
          MakeBaseTypeName(templateCompiler.ViewBaseType, genericArguments)));

      _preamble.AppendLine("protected override TemplateOutputWriter Output {get{return _output;}}");
      _preamble.AppendLine("public override void Render(TextWriter textWriter){");
      _preamble.AppendLine("_output.TextWriter = textWriter;");
      _preamble.AppendLine("base.Render(textWriter);");
    }

    public string ClassName
    {
      get { return _className; }
    }

    public void AppendOutput(string value)
    {
      AppendOutput(value, false);
    }

    public void AppendOutputLine(string value)
    {
      AppendOutputInternal(value, "WriteLine");
    }

    public void AppendOutput(string value, bool newLine)
    {
      AppendOutputInternal(value, newLine ? "WriteLine" : "Write");
    }

    private void AppendOutputInternal(string value, string method)
    {
      if (value != null)
      {
        value = value.Replace("\"", "\"\"");

        if (_depth > 0)
        {
          if (value.StartsWith(string.Empty.PadLeft(_depth * 2), StringComparison.Ordinal))
          {
            value = value.Remove(0, _depth * 2);
          }
        }

        _output.AppendLine("textWriter." + method + "(@\"" + value + "\");");
      }
    }

    public void AppendCodeLine(string code)
    {
      AppendCode(code, true);
    }

    public void AppendCode(string code)
    {
      AppendCode(code, false);
    }

    public void AppendCode(string code, bool newLine)
    {
      if (code != null)
      {
        var action = newLine ? "WriteLine" : "Write";
        _output.AppendLine("textWriter." + action + "(Convert.ToString(" + code + "));");
      }
    }

    public void AppendSilentCode(string code, int depth)
    {
      if (_blockDepth != depth)
      {
        _output.AppendLine("_output.Depth = " + depth + ";");
        _blockDepth = depth;
      }

      AppendSilentCode(code, false);
    }

    public void AppendSilentCode(string code, bool appendSemicolon)
    {
      if (code != null)
      {
        code = code.Trim();

        if (appendSemicolon && !code.EndsWith(";", StringComparison.Ordinal))
        {
          code += ';';
        }

        _output.AppendLine(code);
      }
    }

    public void AppendAttributeCode(string name, string code)
    {
      var varName = "a" + _attributeCount++;

      AppendSilentCode("string " + varName + "=Convert.ToString(" + code + ")", true);
      AppendSilentCode("if (!string.IsNullOrEmpty(" + varName + ")){", false);
      AppendOutput(name + "=\"");
      _output.AppendLine("textWriter.Write(" + varName + ");");
      AppendOutput("\"");
      AppendSilentCode("}", false);
    }

    public void BeginCodeBlock()
    {
      _depth++;
      _output.AppendLine("{");
    }

    public void EndCodeBlock(string ending)
    {
      _output.AppendLine(ending);
      _depth--;
    }

    public void AppendPreamble(string code)
    {
      _preamble.AppendLine(code + ';');
    }

    public string Build()
    {
      _output.Append("}}");

      _preamble.Append(_output);

      return _preamble.ToString();
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