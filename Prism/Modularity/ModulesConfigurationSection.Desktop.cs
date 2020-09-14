using System.Configuration;
namespace Wanghzh.Prism.Modularity
{
    public class ModulesConfigurationSection : ConfigurationSection
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        public ModuleConfigurationElementCollection Modules
        {
            get { return (ModuleConfigurationElementCollection)base[""]; }
            set { base[""] = value; }
        }
    }
}
