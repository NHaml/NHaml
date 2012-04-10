using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Parser;

namespace NHaml4.IO
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
            else if (content.StartsWith("-#"))
            {
                content = content.Substring(2);
                return HamlRuleEnum.HamlComment;
            }
            else if (content.StartsWith("%"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.Tag;
            }
            else if (content.StartsWith("."))
            {
                return HamlRuleEnum.DivClass;
            }
            else if (content.StartsWith("#"))
            {
                return HamlRuleEnum.DivId;
            }
            else if (content.StartsWith("/"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.HtmlComment;
            }
            else if (content.StartsWith("="))
            {
                content = content.Substring(1);
                return HamlRuleEnum.Evaluation;
            }
            else if (content.StartsWith("-"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.Code;
            }
            else if (content.StartsWith(@"\"))
            {
                content = content.Substring(1);
                return HamlRuleEnum.PlainText;
            }
            return HamlRuleEnum.PlainText;
        }
    }
}
