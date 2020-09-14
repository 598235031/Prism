using System;
using System.Globalization;
using CommonServiceLocator;
using Wanghzh.Prism.Logging;
namespace Wanghzh.Prism.Modularity
{
    public class ModuleInitializer : IModuleInitializer
    {
        private readonly IServiceLocator serviceLocator;
        private readonly ILoggerFacade loggerFacade;
        public ModuleInitializer(IServiceLocator serviceLocator, ILoggerFacade loggerFacade)
        {
            if (serviceLocator == null)
            {
                throw new ArgumentNullException("serviceLocator");
            }
            if (loggerFacade == null)
            {
                throw new ArgumentNullException("loggerFacade");
            }
            this.serviceLocator = serviceLocator;
            this.loggerFacade = loggerFacade;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catches Exception to handle any exception thrown during the initialization process with the HandleModuleInitializationError method.")]
        public void Initialize(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");
            IModule moduleInstance = null;
            try
            {
                moduleInstance = this.CreateModule(moduleInfo);
                moduleInstance.Initialize();
            }
            catch (Exception ex)
            {
                this.HandleModuleInitializationError(
                    moduleInfo,
                    moduleInstance != null ? moduleInstance.GetType().Assembly.FullName : null,
                    ex);
            }
        }
        public virtual void HandleModuleInitializationError(ModuleInfo moduleInfo, string assemblyName, Exception exception)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");
            if (exception == null) throw new ArgumentNullException("exception");
            Exception moduleException;
            if (exception is ModuleInitializeException)
            {
                moduleException = exception;
            }
            else
            {
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    moduleException = new ModuleInitializeException(moduleInfo.ModuleName, assemblyName, exception.Message, exception);
                }
                else
                {
                    moduleException = new ModuleInitializeException(moduleInfo.ModuleName, exception.Message, exception);
                }
            }
            this.loggerFacade.Log(moduleException.ToString(), Category.Exception, Priority.High);
            throw moduleException;
        }
        protected virtual IModule CreateModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");
            return this.CreateModule(moduleInfo.ModuleType);
        }
        protected virtual IModule CreateModule(string typeName)
        {
            Type moduleType = Type.GetType(typeName);
            if (moduleType == null)
            {
                throw new ModuleInitializeException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.FailedToGetType, typeName));
            }
            return (IModule)this.serviceLocator.GetInstance(moduleType);
        }
    }
}
