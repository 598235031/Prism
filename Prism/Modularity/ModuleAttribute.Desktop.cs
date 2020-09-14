using System;
namespace Wanghzh.Prism.Modularity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ModuleAttribute : Attribute
    {
        public string ModuleName { get; set; }
        [Obsolete("StartupLoaded has been replaced by the OnDemand property.")]
        public bool StartupLoaded
        {
            get { return !OnDemand; }
            set { OnDemand = !value; }
        }
        public bool OnDemand { get; set; }
    }
}
