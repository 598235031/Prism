using System;
namespace Wanghzh.Prism.Modularity
{
    public interface IModuleManager
    {
        void Run();
        void LoadModule(string moduleName);       
        event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;
        event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;
    }
}
