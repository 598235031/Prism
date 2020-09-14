using System;
namespace Wanghzh.Prism.Modularity
{
    public class LoadModuleCompletedEventArgs : EventArgs
    {
        public LoadModuleCompletedEventArgs(ModuleInfo moduleInfo, Exception error)
        {
            if (moduleInfo == null)
            {
                throw new ArgumentNullException("moduleInfo");
            }
            this.ModuleInfo = moduleInfo;
            this.Error = error;
        }
        public ModuleInfo ModuleInfo { get; private set; }
        public Exception Error { get; private set; }
        public bool IsErrorHandled { get; set; }
    }
}
