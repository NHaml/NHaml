namespace NHaml
{
    public class ParsedAttribute
    {
        public ParsedAttribute(string schema, string name, string value, ParsedAttributeType type)
        {
            Schema = schema;
            Name = name;
            Value = value;
            Type = type;
        }

        public string Schema { get; protected set; }
        public string Name { get; protected set; }
        public string Value { get; protected set; }
        public ParsedAttributeType Type { get; protected set; }
    }
}