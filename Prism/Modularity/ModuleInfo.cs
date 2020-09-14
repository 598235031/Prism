using System;
using System.Collections.ObjectModel;
namespace Wanghzh.Prism.Modularity
{
    public partial class ModuleInfo : IModuleCatalogItem
    {
        public ModuleInfo()
            : this(null, null, new string[0])
        {
        }
        public ModuleInfo(string name, string type, params string[] dependsOn)
        {
            if (dependsOn == null) throw new System.ArgumentNullException("dependsOn");
            this.ModuleName = name;
            this.ModuleType = type;
            this.DependsOn = new Collection<string>();
            foreach (string dependency in dependsOn)
            {
                this.DependsOn.Add(dependency);
            }
        }
        public ModuleInfo(string name, string type) : this(name, type, new string[0])
        {
        }
        public string ModuleName { get; set; }
        public string ModuleType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "The setter is here to work around a Silverlight issue with setting properties from within Xaml.")]
        public Collection<string> DependsOn { get; set; }
        public InitializationMode InitializationMode { get; set; }
        public string Ref { get; set; }
        public ModuleState State { get; set; }
    }
}
