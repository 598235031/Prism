using System;
namespace Wanghzh.Prism.Modularity
{
    public static class ModuleInfoGroupExtensions
    {
        public static ModuleInfoGroup AddModule(
                    this ModuleInfoGroup moduleInfoGroup,
                    string moduleName,
                    Type moduleType,
                    params string[] dependsOn)
        {
            if (moduleType == null) throw new ArgumentNullException("moduleType");
            if (moduleInfoGroup == null) throw new ArgumentNullException("moduleInfoGroup");
            ModuleInfo moduleInfo = new ModuleInfo(moduleName, moduleType.AssemblyQualifiedName);
            moduleInfo.DependsOn.AddRange(dependsOn);
            moduleInfoGroup.Add(moduleInfo);
            return moduleInfoGroup;
        }
        public static ModuleInfoGroup AddModule(
                    this ModuleInfoGroup moduleInfoGroup,
                    Type moduleType,
                    params string[] dependsOn)
        {
            if (moduleType == null) throw new ArgumentNullException("moduleType");
            return AddModule(moduleInfoGroup, moduleType.Name, moduleType, dependsOn);
        }
    }
}
