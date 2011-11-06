using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml.Parser;
using NHaml.TemplateResolution;
using NHaml.Tests.Builders;

namespace NHaml.Tests
{
    [TestFixture]
    public class HamlTreeParser_Tests
    {
        [Test]
        public void Parse_SingleLineTemplate_ReturnsHamlTree()
        {
            HamlTreeParser parser = new HamlTreeParser();
            var layoutViewSources = new List<IViewSource> { ViewSourceBuilder.Create(".className Test") };
            var result = parser.Parse(layoutViewSources);
            Assert.IsInstanceOf(typeof(HamlTree), result);
        }

        [Test]
        [TestCase("", 0)]
        public void Parse_ValidTemplate_TreeContainsCorrectNoOfChildren(string template, int expectedChildren)
        {
            HamlTreeParser parser = new HamlTreeParser();
            var layoutViewSources = new List<IViewSource> { ViewSourceBuilder.Create(template) };
            var result = parser.Parse(layoutViewSources);
            Assert.AreEqual(expectedChildren, result.Children.Count);
        }
    }
}
