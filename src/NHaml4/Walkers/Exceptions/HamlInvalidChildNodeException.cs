using System;

namespace NHaml4.Walkers.Exceptions
{
    [Serializable]
    public class HamlInvalidChildNodeException : Exception
    {
        public HamlInvalidChildNodeException(Type nodeType, Type childType, int lineNo)
            : this(nodeType, childType, lineNo, null)
        { }

        private HamlInvalidChildNodeException(Type nodeType, Type childType, int lineNo, Exception ex)
            : base(string.Format("Node '{0}' has invalid child node {1} on line {2}", nodeType.FullName, childType.FullName, lineNo), ex)
        { }
    }
}
