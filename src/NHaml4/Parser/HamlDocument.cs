using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;
using NHaml4.Parser.Rules;

namespace NHaml4.Parser
{
    public class HamlDocument : HamlNode
    {
        public HamlDocument(string fileName)
            : base(0, fileName)
        { }

        public override bool IsContentGeneratingTag
        {
            get { return false; }
        }

        public string FileName
        {
            get { return Content; }
        }
    }
}
