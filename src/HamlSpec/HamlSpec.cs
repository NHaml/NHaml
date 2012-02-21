using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HamlSpec
{
    internal class HamlSpec
    {
        private IDictionary<string, object> _locals = new Dictionary<string,object>();

        public string Format { get; set; }
        public string GroupName { get; set; }
        public string TestName { get; set; }
        public string Haml { get; set; }
        public string ExpectedHtml { get; set; }
        public bool HasAConfigBlock { get; set; }
        public IDictionary<string, object> Locals {
            get { return _locals; }
            set { _locals = value; }
        }
    }
}
