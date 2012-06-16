﻿namespace NHaml4.Parser.Rules
{
    public class HamlNodeHamlComment : HamlNode
    {
        public HamlNodeHamlComment(IO.HamlLine nodeLine)
            : base(nodeLine)
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return false; }
        }
    }
}
