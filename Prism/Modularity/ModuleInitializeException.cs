using System;
using System.Globalization;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Modularity
{
    public partial class ModuleInitializeException : ModularityException
    {
        public ModuleInitializeException()
        {
        }
        public ModuleInitializeException(string message) : base(message)
        {
        }
        public ModuleInitializeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public ModuleInitializeException(string moduleName, string moduleAssembly, string message)
            : this(moduleName, message, moduleAssembly, null)
        {
        }
        public ModuleInitializeException(string moduleName, string moduleAssembly, string message, Exception innerException)
            : base(
                moduleName,
                String.Format(CultureInfo.CurrentCulture, Resources.FailedToLoadModule, moduleName, moduleAssembly, message),
                innerException)
        {
        }
        public ModuleInitializeException(string moduleName, string message, Exception innerException)
            : base(
                moduleName,
                String.Format(CultureInfo.CurrentCulture, Resources.FailedToLoadModuleNoAssemblyInfo, moduleName, message),
                innerException)
        {
        }
    }
}
