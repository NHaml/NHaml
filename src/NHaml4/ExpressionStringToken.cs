namespace NHaml
{
    public class ExpressionStringToken
    {
        public string Value { get; protected set; }
        public bool IsExpression { get; protected set; }

        public ExpressionStringToken(string value, bool isExpression)
        {
            Value = value;
            IsExpression = isExpression;
        }
    }
}