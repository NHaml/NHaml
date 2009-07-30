using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public abstract class FunctionalTestFixture : TestFixtureBase
    {
        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public virtual void BadPartial()
        {
            AssertRender("BadPartial");
        }

        [Test]
        public virtual void BlankLine()
        {
            AssertRender("BlankLine");
        }

        [Test]
        public virtual void Tabs()
        {
            _templateEngine.Options.UseTabs = true;

            AssertRender("Tabs");
        }

        [Test]
        public virtual void FourSpaces()
        {
            _templateEngine.Options.IndentSize = 4;

            AssertRender("4Spaces");
        }

        [Test, Ignore( "Not implemented yet." )]
        public virtual void Alligators()
        {
            AssertRender("Alligators");
        }

        [Test]
        public virtual void VeryBasic()
        {
            AssertRender("VeryBasic");
        }

        [Test]
        public virtual void SelfClosing()
        {
            AssertRender("SelfClosing");
        }

        [Test]
        public virtual void SharedPartial()
        {
            AssertRender("SharedPartial");
        }

        [Test]
        public virtual void Partials2()
        {
            AssertRender("Partials2");
        }

        [Test]
        public virtual void Javascript()
        {
            AssertRender("Javascript");
        }

        [Test]
        public virtual void Doctype()
        {
            AssertRender("Doctype");
        }

        [Test]
        public virtual void Escape()
        {
            AssertRender("Escape");
        }

        [Test]
        public virtual void EscapeHtmlOffByDefault()
        {
            AssertRender("EscapeHtmlOff", "EscapeHtml");
        }

        [Test]
        public virtual void EscapeHtmlOnByDefault()
        {
            _templateEngine.Options.EncodeHtml = true;

            AssertRender("EscapeHtmlOn", "EscapeHtml");
        }

        [Test]
        public virtual void Empty()
        {
            AssertRender("Empty");
        }

        [Test]
        public virtual void AutoClose()
        {
            AssertRender("AutoClose");
        }

        [Test]
        public virtual void ReferenceExample1()
        {
            AssertRender("ReferenceExample1");
        }

        [Test]
        public virtual void ReferenceExample2()
        {
            AssertRender("ReferenceExample2");
        }

        [Test]
        public virtual void List()
        {
            AssertRender("List");
        }

        [Test]
        public virtual void TagParsing()
        {
            AssertRender("TagParsing");
        }

        [Test]
        public virtual void OriginalEngine()
        {
            AssertRender("OriginalEngine");
        }

        [Test]
        public virtual void Comments()
        {
            AssertRender("Comments");
        }
        
        [Test]
        public virtual void MetaModel()
        {
            _templateEngine.Options.TemplateBaseType = typeof( CustomGenericTemplate<> );
            _templateEngine.Options.AddReference( typeof( Action ).Assembly.Location );

            var template = CreateTemplate("MetaModel");

            Assert.IsInstanceOfType( typeof( CustomGenericTemplate<string> ), template );

            ((CustomGenericTemplate<string>) template).Model = "NHaml";

            var output = new StringWriter();

            template.Render( output );

            AssertRender( output, "MetaModel" );
        }

        [Test]
        public virtual void MetaWithoutModel()
        {
            _templateEngine.Options.TemplateBaseType = typeof( CustomGenericTemplate<> );
            _templateEngine.Options.AddReference( typeof( Action ).Assembly.Location );

            var template = CreateTemplate("MetaWithoutModel");

            Assert.IsInstanceOfType( typeof( CustomGenericTemplate<object> ), template );

            var output = new StringWriter();

            template.Render( output );

            AssertRender( output, "MetaWithoutModel" );
        }

        public class CustomGenericTemplate<T> : Template
        {
            public T Model { get; set; }

            public string ModelType { get { return typeof (T).FullName; } }
        }

        [Test]
        public virtual void WhitespaceSensitive()
        {
            AssertRender("WhitespaceSensitive");
        }

        public class CustomHelper
        {
            private readonly OutputWriter _output;

            public CustomHelper(OutputWriter output)
            {
                _output = output;
            }
            public static void Foo()
            {
                
            }

            public void Tag(Action action)
            {
                Tag("div", action);
            }

            public void Tag(string name, Action yield)
            {
                _output.WriteLine("<" + name + ">");
                yield();
                _output.WriteLine("</" + name + ">");
            }

            public void Tag<T>(string name, T t, Action<T> yield)
            {
                _output.WriteLine("<" + name + ">");
                yield(t);
                _output.WriteLine("</" + name + ">");
            }

            public void Tag<T1, T2>(string name, T1 t1, T2 t2, Action<T1, T2> yield)
            {
                _output.WriteLine("<" + name + ">");
                yield(t1, t2);
                _output.WriteLine("</" + name + ">");
            }
        }

        [Test]
        public virtual void LambdaEval()
        {
            _templateEngine.Options.TemplateBaseType = typeof( CustomTemplate1 );
            _templateEngine.Options.AddReference( typeof( Action ).Assembly.Location );

            AssertRender("LambdaEval");
        }

        [Test]
        public virtual void AttributeEval()
        {
            AssertRender("AttributeEval");
        }

        [Test]
        public virtual void AttributeNamespaceAndDynamic()
        {
            AssertRender("AttributeNamespaceAndDynamic");
        }
        [Test]
        public virtual void IdAndDynamic()
        {
            AssertRender("IdAndDynamic");
        }

        [Test]
        public virtual void SimpleEval()
        {
            AssertRender("SimpleEval");
        }
        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public virtual void WithRunTimeException()
        {
            AssertRender("WithRunTimeException");
        }

        [Test]
        public virtual void SilentEval()
        {
            AssertRender("SilentEval");
        }

        [Test]
        public virtual void LoopEval()
        {
            AssertRender("LoopEval");
        }

        [Test]
        public virtual void SwitchEval()
        {
            AssertRender("SwitchEval");
        }

        [Test]
        public virtual void Conditionals()
        {
            AssertRender("Conditionals");
        }

        [Test]
        public virtual void MultiLine()
        {
            AssertRender("MultiLine");
        }

        [Test]
        public virtual void NullAttributes()
        {
            AssertRender("NullAttributes");
        }

        [Test]
        public virtual void Partials()
        {
            AssertRender("Partials");
        }

        [Test]
        public virtual void Preamble()
        {
            AssertRender("Preamble");
        }

        [Test]
        public virtual void Welcome()
        {
            AssertRender("Welcome");
        }

        [Test]
        public virtual void Layout()
        {
            AssertRender(new[] {"Application", "Welcome"});
        }


        [Test]
        public virtual void MultiLayout()
        {
            using (var output = new StringWriter())
            {
                var compiledTemplate = _templateEngine.Compile(new List<string> { "ApplicationPart1", "ApplicationPart2", "Welcome"});
                var template = compiledTemplate.CreateInstance();
                template.Render(output);
                Console.WriteLine(output);

                Assert.AreEqual(File.ReadAllText(ExpectedFolder + "Application.xhtml"), output.ToString());
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public virtual void LayoutNoContent()
        {
            AssertRender("Application");
        }

        [Test]
        public virtual void ViewBaseClass()
        {
            _templateEngine.Options.TemplateBaseType = typeof( CustomTemplate2 );

            AssertRender("CustomBaseClass");
        }

        [Test]
        public virtual void ViewBaseClassGeneric()
        {
            _templateEngine.Options.TemplateBaseType = typeof( CustomTemplate3<List<int>> );

            AssertRender("CustomBaseClass");
        }

        [Test,Ignore("I currently dont not know how to handle this, because the parsers can not handle the <>Anon types.")]
        public virtual void ViewBaseClassGenericAnon()
        {
            var anonInstance = new { Property = "PropertyValue" };
            var genericType = typeof( GenericTemplateView<> ).MakeGenericType( new[] { anonInstance.GetType() } );
            _templateEngine.Options.TemplateBaseType = genericType;

            AssertRender( "GenericBaseClassAnon" );
        }

    

        [Test]
        public virtual void ViewBaseClassGeneric2()
        {
            _templateEngine.Options.TemplateBaseType = typeof( CustomTemplate4<List<List<int>>> );

            AssertRender( "CustomBaseClass" );
        }
    }

    public abstract class CustomTemplate1 : Template
    {
        protected override void PreRender( OutputWriter outputWriter )
        {
            Html = new FunctionalTestFixture.CustomHelper( outputWriter );
        }

        public FunctionalTestFixture.CustomHelper Html { get; private set; }
    }

    public abstract class CustomTemplate2 : Template
    {
        public int Foo
        {
            get { return 9; }
        }
    }
    public abstract class GenericTemplateView<TViewData> : Template 
    {
        public TViewData ViewData
        {
            get { return default(TViewData); }
        }
    }
    public abstract class CustomTemplate3<T> : Template
      where T : new()
    {
        public int Foo
        {
            get
            {
                object o = new T();

                var list = (List<int>)o;

                return list.Count + 9;
            }
        }
    }

    public abstract class CustomTemplate4<T> : Template
      where T : new()
    {
        public int Foo
        {
            get
            {
                object o = new T();

                var list = (List<List<int>>)o;

                return list.Count + 9;
            }
        }
    }
}