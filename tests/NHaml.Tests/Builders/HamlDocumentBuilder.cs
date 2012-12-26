using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.NHaml.Parser;

namespace NHaml.Tests.Builders
{
    public static class HamlDocumentBuilder
    {
        public static HamlDocument Create()
        {
            return Create("");
        }

        public static HamlDocument Create(string documentName, params HamlNode[] nodes)
        {
            var document = new HamlDocument(documentName);
            foreach (var node in nodes)
                document.AddChild(node);
            return document;
        }
    }
}
