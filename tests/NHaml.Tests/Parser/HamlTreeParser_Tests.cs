using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Parser;
using NHaml4.TemplateResolution;
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
            HamlTreeParser parser = new HamlTreeParser(new HamlFileLexer());
            var layoutViewSources = new List<IViewSource> { ViewSourceBuilder.Create("Test") };
            var result = parser.ParseDocument(layoutViewSources);
            Assert.IsInstanceOf(typeof(HamlDocument), result);
        }

        [Test]
        [TestCase("Test content", typeof(HamlNodeText))]
        public void Parse_DifferentLineTypes_CreatesCorrectTreeNodeTypes(string source, Type nodeType)
        {
            HamlTreeParser parser = new HamlTreeParser(new HamlFileLexer());
            var layoutViewSources = new List<IViewSource> { ViewSourceBuilder.Create(source) };
            var result = parser.ParseDocument(layoutViewSources);
            Assert.IsInstanceOf(nodeType, result.Children[0]);
        }

        [Test]
        [TestCase("", 0)]
        [TestCase("Test", 1)]
        [TestCase("Test\nTest", 2)]
        public void Parse_SingleLevelTemplates_TreeContainsCorrectNoOfChildren(string template, int expectedChildren)
        {
            HamlTreeParser parser = new HamlTreeParser(new HamlFileLexer());
            var layoutViewSources = new List<IViewSource> { ViewSourceBuilder.Create(template) };
            var result = parser.ParseDocument(layoutViewSources);
            Assert.AreEqual(expectedChildren, result.Children.Count);
        }

        [Test]
        [TestCase("Test\n  Test", 1)]
        [TestCase("Test\n  Test\n  Test", 1)]
        [TestCase("Test\n  Test\n    Test", 1)]
        [TestCase("Test\n  Test\nTest", 2)]
        public void Parse_MultiLevelTemplates_TreeContainsCorrectNoChildren(string template, int expectedChildren)
        {
            HamlTreeParser parser = new HamlTreeParser(new HamlFileLexer());
            var layoutViewSources = new List<IViewSource> { ViewSourceBuilder.Create(template) };
            var result = parser.ParseDocument(layoutViewSources);
            Assert.AreEqual(expectedChildren, result.Children.Count);
        }
    }
}
