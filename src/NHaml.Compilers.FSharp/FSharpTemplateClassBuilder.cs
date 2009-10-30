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
        public override void AppendPreambleCode(string code)
        {
             RenderMethod.Statements.Insert(PreambleCount, new CodeSnippetStatement(code));
            PreambleCount++;
        }

        public override void AppendSilentCode(string code, bool closeStatement)
        {
            if (code != null)
            {
                code = code.Trim();


                RenderMethod.Statements.Add(new CodeSnippetStatement
                                            {
                                                Value = code,
                                            });
            }

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

    
    }
}