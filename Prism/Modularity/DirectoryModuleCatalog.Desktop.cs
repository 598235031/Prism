using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Modularity
{
    public class DirectoryModuleCatalog : ModuleCatalog
    {
        public string ModulePath { get; set; }
        protected override void InnerLoad()
        {
            if (string.IsNullOrEmpty(this.ModulePath))
                throw new InvalidOperationException(Resources.ModulePathCannotBeNullOrEmpty);
            if (!Directory.Exists(this.ModulePath))
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.DirectoryNotFound, this.ModulePath));
            AppDomain childDomain = this.BuildChildDomain(AppDomain.CurrentDomain);
            try
            {
                List<string> loadedAssemblies = new List<string>();
                var assemblies = (
                                     from Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     where !(assembly is System.Reflection.Emit.AssemblyBuilder)
										&& assembly.GetType().FullName != "System.Reflection.Emit.InternalAssemblyBuilder"
                                        && !String.IsNullOrEmpty(assembly.Location)
                                     select assembly.Location
                                 );
                loadedAssemblies.AddRange(assemblies);
                Type loaderType = typeof(InnerModuleInfoLoader);
                if (loaderType.Assembly != null)
                {
                    var loader =
                        (InnerModuleInfoLoader)
                        childDomain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();
                    loader.LoadAssemblies(loadedAssemblies);
                    this.Items.AddRange(loader.GetModuleInfos(this.ModulePath));
                }
            }
            finally
            {
                AppDomain.Unload(childDomain);
            }
        }
        protected virtual AppDomain BuildChildDomain(AppDomain parentDomain)
        {
            if (parentDomain == null) throw new System.ArgumentNullException("parentDomain");
            Evidence evidence = new Evidence(parentDomain.Evidence);
            AppDomainSetup setup = parentDomain.SetupInformation;
            return AppDomain.CreateDomain("DiscoveryRegion", evidence, setup);
        }
        private class InnerModuleInfoLoader : MarshalByRefObject
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            internal ModuleInfo[] GetModuleInfos(string path)
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                ResolveEventHandler resolveEventHandler =
                    delegate(object sender, ResolveEventArgs args) { return OnReflectionOnlyResolve(args, directory); };
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolveEventHandler;
                Assembly moduleReflectionOnlyAssembly =
                    AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().First(
                        asm => asm.FullName == typeof(IModule).Assembly.FullName);
                Type IModuleType = moduleReflectionOnlyAssembly.GetType(typeof(IModule).FullName);
                IEnumerable<ModuleInfo> modules = GetNotAllreadyLoadedModuleInfos(directory, IModuleType);
                var array = modules.ToArray();
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolveEventHandler;
                return array;
            }
            private static IEnumerable<ModuleInfo> GetNotAllreadyLoadedModuleInfos(DirectoryInfo directory, Type IModuleType)
            {
                List<FileInfo> validAssemblies = new List<FileInfo>();
                Assembly[] alreadyLoadedAssemblies = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();
                var fileInfos = directory.GetFiles("*.dll")
                    .Where(file => alreadyLoadedAssemblies
                                       .FirstOrDefault(
                                       assembly =>
                                       String.Compare(Path.GetFileName(assembly.Location), file.Name,
                                                      StringComparison.OrdinalIgnoreCase) == 0) == null);
                
                foreach (FileInfo fileInfo in fileInfos)
                {
                    try
                    {
                        Assembly.ReflectionOnlyLoadFrom(fileInfo.FullName);
                        validAssemblies.Add(fileInfo);
                    }
                    catch (BadImageFormatException)
                    {
                    }
                }
                return validAssemblies.SelectMany(file => Assembly.ReflectionOnlyLoadFrom(file.FullName)
                                            .GetExportedTypes()
                                            .Where(IModuleType.IsAssignableFrom)
                                            .Where(t => t != IModuleType)
                                            .Where(t => !t.IsAbstract)
                                            .Select(type => CreateModuleInfo(type)));
            }
            private static Assembly OnReflectionOnlyResolve(ResolveEventArgs args, DirectoryInfo directory)
            {
                Assembly loadedAssembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().FirstOrDefault(
                    asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));
                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }
                AssemblyName assemblyName = new AssemblyName(args.Name);
                string dependentAssemblyFilename = Path.Combine(directory.FullName, assemblyName.Name + ".dll");
                if (File.Exists(dependentAssemblyFilename))
                {
                    return Assembly.ReflectionOnlyLoadFrom(dependentAssemblyFilename);
                }
                return Assembly.ReflectionOnlyLoad(args.Name);
            }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            internal void LoadAssemblies(IEnumerable<string> assemblies)
            {
                foreach (string assemblyPath in assemblies)
                {
                    try
                    {
                        Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
            }
            private static ModuleInfo CreateModuleInfo(Type type)
            {
                string moduleName = type.Name;
                List<string> dependsOn = new List<string>();
                bool onDemand = false;
                var moduleAttribute =
                    CustomAttributeData.GetCustomAttributes(type).FirstOrDefault(
                        cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleAttribute).FullName);
                if (moduleAttribute != null)
                {
                    foreach (CustomAttributeNamedArgument argument in moduleAttribute.NamedArguments)
                    {
                        string argumentName = argument.MemberInfo.Name;
                        switch (argumentName)
                        {
                            case "ModuleName":
                                moduleName = (string) argument.TypedValue.Value;
                                break;
                            case "OnDemand":
                                onDemand = (bool) argument.TypedValue.Value;
                                break;
                            case "StartupLoaded":
                                onDemand = !((bool) argument.TypedValue.Value);
                                break;
                        }                           
                    }
                }
                var moduleDependencyAttributes =
                    CustomAttributeData.GetCustomAttributes(type).Where(
                        cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleDependencyAttribute).FullName);
                foreach (CustomAttributeData cad in moduleDependencyAttributes)
                {
                    dependsOn.Add((string) cad.ConstructorArguments[0].Value);
                }
                ModuleInfo moduleInfo = new ModuleInfo(moduleName, type.AssemblyQualifiedName)
                                            {
                                                InitializationMode =
                                                    onDemand
                                                        ? InitializationMode.OnDemand
                                                        : InitializationMode.WhenAvailable,
                                                Ref = type.Assembly.CodeBase,
                                            };
                moduleInfo.DependsOn.AddRange(dependsOn);
                return moduleInfo;
            }
        }
    }
}
