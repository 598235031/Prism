using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Modularity
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class ModuleInfoGroup : IModuleCatalogItem, IList<ModuleInfo>, IList // IList must be supported in Silverlight 2 to be able to add items from XAML
    {
        private readonly Collection<ModuleInfo> modules = new Collection<ModuleInfo>();
        public InitializationMode InitializationMode { get; set; }
        public string Ref { get; set; }
        public void Add(ModuleInfo item)
        {
            this.ForwardValues(item);
            this.modules.Add(item);
        }
        protected void ForwardValues(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new System.ArgumentNullException("moduleInfo");
            if (moduleInfo.Ref == null)
            {
                moduleInfo.Ref = this.Ref;
            }
            if (moduleInfo.InitializationMode == InitializationMode.WhenAvailable && this.InitializationMode != InitializationMode.WhenAvailable)
            {
                moduleInfo.InitializationMode = this.InitializationMode;
            }
        }
        public void Clear()
        {
            this.modules.Clear();
        }
        public bool Contains(ModuleInfo item)
        {
            return this.modules.Contains(item);
        }
        public void CopyTo(ModuleInfo[] array, int arrayIndex)
        {
            this.modules.CopyTo(array, arrayIndex);
        }
        public int Count
        {
            get { return this.modules.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public bool Remove(ModuleInfo item)
        {
            return this.modules.Remove(item);
        }
        public IEnumerator<ModuleInfo> GetEnumerator()
        {
            return this.modules.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        int IList.Add(object value)
        {
            this.Add((ModuleInfo)value);
            return 1;
        }
        bool IList.Contains(object value)
        {
            if (value == null) throw new ArgumentNullException("value");
            ModuleInfo moduleInfo = value as ModuleInfo;
            if (moduleInfo == null)
                throw new ArgumentException(Resources.ValueMustBeOfTypeModuleInfo, "value");
            return this.Contains(moduleInfo);
        }
        public int IndexOf(object value)
        {
            return this.modules.IndexOf((ModuleInfo)value);
        }
        public void Insert(int index, object value)
        {
            if (value == null) 
                throw new ArgumentNullException("value");
            ModuleInfo moduleInfo = value as ModuleInfo;
            if (moduleInfo == null)
                throw new ArgumentException(Resources.ValueMustBeOfTypeModuleInfo, "value");
            this.modules.Insert(index, moduleInfo);
        }
        public bool IsFixedSize
        {
            get { return false; }
        }
        void IList.Remove(object value)
        {
            this.Remove((ModuleInfo)value);
        }
        public void RemoveAt(int index)
        {
            this.modules.RemoveAt(index);
        }
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (ModuleInfo)value; }
        }
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)this.modules).CopyTo(array, index);
        }
        public bool IsSynchronized
        {
            get { return ((ICollection)this.modules).IsSynchronized; }
        }
        public object SyncRoot
        {
            get { return ((ICollection)this.modules).SyncRoot; }
        }
        public int IndexOf(ModuleInfo item)
        {
            return this.modules.IndexOf(item);
        }
        public void Insert(int index, ModuleInfo item)
        {
            this.modules.Insert(index, item);
        }
        public ModuleInfo this[int index]
        {
            get { return this.modules[index]; }
            set { this.modules[index] = value; }
        }
    }
}
