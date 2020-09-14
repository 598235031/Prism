using System.Collections.Generic;
namespace Wanghzh.Prism.Modularity
{
    public interface IModuleCatalog
    {
        IEnumerable<ModuleInfo> Modules { get; }
        IEnumerable<ModuleInfo> GetDependentModules(ModuleInfo moduleInfo);
        IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules);
        void Initialize();
        void AddModule(ModuleInfo moduleInfo);
    }
}
