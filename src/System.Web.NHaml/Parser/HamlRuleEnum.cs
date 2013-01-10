namespace System.Web.NHaml.Parser
{
    public enum HamlRuleEnum
    {
        Unknown = 0,
        PlainText,
        Tag,
        DivId,
        DivClass,
        DocType,
        HtmlComment,
        HamlComment,
        Evaluation,
        Code,
        Partial,
        Document
    }
}
