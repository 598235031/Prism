using System.Configuration;
namespace Wanghzh.Prism.Modularity
{
    public class ModuleDependencyConfigurationElement : ConfigurationElement
    {
        public ModuleDependencyConfigurationElement()
        {
        }
        public ModuleDependencyConfigurationElement(string moduleName)
        {
            base["moduleName"] = moduleName;
        }
        [ConfigurationProperty("moduleName", IsRequired = true, IsKey = true)]
        public string ModuleName
        {
            get { return (string)base["moduleName"]; }
            set { base["moduleName"] = value; }
        }
    }
}
