﻿namespace NHaml4.Parser.Rules
{
    public class HamlNodeTagId : HamlNode
    {
        public HamlNodeTagId(int sourceFileLineNo, string tagId)
            : base(sourceFileLineNo, tagId)
        { }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }
    }
}
