using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.TemplateBase;

namespace NHaml4.Tests
{
    [TestFixture]
    public class SimpleTemplateCache_Tests
    {
        class DummyTemplate : Template
        {
        }

        [Test]
        public void GetOrAdd_NewTemplate_AddsTemplateToCache()
        {
            string fileName = "test.haml";
            var cache = new SimpleTemplateCache();
            cache.GetOrAdd(fileName, DateTime.Now, () => new TemplateFactory(typeof(DummyTemplate)));
            Assert.That(cache.ContainsTemplate(fileName), Is.True);
        }

        [Test]
        public void GetOrAdd_SameTemplateAddedTwice_CreatesTemplateOnce()
        {
            string fileName = "test1.haml";
            int count = 0;
            var timeStamp = new DateTime(2012, 1, 1);
            var cache = new SimpleTemplateCache();
            for (int c = 0; c < 2; c++)
                cache.GetOrAdd(fileName, timeStamp, () =>
                {
                    count++;
                    return new TemplateFactory(typeof(DummyTemplate));
                });

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void GetOrAdd_DifferentTemplateAddedTwice_CreatesTemplateTwice()
        {
            string fileName = "test2.haml";
            int count = 0;
            var cache = new SimpleTemplateCache();
            for (int c = 0; c < 2; c++)
                cache.GetOrAdd(fileName, DateTime.Now.AddDays(c), () =>
                {
                    count++;
                    return new TemplateFactory(typeof(DummyTemplate));
                });

            Assert.That(count, Is.EqualTo(2));
        }
    }
}
