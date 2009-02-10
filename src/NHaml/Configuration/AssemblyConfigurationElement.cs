using System.Configuration;

namespace NHaml.Configuration
{
    public sealed class AssemblyConfigurationElement : KeyedConfigurationElement
    {
        private const string AssemblyElement = "assembly";

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

        [ConfigurationProperty( AssemblyElement, IsRequired = true, IsKey = true )]
        public string Name
        {
            get { return (string)this[AssemblyElement]; }
            set { this[AssemblyElement] = value; }
        }
    }
}