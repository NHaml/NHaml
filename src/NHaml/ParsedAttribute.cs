namespace NHaml
{
    public class ParsedAttribute
    {
        //public ParsedAttribute()
        //{
        //    Tokens = new List<ExpressionStringToken>();
        //}
        // List<ExpressionStringToken> Tokens { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public ParsedAttributeType Type { get; set; }
    }
}