namespace NHaml.BackEnds
{
  public interface ITemplateClassBuilder
  {
    string ClassName { get; }
    void AppendOutput(string value);
    void AppendOutputLine(string value);
    void AppendOutput(string value, bool newLine);
    void AppendCode(string code);
    void AppendCode(string code, bool newLine);
    void AppendCodeLine(string code);
    void AppendChangeOutputDepth(int depth);
    void AppendSilentCode(string code, bool closeStatement);
    void AppendAttributeCode(string name, string code);
    void BeginCodeBlock();
    void EndCodeBlock();
    void AppendPreambleCode(string code);
    string Build();
  }
}