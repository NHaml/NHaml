using System;
using System.Collections.Generic;
using System.Text;

using NHaml.Utilities;

namespace NHaml
{
  public sealed class TemplateClassBuilder
  {
    private readonly StringBuilder _preamble = new StringBuilder();
    private readonly StringBuilder _output = new StringBuilder();

    private readonly string _className;

    private int _depth;

    public TemplateClassBuilder(TemplateCompiler templateCompiler, string className, params Type[] genericArguments)
    {
      _className = className;

      _preamble.AppendLine(
        "public class {0} : {1}, ICompiledTemplate {{"
          .FormatInvariant(_className,
            MakeBaseTypeName(templateCompiler.ViewBaseType, genericArguments)));

      _preamble.AppendLine("public void Render(TextWriter writer){");
    }

    public string ClassName
    {
      get { return _className; }
    }

    public static string MakeBaseTypeName(Type baseType, params Type[] genericArguments)
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

    public void AppendOutput(string value)
    {
      AppendOutput(value, false);
    }

    public void AppendOutput(string value, bool newLine)
    {
      AppendOutputInternal(value, newLine ? "WriteLine" : "Write");
    }

    public void AppendOutputLine(string value)
    {
      AppendOutputInternal(value, "WriteLine");
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

        _output.AppendLine("writer." + method + "(@\"" + value + "\");");
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
        _output.AppendLine("writer." + action + "(Convert.ToString(" + code + "));");
      }
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

    public void BeginCodeBlock()
    {
      _depth++;
      _output.AppendLine("{");
    }

    public void EndCodeBlock()
    {
      _output.AppendLine("}");
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
  }
}