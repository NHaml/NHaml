using System.Collections.Generic;
using NHaml.Exceptions;

namespace NHaml
{
    public static class AttributeValueParser
    {
        public static List<Item> Parse(string value)
        {
            var values = new List<Item>();
            var splitByCode = SplitByCode(value);
            foreach (var item in splitByCode)
            {
                if (item.StartsWith("#{"))
                {
                    var lastIndexOf = item.LastIndexOf('}');
                    if (lastIndexOf == -1)
                    {
                        throw new SyntaxException(string.Format("Could not find closing right curly bracket '}}'.\r\nAttribute Value:\r\n{0}", value));
                    }
                    var code = item.Substring(2, lastIndexOf-2);
                    values.Add(new Item { IsCode = true, Value = Escape(code) });
                    if (lastIndexOf + 1 < item.Length)
                    {
                        var text = item.Substring(lastIndexOf +1, item.Length- lastIndexOf -1);
                        values.Add(new Item { IsCode = false, Value = Escape(text) });
                    }
                }
                else
                {
                    values.Add(new Item {IsCode = false,Value = Escape(item)});
                }
            }
            return values;
        }

        private static string Escape(string item)
        {
            return item.Replace(@"\#{", "#{");
        }

        public static List<string> SplitByCode(string value)
        {
            var values = new List<string>();
            var current = "";
            for (var index = 0; index < value.Length; index++)
            {
                var startingCode = index+2<=value.Length && value.Substring(index,2) == "#{";
                var previousCharIsEscape = index>0 && value[index-1] == '\\';
                var isFirstChar = index==0;

                if (startingCode && !previousCharIsEscape && !isFirstChar)
                {
                    values.Add(current);
                    current = "";
                }
                var c = value[index];
                current += c;
            }
            values.Add(current);
            return values;
        }

        public class Item
        {
            public string Value{ get; set;}
            public bool IsCode{ get; set;}
        }
    }
}