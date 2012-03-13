using System.Configuration;

namespace NHaml4.Configuration
{
    public  class AssemblyConfigurationElement : KeyedConfigurationElement
    {
        public AssemblyConfigurationElement()
        {
        }

        public AssemblyConfigurationElement( string name )
        {
            Name = name;
        }

        public override string Key
        {
            get { return Name; }
        }

        private const string AssemblyElement = "assembly";
        [ConfigurationProperty(AssemblyElement, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[AssemblyElement]; }
            set { this[AssemblyElement] = value; }
        }
    }
}