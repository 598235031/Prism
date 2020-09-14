using System;
using System.Configuration;
namespace Wanghzh.Prism.Modularity
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class ModuleDependencyCollection : ConfigurationElementCollection
    {
        public ModuleDependencyCollection()
        {
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModuleDependencyCollection(ModuleDependencyConfigurationElement[] dependencies)
        {
            if (dependencies == null)
                throw new ArgumentNullException("dependencies");
            foreach (ModuleDependencyConfigurationElement dependency in dependencies)
            {
                BaseAdd(dependency);
            }
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
        protected override string ElementName
        {
            get { return "dependency"; }
        }
        public ModuleDependencyConfigurationElement this[int index]
        {
            get { return (ModuleDependencyConfigurationElement)base.BaseGet(index); }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleDependencyConfigurationElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleDependencyConfigurationElement)element).ModuleName;
        }
    }
}
