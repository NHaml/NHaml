using System;
using System.Diagnostics;

using NHaml.Compilers.CSharp2;
using NHaml.Compilers.CSharp3;

using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    [Ignore]
    public class PerformanceTests : TestFixtureBase
    {
        [Test]
        public void CS2AttributeCompilationPerformance()
        {
            _templateEngine.Options.TemplateCompiler = new CSharp2TemplateCompiler();

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var resources = new TemplateCompileResources(_templateEngine.Options.TemplateBaseType,
                TemplatesFolder + @"CSharp2\AttributeEval.haml");

            for( var i = 0; i < 100; i++ )
            {
                _templateEngine.GetCompiledTemplate( resources );
            }

            stopwatch.Stop();

            Console.WriteLine( stopwatch.ElapsedMilliseconds );
        }

        [Test]
        public void CS3AttributeCompilationPerformance()
        {
            _templateEngine.Options.TemplateCompiler = new CSharp3TemplateCompiler();

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var resources = new TemplateCompileResources(_templateEngine.Options.TemplateBaseType,
                TemplatesFolder + @"CSharp2\AttributeEval.haml");

            for( var i = 0; i < 100; i++ )
            {
                _templateEngine.GetCompiledTemplate( resources );
            }

            stopwatch.Stop();

            Console.WriteLine( stopwatch.ElapsedMilliseconds );
        }
    }
}