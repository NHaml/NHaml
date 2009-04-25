using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace NHaml
{
    public class AttributeParser
    {
        private readonly string attributes;
        XmlReader reader;


        // So at the moment, I'm leaning towards {foo="bar"} or {foo=bar} for
        // static attributes and {foo=#{bar}} for dynamic ones. Thoughts?
        public AttributeParser(string attributes)
        {
            this.attributes = attributes.TrimStart();
        }

        public Dictionary<string, string> Values { get; set; }

        public void Parse()
        {
            Values = new Dictionary<string, string>();
            reader = XmlReader.Create(new StringReader(string.Format("<node {0}/>", attributes)));
            reader.MoveToContent();
            while (reader.MoveToNextAttribute())
            {
                Values.Add(reader.Name, reader.Value);
            }
        }

    }
}
