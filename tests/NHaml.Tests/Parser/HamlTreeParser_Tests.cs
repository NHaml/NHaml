using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml.Parser;
using NHaml.TemplateResolution;
using NHaml.Tests.Builders;
using NHaml.IO;

namespace NHaml.Tests
{
    [TestFixture]
    public class HamlTreeParser_Tests
    {
        [Test]
        public void Parse_SingleLineTemplate_ReturnsHamlTree()
        {
            HamlTreeParser parser = new HamlTreeParser(new HamlFileReader());
            var layoutViewSources = new List<IViewSource> { ViewSourceBuilder.Create(".className Test") };
            var result = parser.ParseDocument(layoutViewSources);
            Assert.IsInstanceOf(typeof(HamlDocument), result);
        }

        [Test]
        [TestCase("", 0)]
        [TestCase("Test", 1)]
        [TestCase("Test\nTest", 2)]
        public void Parse_SingleLevelTemplates_TreeContainsCorrectNoOfChildren(string template, int expectedChildren)
        {
            HamlTreeParser parser = new HamlTreeParser(new HamlFileReader());
            var layoutViewSources = new List<IViewSource> { ViewSourceBuilder.Create(template) };
            var result = parser.ParseDocument(layoutViewSources);
            Assert.AreEqual(expectedChildren, result.Children.Count);
        }

        [Test]
        [TestCase("Test\n  Test", 1)]
        public void Parse_MultiLevelTemplates_TreeContainsCorrectNoChildren(string template, int expectedChildren)
        {
            HamlTreeParser parser = new HamlTreeParser(new HamlFileReader());
            var layoutViewSources = new List<IViewSource> { ViewSourceBuilder.Create(template) };
            var result = parser.ParseDocument(layoutViewSources);
            Assert.AreEqual(expectedChildren, result.Children.Count);
        }
    }
}
