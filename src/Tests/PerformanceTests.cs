using System;
using System.Diagnostics;

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
      _templateCompiler.CompilerVersion = "2.0";

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
      _templateCompiler.CompilerVersion = "3.5";

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