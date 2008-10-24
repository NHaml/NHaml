using System;
using System.Diagnostics;

using NHaml.Backends.CSharp2;
using NHaml.Backends.CSharp3;

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
        _templateCompiler.CompilerBackend = new CSharp2CompilerBackend();

      var stopwatch = new Stopwatch();

      stopwatch.Start();

      for (var i = 0; i < 100; i++)
      {
        _templateCompiler.Compile(
          TemplatesFolder + "AttributeEval.haml",
          TemplatesFolder + "AttributeEval.haml");
      }

      stopwatch.Stop();

      Console.WriteLine(stopwatch.ElapsedMilliseconds);
    }

    [Test]
    public void CS3AttributeCompilationPerformance()
    {
        _templateCompiler.CompilerBackend = new CSharp3CompilerBackend();

      var stopwatch = new Stopwatch();

      stopwatch.Start();

      for (var i = 0; i < 100; i++)
      {
        _templateCompiler.Compile(
          TemplatesFolder + "AttributeEval.haml",
          TemplatesFolder + "AttributeEval.haml");
      }

      stopwatch.Stop();

      Console.WriteLine(stopwatch.ElapsedMilliseconds);
    }
  }
}