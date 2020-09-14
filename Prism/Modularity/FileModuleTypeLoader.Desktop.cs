using System;
using System.IO;
using System.Collections.Generic;
namespace Wanghzh.Prism.Modularity
{
    public class FileModuleTypeLoader : IModuleTypeLoader, IDisposable
    {
        private const string RefFilePrefix = "file://";
        private readonly IAssemblyResolver assemblyResolver;
        private HashSet<Uri> downloadedUris = new HashSet<Uri>();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This is disposed of in the Dispose method.")]
        public FileModuleTypeLoader()
            : this(new AssemblyResolver())
        {
        }
        public FileModuleTypeLoader(IAssemblyResolver assemblyResolver)
        {
            this.assemblyResolver = assemblyResolver;
        }
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;
        private void RaiseModuleDownloadProgressChanged(ModuleInfo moduleInfo, long bytesReceived, long totalBytesToReceive)
        {
            this.RaiseModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, bytesReceived, totalBytesToReceive));
        }
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
        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                throw new System.ArgumentNullException("moduleInfo");
            }
            return moduleInfo.Ref != null && moduleInfo.Ref.StartsWith(RefFilePrefix, StringComparison.Ordinal);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is rethrown as part of a completion event")]
        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                throw new System.ArgumentNullException("moduleInfo");
            }
            try
            {
                Uri uri = new Uri(moduleInfo.Ref, UriKind.RelativeOrAbsolute);
                if (this.IsSuccessfullyDownloaded(uri))
                {
                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
                else
                {
                    string path;
                    if (moduleInfo.Ref.StartsWith(RefFilePrefix + "/", StringComparison.Ordinal))
                    {
                        path = moduleInfo.Ref.Substring(RefFilePrefix.Length + 1);
                    }
                    else
                    {
                        path = moduleInfo.Ref.Substring(RefFilePrefix.Length);
                    }
                    long fileSize = -1L;
                    if (File.Exists(path))
                    {
                        FileInfo fileInfo = new FileInfo(path);
                        fileSize = fileInfo.Length;
                    }
                    this.RaiseModuleDownloadProgressChanged(moduleInfo, 0, fileSize);
                    this.assemblyResolver.LoadAssemblyFrom(moduleInfo.Ref);
                    this.RaiseModuleDownloadProgressChanged(moduleInfo, fileSize, fileSize);
                    this.RecordDownloadSuccess(uri);
                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
            }
            catch (Exception ex)
            {
                this.RaiseLoadModuleCompleted(moduleInfo, ex);
            }
        }
        private bool IsSuccessfullyDownloaded(Uri uri)
        {
            lock (this.downloadedUris)
            {
                return this.downloadedUris.Contains(uri);
            }
        }
        private void RecordDownloadSuccess(Uri uri)
        {
            lock (this.downloadedUris)
            {
                this.downloadedUris.Add(uri);
            }
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            IDisposable disposableResolver = this.assemblyResolver as IDisposable;
            if (disposableResolver != null)
            {
                disposableResolver.Dispose();
            }
        }
    }
}
