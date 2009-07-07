namespace NHaml
{
    public class ParsedAttribute
    {
        public ParsedAttribute()
        {
        }

        public ParsedAttribute(string schema, string name, string value, ParsedAttributeType type)
        {
            Schema = schema;
            Name = name;
            Value = value;
            Type = type;
        }

        public string Schema { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public ParsedAttributeType Type { get; set; }
    }
}