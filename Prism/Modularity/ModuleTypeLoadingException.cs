using System;
using System.Globalization;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Modularity
{
    public partial class ModuleTypeLoadingException : ModularityException
    {
        public ModuleTypeLoadingException()
            : base()
        {
        }
        public ModuleTypeLoadingException(string message)
            : base(message)
        {
        }
        public ModuleTypeLoadingException(string message, Exception exception)
            : base(message, exception)
        {
        }
        public ModuleTypeLoadingException(string moduleName, string message)
            : this(moduleName, message, null)
        {
        }
        public ModuleTypeLoadingException(string moduleName, string message, Exception innerException)
            : base(moduleName, String.Format(CultureInfo.CurrentCulture, Resources.FailedToRetrieveModule, moduleName, message), innerException)
        {
        }
    }
}
