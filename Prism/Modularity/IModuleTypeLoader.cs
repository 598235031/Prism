using System;
namespace Wanghzh.Prism.Modularity
{
    public interface IModuleTypeLoader
    {
        bool CanLoadModuleType(ModuleInfo moduleInfo);      
        void LoadModuleType(ModuleInfo moduleInfo);
   
        event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;
        event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;
    }
}
