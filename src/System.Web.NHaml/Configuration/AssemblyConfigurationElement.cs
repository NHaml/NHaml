using System.Configuration;

namespace System.Web.NHaml.Configuration
{
    public  class AssemblyConfigurationElement : KeyedConfigurationElement
    {
        public override string Key
        {
            get { return Name; }
        }

        private const string AssemblyElement = "assembly";
        [ConfigurationProperty(AssemblyElement, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[AssemblyElement]; }
        }
    }
}