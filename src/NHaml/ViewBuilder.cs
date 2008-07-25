using System;
using System.Linq;
using System.Text;

using NHaml.Utilities;

namespace NHaml
{
  public sealed class ViewBuilder
  {
    private readonly StringBuilder _preamble = new StringBuilder();
    private readonly StringBuilder _output = new StringBuilder();

    private readonly string _className;

    private int _depth;

    public ViewBuilder(TemplateCompiler templateCompiler, string className, params Type[] genericArguments)
    {
      _className = className;

      _preamble.AppendLine("public class {0} : {1}, ICompiledView {{".FormatInvariant(_className,
        MakeBaseTypeName(templateCompiler.ViewBaseType,
          genericArguments)));
      _preamble.AppendLine("StringBuilder _buffer;");
      _preamble.AppendLine("public string Render(){");
      _preamble.AppendLine("_buffer = new StringBuilder();");
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

        var parameters = from t in baseType.GetGenericArguments()
        select MakeBaseTypeName(t, null);

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
      AppendOutputInternal(value, newLine ? "AppendLine" : "Append");
    }

    public void AppendOutputLine(string value)
    {
      AppendOutputInternal(value, "AppendLine");
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

        _output.AppendLine("_buffer." + method + "(@\"" + value + "\");");
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
        var action = newLine ? "AppendLine" : "Append";
        _output.AppendLine("_buffer." + action + "(Convert.ToString(" + code + "));");
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
      _output.Append("return _buffer.ToString();}}");

      _preamble.Append(_output);

      return _preamble.ToString();
    }
  }
}