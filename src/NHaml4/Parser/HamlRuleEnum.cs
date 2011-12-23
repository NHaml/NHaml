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

        //[Signifier("")]
        PlainText,

        //[Signifier("%")]
        Tag,

        //[Signifier("#")]
        DivId,

        //[Signifier(".")]
        DivClass,

        //[Signifier("!!!")]
        DocType,

        //[Signifier("/")]
        HtmlComment,

        //[Signifier("-#")]
        HamlComment,

        //[Signifier("=")]
        Evaluation,
    }
}
