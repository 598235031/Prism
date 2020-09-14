using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Modularity
{
    public class AssemblyResolver : IAssemblyResolver, IDisposable
    {
        private readonly List<AssemblyInfo> registeredAssemblies = new List<AssemblyInfo>();
        private bool handlesAssemblyResolve;
        public void LoadAssemblyFrom(string assemblyFilePath)
        {
            if (!this.handlesAssemblyResolve)
            {
                AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
                this.handlesAssemblyResolve = true;
            }
            Uri assemblyUri = GetFileUri(assemblyFilePath);
            if (assemblyUri == null)
            {
                throw new ArgumentException(Resources.InvalidArgumentAssemblyUri, "assemblyFilePath");
            }
            if (!File.Exists(assemblyUri.LocalPath))
            {
                throw new FileNotFoundException();
            }
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyUri.LocalPath);
            AssemblyInfo assemblyInfo = this.registeredAssemblies.FirstOrDefault(a => assemblyName == a.AssemblyName);
            if (assemblyInfo != null)
            {
                return;
            }
            assemblyInfo = new AssemblyInfo() { AssemblyName = assemblyName, AssemblyUri = assemblyUri };
            this.registeredAssemblies.Add(assemblyInfo);
        }
        private static Uri GetFileUri(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                return null;
            }
            Uri uri;
            if (!Uri.TryCreate(filePath, UriKind.Absolute, out uri))
            {
                return null;
            }
            if (!uri.IsFile)
            {
                return null;
            }
            return uri;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom")]
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            AssemblyInfo assemblyInfo = this.registeredAssemblies.FirstOrDefault(a => AssemblyName.ReferenceMatchesDefinition(assemblyName, a.AssemblyName));
            if (assemblyInfo != null)
            {
                if (assemblyInfo.Assembly == null)
                {
                    assemblyInfo.Assembly = Assembly.LoadFrom(assemblyInfo.AssemblyUri.LocalPath);
                }
                return assemblyInfo.Assembly;
            }
            return null;
        }
        private class AssemblyInfo
        {
            public AssemblyName AssemblyName { get; set; }
            public Uri AssemblyUri { get; set; }
            public Assembly Assembly { get; set; }
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this.handlesAssemblyResolve)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= this.CurrentDomain_AssemblyResolve;
                this.handlesAssemblyResolve = false;
            }
        }
    }
}
