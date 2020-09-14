using System;
namespace Wanghzh.Prism.Modularity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ModuleDependencyAttribute : Attribute
    {
        private readonly string _moduleName;
        public ModuleDependencyAttribute(string moduleName)
        {
            _moduleName = moduleName;
        }
        public string ModuleName
        {
            get { return _moduleName; }
        }
    }
}
