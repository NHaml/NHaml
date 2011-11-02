using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HamlSpec
{
    internal class HamlSpec
    {
        public string GroupName { get; set; }
        public string TestName { get; set; }
        public string Haml { get; set; }
        public string ExpectedHtml { get; set; }

        public bool HasAConfigBlock { get; set; }
    }
}
