using System;

namespace NHaml4
{
    [Serializable]
    public class PartialNotFoundException : Exception
    {
        public PartialNotFoundException(string fileName)
            : base("Unable to find partial : " + fileName)
        { }
    }
}
