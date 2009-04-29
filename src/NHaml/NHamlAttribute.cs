namespace NHaml
{
    public class NHamlAttribute
    {
        public NHamlAttribute(string schema, string name, string value, NHamlAttributeType type)
        {
            Schema = schema;
            Name = name;
            Value = value;
            Type = type;
        }

        public string Schema { get; protected set; }
        public string Name { get; protected set; }
        public string Value { get; protected set; }
        public NHamlAttributeType Type { get; protected set; }
    }
}