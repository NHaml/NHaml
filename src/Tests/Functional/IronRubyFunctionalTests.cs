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

        [Test]
        public override void Layout()
        {
            _templateEngine.Options.AddReference( typeof( string ).Assembly.Location );

            AssertRender( "Welcome", "Application" );
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