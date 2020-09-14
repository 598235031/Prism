using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Markup;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Modularity
{
    [ContentProperty("Items")]
    public class ModuleCatalog : IModuleCatalog
    {
        private readonly ModuleCatalogItemCollection items;
        private bool isLoaded;
        public ModuleCatalog()
        {
            this.items = new ModuleCatalogItemCollection();
            this.items.CollectionChanged += this.ItemsCollectionChanged;
        }
        public ModuleCatalog(IEnumerable<ModuleInfo> modules)
            : this()
        {
            if (modules == null) throw new System.ArgumentNullException("modules");
            foreach (ModuleInfo moduleInfo in modules)
            {
                this.Items.Add(moduleInfo);
            }
        }
        public Collection<IModuleCatalogItem> Items
        {
            get { return this.items; }
        }
        public virtual IEnumerable<ModuleInfo> Modules
        {
            get
            {
                return this.GrouplessModules.Union(this.Groups.SelectMany(g => g));
            }
        }
        public IEnumerable<ModuleInfoGroup> Groups
        {
            get
            {
                return this.Items.OfType<ModuleInfoGroup>();
            }
        }
        protected bool Validated { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Groupless")]
        protected IEnumerable<ModuleInfo> GrouplessModules
        {
            get
            {
                return this.Items.OfType<ModuleInfo>();
            }
        }
        public static ModuleCatalog CreateFromXaml(Stream xamlStream)
        {
            if (xamlStream == null)
            {
                throw new ArgumentNullException("xamlStream");
            }
            return XamlReader.Load(xamlStream) as ModuleCatalog;
        }
        public static ModuleCatalog CreateFromXaml(Uri builderResourceUri)
        {
            var streamInfo = System.Windows.Application.GetResourceStream(builderResourceUri);
            if ((streamInfo != null) && (streamInfo.Stream != null))
            {
                return CreateFromXaml(streamInfo.Stream);
            }
            return null;
        }
        public void Load()
        {
            this.isLoaded = true;
            this.InnerLoad();
        }
        public virtual IEnumerable<ModuleInfo> GetDependentModules(ModuleInfo moduleInfo)
        {
            this.EnsureCatalogValidated();
            return this.GetDependentModulesInner(moduleInfo);
        }       
        public virtual IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null)
            {
                throw new ArgumentNullException("modules");
            }
            this.EnsureCatalogValidated();
            List<ModuleInfo> completeList = new List<ModuleInfo>();
            List<ModuleInfo> pendingList = modules.ToList();
            while (pendingList.Count > 0)
            {
                ModuleInfo moduleInfo = pendingList[0];
                foreach (ModuleInfo dependency in this.GetDependentModules(moduleInfo))
                {
                    if (!completeList.Contains(dependency) && !pendingList.Contains(dependency))
                    {
                        pendingList.Add(dependency);
                    }
                }
                pendingList.RemoveAt(0);
                completeList.Add(moduleInfo);
            }
            IEnumerable<ModuleInfo> sortedList = this.Sort(completeList);
            return sortedList;
        }
        public virtual void Validate()
        {
            this.ValidateUniqueModules();
            this.ValidateDependencyGraph();
            this.ValidateCrossGroupDependencies();
            this.ValidateDependenciesInitializationMode();
            this.Validated = true;
        }
        public virtual void AddModule(ModuleInfo moduleInfo)
        {
            this.Items.Add(moduleInfo);
        }
        public ModuleCatalog AddModule(Type moduleType, params string[] dependsOn)
        {
            return this.AddModule(moduleType, InitializationMode.WhenAvailable, dependsOn);
        }
        public ModuleCatalog AddModule(Type moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            if (moduleType == null) throw new System.ArgumentNullException("moduleType");
            return this.AddModule(moduleType.Name, moduleType.AssemblyQualifiedName, initializationMode, dependsOn);
        }
        public ModuleCatalog AddModule(string moduleName, string moduleType, params string[] dependsOn)
        {
            return this.AddModule(moduleName, moduleType, InitializationMode.WhenAvailable, dependsOn);
        }
        public ModuleCatalog AddModule(string moduleName, string moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            return this.AddModule(moduleName, moduleType, null, initializationMode, dependsOn);
        }
        public ModuleCatalog AddModule(string moduleName, string moduleType, string refValue, InitializationMode initializationMode, params string[] dependsOn)
        {
            if (moduleName == null)
            {
                throw new ArgumentNullException("moduleName");
            }
            if (moduleType == null)
            {
                throw new ArgumentNullException("moduleType");
            }
            ModuleInfo moduleInfo = new ModuleInfo(moduleName, moduleType);
            moduleInfo.DependsOn.AddRange(dependsOn);
            moduleInfo.InitializationMode = initializationMode;
            moduleInfo.Ref = refValue;
            this.Items.Add(moduleInfo);
            return this;
        }
        public virtual void Initialize()
        {
            if (!this.isLoaded)
            {
                this.Load();
            }
            this.Validate();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public virtual ModuleCatalog AddGroup(InitializationMode initializationMode, string refValue, params ModuleInfo[] moduleInfos)
        {
            if (moduleInfos == null) throw new System.ArgumentNullException("moduleInfos");
            ModuleInfoGroup newGroup = new ModuleInfoGroup();
            newGroup.InitializationMode = initializationMode;
            newGroup.Ref = refValue;
            foreach (ModuleInfo info in moduleInfos)
            {
                newGroup.Add(info);
            }
            this.items.Add(newGroup);
            return this;
        }
        protected static string[] SolveDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null) throw new System.ArgumentNullException("modules");
            ModuleDependencySolver solver = new ModuleDependencySolver();
            foreach (ModuleInfo data in modules)
            {
                solver.AddModule(data.ModuleName);
                if (data.DependsOn != null)
                {
                    foreach (string dependency in data.DependsOn)
                    {
                        solver.AddDependency(data.ModuleName, dependency);
                    }
                }
            }
            if (solver.ModuleCount > 0)
            {
                return solver.Solve();
            }
            return new string[0];
        }
        protected static void ValidateDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null) throw new System.ArgumentNullException("modules");
            var moduleNames = modules.Select(m => m.ModuleName).ToList();
            foreach (ModuleInfo moduleInfo in modules)
            {
                if (moduleInfo.DependsOn != null && moduleInfo.DependsOn.Except(moduleNames).Any())
                {
                    throw new ModularityException(
                        moduleInfo.ModuleName,
                        String.Format(CultureInfo.CurrentCulture, Resources.ModuleDependenciesNotMetInGroup, moduleInfo.ModuleName));
                }
            }
        }
        protected virtual void InnerLoad()
        {
        }
        protected virtual IEnumerable<ModuleInfo> Sort(IEnumerable<ModuleInfo> modules)
        {
            foreach (string moduleName in SolveDependencies(modules))
            {
                yield return modules.First(m => m.ModuleName == moduleName);
            }
        }
        
        protected virtual void ValidateUniqueModules()
        {
            List<string> moduleNames = this.Modules.Select(m => m.ModuleName).ToList();
            string duplicateModule = moduleNames.FirstOrDefault(
                m => moduleNames.Count(m2 => m2 == m) > 1);
            if (duplicateModule != null)
            {
                throw new DuplicateModuleException(duplicateModule, String.Format(CultureInfo.CurrentCulture, Resources.DuplicatedModule, duplicateModule));
            }
        }
        protected virtual void ValidateDependencyGraph()
        {
            SolveDependencies(this.Modules);
        }
        protected virtual void ValidateCrossGroupDependencies()
        {
            ValidateDependencies(this.GrouplessModules);
            foreach (ModuleInfoGroup group in this.Groups)
            {
                ValidateDependencies(this.GrouplessModules.Union(group));
            }
        }
        protected virtual void ValidateDependenciesInitializationMode()
        {
            ModuleInfo moduleInfo = this.Modules.FirstOrDefault(
                m =>
                m.InitializationMode == InitializationMode.WhenAvailable &&
                this.GetDependentModulesInner(m)
                    .Any(dependency => dependency.InitializationMode == InitializationMode.OnDemand));
            if (moduleInfo != null)
            {
                throw new ModularityException(
                    moduleInfo.ModuleName,
                    String.Format(CultureInfo.CurrentCulture, Resources.StartupModuleDependsOnAnOnDemandModule, moduleInfo.ModuleName));
            }
        }
        protected virtual IEnumerable<ModuleInfo> GetDependentModulesInner(ModuleInfo moduleInfo)
        {
            return this.Modules.Where(dependantModule => moduleInfo.DependsOn.Contains(dependantModule.ModuleName));
        }
        protected virtual void EnsureCatalogValidated()
        {
            if (!this.Validated)
            {
                this.Validate();
            }
        }
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Validated)
            {
                this.EnsureCatalogValidated();
            }
        }
        private class ModuleCatalogItemCollection : Collection<IModuleCatalogItem>, INotifyCollectionChanged
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;
            protected override void InsertItem(int index, IModuleCatalogItem item)
            {
                base.InsertItem(index, item);
                this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
            protected void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
            {
                if (this.CollectionChanged != null)
                {
                    this.CollectionChanged(this, eventArgs);
                }
            }
        }
    }
}
