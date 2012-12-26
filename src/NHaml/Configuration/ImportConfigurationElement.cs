using System.Configuration;

namespace System.Web.NHaml.Configuration
{
    public  class ImportConfigurationElement : KeyedConfigurationElement
    {
        public override string Key
        {
            get { return Name; }
        }

        private const string NamespaceElement = "namespace";
        [ConfigurationProperty(NamespaceElement, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[NamespaceElement]; }
        }
    }
}