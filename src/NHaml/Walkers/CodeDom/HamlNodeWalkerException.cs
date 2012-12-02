using System;

namespace NHaml.Walkers.CodeDom
{
    class HamlNodeWalkerException : Exception
    {
        public HamlNodeWalkerException(string hamlNodeType, int lineNo, Exception e)
            : base(
                string.Format("Exception occurred walking {0} HamlNode on line {1}.", hamlNodeType, lineNo),
                e)
        { }
    }
}
