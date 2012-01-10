using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Crosscutting;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeHtmlAttributeCollection : HamlNode
    {
        public HamlNodeHtmlAttributeCollection(string attributeCollection)
        {
            Content = attributeCollection;
            ParseChildren(attributeCollection);
        }

        private void ParseChildren(string attributeCollection)
        {
            int index = 1;
            while (index < attributeCollection.Length)
            {
                string nameValuePair = GetNextAttributeToken(attributeCollection, ref index);
                if (!string.IsNullOrEmpty(nameValuePair))
                    Add(new HamlNodeHtmlAttribute(nameValuePair));
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
