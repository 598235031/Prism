using System;
namespace Wanghzh.Prism.Modularity
{
    public partial class DuplicateModuleException : ModularityException
    {
        public DuplicateModuleException()
        {
        }
        public DuplicateModuleException(string message) : base(message)
        {
        }
        public DuplicateModuleException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public DuplicateModuleException(string moduleName, string message)
            : base(moduleName, message)
        {
        }
        public DuplicateModuleException(string moduleName, string message, Exception innerException)
            : base(moduleName, message, innerException)
        {
        }
    }
}
