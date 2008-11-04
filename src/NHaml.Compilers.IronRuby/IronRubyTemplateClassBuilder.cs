using System;

using NHaml.Utils;

namespace NHaml.Compilers.IronRuby
{
  internal sealed class IronRubyTemplateClassBuilder : TemplateClassBuilder
  {
    public IronRubyTemplateClassBuilder(string className, Type templateBaseType)
      : base(className)
    {
      Preamble.AppendLine("class " + className
        + "<" + Utility.MakeBaseClassName(templateBaseType, '[', ']', "::"));

      Preamble.AppendLine("def __a(as)");
      Preamble.AppendLine("as.collect { |k,v| ");
      Preamble.AppendLine("next unless v");
      Preamble.AppendLine("\"#{k.to_s.gsub('_','-')}=\\\"#{v}\\\"\"");
      Preamble.AppendLine("}.compact.join(' ')");
      Preamble.AppendLine("end");

      Preamble.AppendLine("def CoreRender(text_writer)");
    }

    public override void AppendAttributeCode(string name, string code)
    {
    }

    public override void AppendPreambleCode(string code)
    {
      Preamble.AppendLine(code);
    }

    public override void AppendOutput(string value, bool newLine)
    {
      if (value == null)
      {
        return;
      }

      var method = newLine ? "WriteLine" : "Write";

      if (Depth > 0)
      {
        if (value.StartsWith(string.Empty.PadLeft(Depth * 2), StringComparison.Ordinal))
        {
          value = value.Remove(0, Depth * 2);
        }
      }

      Output.AppendLine("text_writer." + method + "('" + value.Replace("'", "\\'") + "')");
    }

    public override void AppendCode(string code, bool newLine)
    {
      if (code != null)
      {
        var method = newLine ? "WriteLine" : "Write";

        Output.AppendLine("text_writer." + method + "(" + code + ")");
      }
    }

    public override void AppendChangeOutputDepth(int depth)
    {
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

    public override void EndCodeBlock()
    {
      Output.AppendLine("end");

      base.EndCodeBlock();
    }

    public override string Build()
    {
      Output.AppendLine("end;end");

      Preamble.Append(Output);

      return Preamble.ToString();
    }
  }
}