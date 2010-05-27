using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NUnit.Framework;
using NHaml.Core.Template;

namespace NHaml.Tests
{
    [TestFixture]
    public abstract class FunctionalTestFixture : TestFixtureBase
    {
        [Test, Ignore("Partials are handled by the View Engine")]
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

        [Test, Ignore("Tabs are currently output-only in NHaml 3")]
        public virtual void Tabs()
        {
            _templateEngine.Options.UseTabs = true;

            AssertRender("Tabs");
        }

        [Test, Ignore("Alternative indentation is currently output-only in NHaml 3")]
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

        [Test, Ignore("Partials are handled by the View Engine")]
        public virtual void SharedPartial()
        {
            AssertRender("SharedPartial");
        }

        [Test, Ignore("Partials are handled by the View Engine")]
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
            AssertRender("EscapeHtmlOff");
        }

        [Test]
        public virtual void EscapeHtmlOnByDefault()
        {
            _templateEngine.Options.EncodeHtml = true;
            AssertRender("EscapeHtmlOn");
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
            _templateEngine.Options.TemplateBaseType = typeof( CustomGenericTemplate<string> );
            _templateEngine.Options.AddReference( typeof( Action ).Assembly.Location );

            var template = CreateTemplate("MetaModel");

            //TODO: work out why this does not work
            //Assert.IsAssignableFrom<CustomGenericTemplate<string>>(template);

            ((CustomGenericTemplate<string>) template).Model = "NHaml";

            var output = new StringWriter();

            template.Render( output );

            AssertRender( output, "MetaModel" );
        }

        [Test]
        public virtual void MetaWithoutModel()
        {
            _templateEngine.Options.TemplateBaseType = typeof( CustomGenericTemplate<object> );
            _templateEngine.Options.AddReference( typeof( Action ).Assembly.Location );

            var template = CreateTemplate("MetaWithoutModel");

            //TODO: work out why this does not work
            //Assert.IsAssignableFrom<CustomGenericTemplate<object>>(template);

            var output = new StringWriter();
            template.Render( output );
            AssertRender( output, "MetaWithoutModel" );
        }

        public abstract class CustomGenericTemplate<T> : Template
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
            private readonly TextWriter _output;

            public CustomHelper(TextWriter output)
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

            public void Tag<T1, T2>(string name, T1 t1, T2 t2, System.Action<T1, T2> yield)
            {
                _output.WriteLine("<" + name + ">");
                yield(t1, t2);
                _output.WriteLine("</" + name + ">");
            }
        }

        [Test, Ignore("Lambda support need to be added back")]
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
        public virtual void TextEvalEncodeHtml()
        {
            _templateEngine.Options.EncodeHtml = true;
            AssertRender("TextEvalEncode");
        }

        [Test]
        public virtual void TextEvalDontEncode()
        {
            _templateEngine.Options.EncodeHtml = false;
            AssertRender("TextEvalDontEncode");
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

        [Test, Ignore("Not yet implemented")]
        public virtual void NullAttributes()
        {
            AssertRender("NullAttributes");
        }

        [Test, Ignore("Partials are handled by the View Engine")]
        public virtual void Partials()
        {
            AssertRender("Partials");
        }

        [Test, Ignore("Preamble is deprecated, and inavailable")]
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
            AssertRender("Welcome", "Application", "Application");
        }


        [Test]
        public virtual void MultiLayout()
        {
            // ApplicationPart2 master file has a reference to ApplicationPart1
            AssertRender("Welcome2", "ApplicationPart2");
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
        [Test]
        public virtual void EnsureBaseAndInterfaceReferenceIsAddedFromOptions()
        {
            _templateEngine.Options.TemplateBaseType = typeof (GenericTemplateView<DataSet>);
            AssertRender("EnsureBaseAndInterfaceReferenceIsAdded");
        }

        [Test]
        public virtual void EnsureBaseAndInterfaceReferenceIsAdded()
        {
            _templateEngine.Options.TemplateBaseType = typeof(GenericTemplateView<DataSet>);
            var template = _templateEngine.Compile("EnsureBaseAndInterfaceReferenceIsAdded");
            using (var output = new StringWriter())
            {
                template.CreateInstance().Render(output);

                AssertRender(output, "EnsureBaseAndInterfaceReferenceIsAdded");
            }
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
        protected override void PreRender( TextWriter outputWriter )
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