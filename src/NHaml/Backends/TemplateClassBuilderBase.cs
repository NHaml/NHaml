using System.Text;

namespace NHaml.Backends
{
  public abstract class TemplateClassBuilderBase : ITemplateClassBuilder
  {
    private readonly string _className;
    private readonly StringBuilder _output = new StringBuilder();
    private readonly StringBuilder _preamble = new StringBuilder();

    protected TemplateClassBuilderBase(string className)
    {
      _className = className;
    }

    protected StringBuilder Output
    {
      get { return _output; }
    }

    protected StringBuilder Preamble
    {
      get { return _preamble; }
    }

    public int Depth { get; set; }

    public int BlockDepth { get; set; }

    public int AttributeCount { get; set; }

    #region ITemplateClassBuilder Members

    public string ClassName
    {
      get { return _className; }
    }

    public abstract void AppendOutput(string value, bool newLine);

    public virtual void AppendOutput(string value)
    {
      AppendOutput(value, false);
    }

    public virtual void AppendCodeLine(string code)
    {
      AppendCode(code, true);
    }

    public virtual void AppendOutputLine(string value)
    {
      AppendOutput(value, true);
    }

    public virtual void AppendCode(string code)
    {
      AppendCode(code, false);
    }

    public virtual void BeginCodeBlock()
    {
      Depth++;
    }

    public virtual void EndCodeBlock()
    {
      Depth--;
    }

    public abstract void AppendCode(string code, bool newLine);

    public abstract void AppendChangeOutputDepth(int depth);

    public abstract void AppendSilentCode(string code, bool closeStatement);

    public abstract void AppendAttributeCode(string name, string code);

    public abstract void AppendPreambleCode(string code);

    public abstract string Build();

    #endregion
  }
}