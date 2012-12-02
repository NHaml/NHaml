using NHaml.Parser;

namespace NHaml.IO
{
    public static class HamlRuleFactory
    {
        public static HamlRuleEnum ParseHamlRule(ref string content)
        {
            if (content == "") return HamlRuleEnum.PlainText;

            if (content.StartsWith("!!!"))
            {
                content = content.Substring(3);
                return HamlRuleEnum.DocType;
            }
            if (content.StartsWith("-#"))
            {
                content = content.Substring(2);
                return HamlRuleEnum.HamlComment;
            }
            if (content.StartsWith("#{"))
            {
                content = content.Substring(0);
                return HamlRuleEnum.PlainText;
            }
            if (content.StartsWith("\\\\"))
            {
                content = content.Substring(0);
                return HamlRuleEnum.PlainText;
            }
            if (content.StartsWith("\\#"))
            {
                content = content.Substring(0);
                return HamlRuleEnum.PlainText;
            }
            if (content.StartsWith("%"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.Tag;
            }
            if (content.StartsWith("."))
            {
                return HamlRuleEnum.DivClass;
            }
            if (content.StartsWith("#"))
            {
                return HamlRuleEnum.DivId;
            }
            if (content.StartsWith("/"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.HtmlComment;
            }
            if (content.StartsWith("="))
            {
                content = content.Substring(1);
                return HamlRuleEnum.Evaluation;
            }
            if (content.StartsWith("-"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.Code;
            }
            if (content.StartsWith(@"\"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.PlainText;
            }
            if (content.StartsWith("_"))
            {
                content = content.Substring(1).Trim();
                return HamlRuleEnum.Partial;
            }
            return HamlRuleEnum.PlainText;
        }
    }
}
