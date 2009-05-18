using System;
using System.CodeDom;
using Microsoft.FSharp.Compiler.CodeDom;

namespace NHaml.Compilers.FSharp
{
    internal sealed class FSharpTemplateClassBuilder : CodeDomClassBuilder
    {

        public FSharpTemplateClassBuilder(string className, Type baseType)
            : base(className, baseType, new FSharpCodeProvider())
        {
        }

        public string IndentString
        {
            get { return new string(' ', 4 + ((Depth < 0 ? 0 : Depth) * 2)); }
        }
      


        protected override void RenderBeginBlock()
        {
            RenderMethod.Statements.Add(new CodeSnippetStatement
            {
                Value = IndentString + "(",
            });
        }


        protected override void RenderEndBlock()
        {
            RenderMethod.Statements.Add(new CodeSnippetStatement
            {
                Value = IndentString + ")",
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