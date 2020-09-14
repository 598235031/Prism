using System;
namespace Wanghzh.Prism.Modularity
{
    public partial class ModuleNotFoundException : ModularityException
    {
        public ModuleNotFoundException()
        {
        }
        public ModuleNotFoundException(string message)
            : base(message)
        {
        }
        public ModuleNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public ModuleNotFoundException(string moduleName, string message)
            : base(moduleName, message)
        {
        }
        public ModuleNotFoundException(string moduleName, string message, Exception innerException)
            : base(moduleName, message, innerException)
        {
        }
    }
}
