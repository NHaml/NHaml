using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Crosscutting;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeHtmlAttributeCollection : HamlNode
    {
        public HamlNodeHtmlAttributeCollection(int sourceFileLineNo, string attributeCollection)
            : base(sourceFileLineNo, attributeCollection)
            
        {
            ParseChildren(attributeCollection);
        }

        public override bool IsContentGeneratingTag
        {
            get { return true; }
        }

        private void ParseChildren(string attributeCollection)
        {
            if (Content[0] != '(')
                throw new HamlMalformedTagException("AttributeCollection tag must start with an opening bracket.", SourceFileLineNum);

            int index = 1;
            while (index < attributeCollection.Length)
            {
                string nameValuePair = GetNextAttributeToken(attributeCollection, ref index);
                if (!string.IsNullOrEmpty(nameValuePair))
                    AddChild(new HamlNodeHtmlAttribute(SourceFileLineNum, nameValuePair));
                index++;
            }
        }

        private static string GetNextAttributeToken(string attributeCollection, ref int index)
        {
            char[] terminatingChars = new[] { ' ', '\t', ')' };
            string nameValuePair = HtmlStringHelper.ExtractTokenFromTagString(attributeCollection, ref index,
                terminatingChars);
            if (terminatingChars.Contains(nameValuePair[nameValuePair.Length - 1]))
                nameValuePair = nameValuePair.Substring(0, nameValuePair.Length - 1);
            return nameValuePair;
        }
    }
}
