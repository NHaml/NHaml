using NUnit.Framework;

namespace HamlSpec
{
    [TestFixture]
    public class NHamlSpecs
    {
        private SpecTestRunner _specTestRunner;
        const string FileName = "specs_nhaml.json";

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _specTestRunner = new SpecTestRunner(FileName);

#if PROFILED
            EQATEC.Profiler.Runtime.ClearProfileSnapshot();
#endif
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            _specTestRunner.OutputSummaryToConsole();

#if PROFILED
            EQATEC.Profiler.Runtime.TakeProfileSnapshot(true);
#endif
        }

        // WORKING
        [TestCase("Eval")]

        // IN PROGRESS
        //[TestCase("Inline code")]
        public void ExecuteHamlsTestSuite(string groupName)
        {
            _specTestRunner.ExecuteSpecs(groupName);
        }
    }
}