using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using CommonServiceLocator;
using Wanghzh.Prism.Events;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions
{
    public class RegionViewRegistry : IRegionViewRegistry
    {
        private readonly IServiceLocator locator;
        private readonly ListDictionary<string, Func<object>> registeredContent = new ListDictionary<string, Func<object>>();
        private readonly WeakDelegatesManager contentRegisteredListeners = new WeakDelegatesManager();
        public RegionViewRegistry(IServiceLocator locator)
        {
            this.locator = locator;
        }
        public event EventHandler<ViewRegisteredEventArgs> ContentRegistered
        {
            add { this.contentRegisteredListeners.AddListener(value); }
            remove { this.contentRegisteredListeners.RemoveListener(value); }
        }
        public IEnumerable<object> GetContents(string regionName)
        {
            List<object> items = new List<object>();
            foreach (Func<object> getContentDelegate in this.registeredContent[regionName])
            {
                items.Add(getContentDelegate());
            }
            return items;
        }
        public void RegisterViewWithRegion(string regionName, Type viewType)
        {
            this.RegisterViewWithRegion(regionName, () => this.CreateInstance(viewType));
        }
        public void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            this.registeredContent.Add(regionName, getContentDelegate);
            this.OnContentRegistered(new ViewRegisteredEventArgs(regionName, getContentDelegate));
        }
        protected virtual object CreateInstance(Type type)
        {
            return this.locator.GetInstance(type);
        }
        private void OnContentRegistered(ViewRegisteredEventArgs e)
        {
            try
            {
                this.contentRegisteredListeners.Raise(this, e);
            }
            catch (TargetInvocationException ex)
            {
                Exception rootException;
                if (ex.InnerException != null)
                {
                     rootException = ex.InnerException.GetRootException();
                }
                else
                {
                    rootException = ex.GetRootException();
                }
                throw new ViewRegistrationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.OnViewRegisteredException, e.RegionName, rootException), ex.InnerException);
            }
        }
    }
}
