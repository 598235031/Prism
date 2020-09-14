using System;
using System.Collections.Generic;
using System.Configuration;
namespace Wanghzh.Prism.Modularity
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class ModuleConfigurationElementCollection : ConfigurationElementCollection
    {
        public ModuleConfigurationElementCollection()
        {
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModuleConfigurationElementCollection(ModuleConfigurationElement[] modules)
        {
            if (modules == null) throw new System.ArgumentNullException("modules");
            foreach (ModuleConfigurationElement module in modules)
            {
                BaseAdd(module);
            }
        }
        protected override bool ThrowOnDuplicate
        {
            get { return true; }
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
        protected override string ElementName
        {
            get { return "module"; }
        }
        public ModuleConfigurationElement this[int index]
        {
            get { return (ModuleConfigurationElement)base.BaseGet(index); }
        }
        public void Add(ModuleConfigurationElement module)
        {
            BaseAdd(module);
        }
        public bool Contains(string moduleName)
        {
            return base.BaseGet(moduleName) != null;
        }
        public IList<ModuleConfigurationElement> FindAll(Predicate<ModuleConfigurationElement> match)
        {
            if (match == null) throw new System.ArgumentNullException("match");
            IList<ModuleConfigurationElement> found = new List<ModuleConfigurationElement>();
            foreach (ModuleConfigurationElement moduleElement in this)
            {
                if (match(moduleElement))
                {
                    found.Add(moduleElement);
                }
            }
            return found;
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleConfigurationElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleConfigurationElement)element).ModuleName;
        }
    }
}
