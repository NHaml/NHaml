namespace NHaml
{
    public class ParsedAttribute
    {
        public ParsedAttribute()
        {
            //Tokens = new List<ExpressionStringToken>();
        }
        public string Schema { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public ParsedAttributeType Type { get; set; }

       // List<ExpressionStringToken> Tokens { get; set; }
    }
}