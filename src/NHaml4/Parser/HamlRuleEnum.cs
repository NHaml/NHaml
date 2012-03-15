using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NHaml4.Parser
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
        Code
    }
}
