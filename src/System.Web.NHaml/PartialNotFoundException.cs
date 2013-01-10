namespace System.Web.NHaml
{
    [Serializable]
    public class PartialNotFoundException : Exception
    {
        public PartialNotFoundException(string fileName)
            : base("Unable to find partial : " + fileName)
        { }
    }
}
