using System.CodeDom;

namespace NHaml.Compilers.FSharp
{
    internal  class FSharpTemplateClassBuilder : CodeDomClassBuilder
    {

        public FSharpTemplateClassBuilder(string className)
            : base(className)
        {
        }

 
        protected override void RenderBeginBlock()
        {
            RenderMethod.Statements.Add(new CodeSnippetStatement
            {
                Value = " (",
            });
        }


        protected override void RenderEndBlock()
        {
            RenderMethod.Statements.Add(new CodeSnippetStatement
            {
                Value = " )",
            });
        }

        public override string Build(System.Collections.Generic.IList<string> imports)
        {
            var build = base.Build(imports);
            //WTF??
            build =build.Replace("|> ignore", string.Empty);
            build =build.Replace(@"
                                                                                                                ", string.Empty);
            
            return build;
        }

    
        public override void AppendOutput(string value, bool newLine)
        {
            //WTF
            if (value != null)
            {
                value = value.Replace("\"", "\\\"");
            }
            base.AppendOutput(value, newLine);
        }
    }
}