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
            _templateEngine.TemplateCompiler = new CSharp2TemplateCompiler();

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for( var i = 0; i < 100; i++ )
            {
                _templateEngine.Compile( TemplatesFolder + @"CSharp2\AttributeEval.haml" );
            }

            stopwatch.Stop();

            Console.WriteLine( stopwatch.ElapsedMilliseconds );
        }

        [Test]
        public void CS3AttributeCompilationPerformance()
        {
            _templateEngine.TemplateCompiler = new CSharp3TemplateCompiler();

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for( var i = 0; i < 100; i++ )
            {
                _templateEngine.Compile( TemplatesFolder + @"CSharp2\AttributeEval.haml" );
            }

            stopwatch.Stop();

            Console.WriteLine( stopwatch.ElapsedMilliseconds );
        }
    }
}