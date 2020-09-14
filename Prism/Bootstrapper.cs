using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CommonServiceLocator;
using Wanghzh.Prism.Logging;
using Wanghzh.Prism.Modularity;
using Wanghzh.Prism.Regions;
using Wanghzh.Prism.Regions.Behaviors;
namespace Wanghzh.Prism
{
    public abstract class Bootstrapper
    {
        protected ILoggerFacade Logger { get; set; }
        protected IModuleCatalog ModuleCatalog { get; set; }
        protected DependencyObject Shell { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The Logger is added to the container which will dispose it when the container goes out of scope.")]
        protected virtual ILoggerFacade CreateLogger()
        {
            return new TextLogger();
        }
        public void Run()
        {
            this.Run(true);
        }
        protected virtual IModuleCatalog CreateModuleCatalog()
        {
            return new ModuleCatalog();
        }
        protected virtual void ConfigureModuleCatalog()
        {
        }
        protected virtual void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(
                typeof(ActivationException));
        }
        protected virtual void InitializeModules()
        {
            IModuleManager manager = ServiceLocator.Current.GetInstance<IModuleManager>();
            manager.Run();
        }
        protected virtual RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings regionAdapterMappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
            if (regionAdapterMappings != null)
            {
                regionAdapterMappings.RegisterMapping(typeof(Selector), ServiceLocator.Current.GetInstance<SelectorRegionAdapter>());
                regionAdapterMappings.RegisterMapping(typeof(ItemsControl), ServiceLocator.Current.GetInstance<ItemsControlRegionAdapter>());
                regionAdapterMappings.RegisterMapping(typeof(ContentControl), ServiceLocator.Current.GetInstance<ContentControlRegionAdapter>());
            }
            return regionAdapterMappings;
        }
        protected virtual IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var defaultRegionBehaviorTypesDictionary = ServiceLocator.Current.GetInstance<IRegionBehaviorFactory>();
            if (defaultRegionBehaviorTypesDictionary != null)
            {
                defaultRegionBehaviorTypesDictionary.AddIfMissing(BindRegionContextToDependencyObjectBehavior.BehaviorKey,
                                                                  typeof(BindRegionContextToDependencyObjectBehavior));
                defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionActiveAwareBehavior.BehaviorKey,
                                                                  typeof(RegionActiveAwareBehavior));
                defaultRegionBehaviorTypesDictionary.AddIfMissing(SyncRegionContextWithHostBehavior.BehaviorKey,
                                                                  typeof(SyncRegionContextWithHostBehavior));
                defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionManagerRegistrationBehavior.BehaviorKey,
                                                                  typeof(RegionManagerRegistrationBehavior));
                defaultRegionBehaviorTypesDictionary.AddIfMissing(RegionMemberLifetimeBehavior.BehaviorKey,
                                                  typeof(RegionMemberLifetimeBehavior));
                defaultRegionBehaviorTypesDictionary.AddIfMissing(ClearChildViewsRegionBehavior.BehaviorKey,
                                                  typeof(ClearChildViewsRegionBehavior));
                defaultRegionBehaviorTypesDictionary.AddIfMissing(AutoPopulateRegionBehavior.BehaviorKey,
                                                  typeof(AutoPopulateRegionBehavior));
            }
            return defaultRegionBehaviorTypesDictionary;
        }
        protected virtual void InitializeShell()
        {
        }
        public abstract void Run(bool runWithDefaultConfiguration);
        protected abstract DependencyObject CreateShell();
        protected abstract void ConfigureServiceLocator();
    }
}
