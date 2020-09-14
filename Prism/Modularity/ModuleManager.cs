using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Wanghzh.Prism.Logging;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Modularity
{
    public partial class ModuleManager : IModuleManager, IDisposable
    {
        private readonly IModuleInitializer moduleInitializer;
        private readonly IModuleCatalog moduleCatalog;
        private readonly ILoggerFacade loggerFacade;
        private IEnumerable<IModuleTypeLoader> typeLoaders;
        private HashSet<IModuleTypeLoader> subscribedToModuleTypeLoaders = new HashSet<IModuleTypeLoader>();
        public ModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog, ILoggerFacade loggerFacade)
        {
            if (moduleInitializer == null)
            {
                throw new ArgumentNullException("moduleInitializer");
            }
            if (moduleCatalog == null)
            {
                throw new ArgumentNullException("moduleCatalog");
            }
            if (loggerFacade == null)
            {
                throw new ArgumentNullException("loggerFacade");
            }
            this.moduleInitializer = moduleInitializer;
            this.moduleCatalog = moduleCatalog;
            this.loggerFacade = loggerFacade;
        }
        protected IModuleCatalog ModuleCatalog
        {
            get { return this.moduleCatalog; }
        }
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;
        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            if (this.ModuleDownloadProgressChanged != null)
            {
                this.ModuleDownloadProgressChanged(this, e);
            }
        }
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;
        private void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }
        private void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            if (this.LoadModuleCompleted != null)
            {
                this.LoadModuleCompleted(this, e);
            }
        }
        public void Run()
        {
            this.moduleCatalog.Initialize();
            this.LoadModulesWhenAvailable();
        }
        public void LoadModule(string moduleName)
        {
            IEnumerable<ModuleInfo> module = this.moduleCatalog.Modules.Where(m => m.ModuleName == moduleName);
            if (module == null || module.Count() != 1)
            {
                throw new ModuleNotFoundException(moduleName, string.Format(CultureInfo.CurrentCulture, Resources.ModuleNotFound, moduleName));
            }
            IEnumerable<ModuleInfo> modulesToLoad = this.moduleCatalog.CompleteListWithDependencies(module);
            this.LoadModuleTypes(modulesToLoad);
        }
        protected virtual bool ModuleNeedsRetrieval(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");
            if (moduleInfo.State == ModuleState.NotStarted)
            {
                bool isAvailable = Type.GetType(moduleInfo.ModuleType) != null;
                if (isAvailable)
                {
                    moduleInfo.State = ModuleState.ReadyForInitialization;
                }
                return !isAvailable;
            }
            return false;
        }
        private void LoadModulesWhenAvailable()
        {
            IEnumerable<ModuleInfo> whenAvailableModules = this.moduleCatalog.Modules.Where(m => m.InitializationMode == InitializationMode.WhenAvailable);
            IEnumerable<ModuleInfo> modulesToLoadTypes = this.moduleCatalog.CompleteListWithDependencies(whenAvailableModules);
            if (modulesToLoadTypes != null)
            {
                this.LoadModuleTypes(modulesToLoadTypes);
            }
        }
        private void LoadModuleTypes(IEnumerable<ModuleInfo> moduleInfos)
        {
            if (moduleInfos == null)
            {
                return;
            }
            foreach (ModuleInfo moduleInfo in moduleInfos)
            {
                if (moduleInfo.State == ModuleState.NotStarted)
                {
                    if (this.ModuleNeedsRetrieval(moduleInfo))
                    {
                        this.BeginRetrievingModule(moduleInfo);
                    }
                    else
                    {
                        moduleInfo.State = ModuleState.ReadyForInitialization;
                    }
                }
            }
            this.LoadModulesThatAreReadyForLoad();
        }
        protected void LoadModulesThatAreReadyForLoad()
        {
            bool keepLoading = true;
            while (keepLoading)
            {
                keepLoading = false;
                IEnumerable<ModuleInfo> availableModules = this.moduleCatalog.Modules.Where(m => m.State == ModuleState.ReadyForInitialization);
                foreach (ModuleInfo moduleInfo in availableModules)
                {
                    if ((moduleInfo.State != ModuleState.Initialized) && (this.AreDependenciesLoaded(moduleInfo)))
                    {
                        moduleInfo.State = ModuleState.Initializing;
                        this.InitializeModule(moduleInfo);
                        keepLoading = true;
                        break;
                    }
                }
            }
        }        
        private void BeginRetrievingModule(ModuleInfo moduleInfo)
        {
            ModuleInfo moduleInfoToLoadType = moduleInfo;
            IModuleTypeLoader moduleTypeLoader = this.GetTypeLoaderForModule(moduleInfoToLoadType);
            moduleInfoToLoadType.State = ModuleState.LoadingTypes;
            if (!this.subscribedToModuleTypeLoaders.Contains(moduleTypeLoader))
            {
                moduleTypeLoader.ModuleDownloadProgressChanged += this.IModuleTypeLoader_ModuleDownloadProgressChanged;
                moduleTypeLoader.LoadModuleCompleted += this.IModuleTypeLoader_LoadModuleCompleted;
                this.subscribedToModuleTypeLoaders.Add(moduleTypeLoader);
            }
            moduleTypeLoader.LoadModuleType(moduleInfo);
        }
        private void IModuleTypeLoader_ModuleDownloadProgressChanged(object sender, ModuleDownloadProgressChangedEventArgs e)
        {
            this.RaiseModuleDownloadProgressChanged(e);
        }
        private void IModuleTypeLoader_LoadModuleCompleted(object sender, LoadModuleCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if ((e.ModuleInfo.State != ModuleState.Initializing) && (e.ModuleInfo.State != ModuleState.Initialized))
                {
                    e.ModuleInfo.State = ModuleState.ReadyForInitialization;
                }
                this.LoadModulesThatAreReadyForLoad();
            }
            else
            {
                this.RaiseLoadModuleCompleted(e);
                if (!e.IsErrorHandled)
                {
                    this.HandleModuleTypeLoadingError(e.ModuleInfo, e.Error);
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        protected virtual void HandleModuleTypeLoadingError(ModuleInfo moduleInfo, Exception exception)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");
            ModuleTypeLoadingException moduleTypeLoadingException = exception as ModuleTypeLoadingException;
            if (moduleTypeLoadingException == null)
            {
                moduleTypeLoadingException = new ModuleTypeLoadingException(moduleInfo.ModuleName, exception.Message, exception);
            }
            this.loggerFacade.Log(moduleTypeLoadingException.Message, Category.Exception, Priority.High);
            throw moduleTypeLoadingException;
        }
        private bool AreDependenciesLoaded(ModuleInfo moduleInfo)
        {
            IEnumerable<ModuleInfo> requiredModules = this.moduleCatalog.GetDependentModules(moduleInfo);
            if (requiredModules == null)
            {
                return true;
            }
            int notReadyRequiredModuleCount =
                requiredModules.Count(requiredModule => requiredModule.State != ModuleState.Initialized);
            return notReadyRequiredModuleCount == 0;
        }
        private IModuleTypeLoader GetTypeLoaderForModule(ModuleInfo moduleInfo)
        {
            foreach (IModuleTypeLoader typeLoader in this.ModuleTypeLoaders)
            {
                if (typeLoader.CanLoadModuleType(moduleInfo))
                {
                    return typeLoader;
                }
            }
            throw new ModuleTypeLoaderNotFoundException(moduleInfo.ModuleName, String.Format(CultureInfo.CurrentCulture, Resources.NoRetrieverCanRetrieveModule, moduleInfo.ModuleName), null);
        }
        private void InitializeModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo.State == ModuleState.Initializing)
            {
                this.moduleInitializer.Initialize(moduleInfo);
                moduleInfo.State = ModuleState.Initialized;
                this.RaiseLoadModuleCompleted(moduleInfo, null);
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            foreach (IModuleTypeLoader typeLoader in this.ModuleTypeLoaders)
            {
                IDisposable disposableTypeLoader = typeLoader as IDisposable;
                if (disposableTypeLoader != null)
                {
                    disposableTypeLoader.Dispose();
                }
            }
        }
    }
}
