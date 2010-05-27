using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Core.Visitors;
using NHaml.Core.Ast;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace NHaml.Core.Tests
{
    public class DebugCompiler
    {
        private CodeDomVisitor Visitor;

        public Dictionary<string, string> Locals = new Dictionary<string, string>();

        public DebugCompiler(DocumentNode n, string format)
        {
            Visitor = new DebugCodeDomVisitor() { Format = format };
            Visitor.Visit(n);
        }

        public string Run()
        {
            var compileUnit = new CodeCompileUnit();

            var testNamespace = new CodeNamespace("TestNamespace");
            testNamespace.Imports.Add(new CodeNamespaceImport("System"));
            testNamespace.Imports.Add(new CodeNamespaceImport("System.Web"));
            compileUnit.Namespaces.Add(testNamespace);
            var options = new CodeGeneratorOptions();
            var declaration = new CodeTypeDeclaration
            {
                Name = "Test",
                IsClass = true,
                TypeAttributes = TypeAttributes.Public
            };

            foreach (var local in Locals)
            {
                declaration.Members.Add(new CodeMemberField(
                        new CodeTypeReference(typeof(string)),
                        local.Key) {
                        InitExpression = new CodePrimitiveExpression(local.Value)
                    }
                );
            }

            declaration.Members.Add(Visitor.Methods["Main"]);
            testNamespace.Types.Add(declaration);

            var unit = new CodeCompileUnit();
            unit.Namespaces.Add(testNamespace);

            var cp = new CSharpCodeProvider();
            var compilerParams = new CompilerParameters();
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Web.dll");
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = false;
            CompilerResults cr = cp.CompileAssemblyFromDom(compilerParams, unit);
            if (cr.Errors.Count>0)
            {
                throw new Exception(cr.Errors[0].ToString());
            }
            var assembly = cr.CompiledAssembly;
            Type runner = assembly.GetType("TestNamespace.Test");


            object runnerobject = Activator.CreateInstance(runner);
            StringWriter sw = new StringWriter();
            //cp.GenerateCodeFromCompileUnit(unit, sw, new CodeGeneratorOptions());
            runner.InvokeMember("RenderMain", BindingFlags.InvokeMethod, null, runnerobject, new object[] { sw });
            return sw.GetStringBuilder().ToString();
        }

        internal class DebugCodeDomVisitor : CodeDomVisitor
        {
            protected override CodeObject StartBlock { get { return new CodeSnippetExpression("{//"); } }
            protected override CodeObject EndBlock { get { return new CodeSnippetExpression("}//"); } }
            protected override string Comment { get { return "//"; } }
            protected override CodeObject LambdaEndBlock
            {
                get { throw new NotImplementedException(); }
            }

            protected override bool SupportLambda
            {
                get { return false; }
            }

            protected override System.Text.RegularExpressions.Regex LambdaRegex
            {
                get { throw new NotImplementedException(); }
            }

            protected override string TranslateLambda(string codeLine, System.Text.RegularExpressions.Match lambdaMatch)
            {
                throw new NotImplementedException();
            }
        }
    }
}
