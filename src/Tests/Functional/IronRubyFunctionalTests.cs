using NHaml.Compilers.IronRuby;

using NUnit.Framework;

namespace NHaml.Tests.Functional
{
    [TestFixture]
    public class IronRubyFunctionalTests : FunctionalTestFixture
    {
        public override void SetUp()
        {
            base.SetUp();

            _templateEngine.Options.TemplateCompiler = new IronRubyTemplateCompiler();
            _templateEngine.TemplateContentProvider.PathSources.Insert( 0, TemplatesFolder + @"IronRuby" );
        }

        [Ignore("Until debugging with IR works")]
        public override void WithRunTimeException()
        {
            base.WithRunTimeException();
        }
        [Test]
        public override void Layout()
        {
            _templateEngine.Options.AddReference( typeof( string ).Assembly.Location );

            AssertRender(new[]{"Application", "Welcome"});
        }

        [Test,Ignore]
        public override void LambdaEval()
        {
            base.LambdaEval();
        }

        [Test, Ignore]
        public override void MultiLayout()
        {
            base.MultiLayout();
        }
        [Test, Ignore]
        public override void MetaModel()
        {
            base.MetaModel();
        }

        [Test, Ignore]
        public override void MetaWithoutModel()
        {
            base.MetaWithoutModel();
        }
    }
}