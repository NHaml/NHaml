using NUnit.Framework;

namespace HamlSpec
{
    [TestFixture]
    public class HamlSpecs
    {
        private SpecTestRunner _specTestRunner;
        const string FileName = "specs_haml.json";

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
        [TestCase("plain text templates")]
        [TestCase("tags with unusual HTML characters")]
        [TestCase("tags with unusual CSS identifiers")]
        [TestCase("basic Haml tags and CSS")]
        [TestCase("silent comments")]
        [TestCase("tags with inline content")]
        [TestCase("markup comments")]
        [TestCase("tags with nested content")]
        [TestCase("tags with HTML-style attributes")]
        [TestCase("boolean attributes")]
        [TestCase("whitespace removal")]
        [TestCase("Ruby-style interpolation")]
        [TestCase("tags with HTML-style attributes and variables")]
        [TestCase("headers")]

        // IN PROGRESS

        // TODO
        //[TestCase("whitespace preservation")]
        //[TestCase("HTML escaping")]
        //[TestCase("tags with Ruby-style attributes")]
        //[TestCase("internal filters")]
        public void ExecuteHamlsTestSuite(string groupName)
        {
            _specTestRunner.ExecuteSpecs(groupName);
        }
    }
}