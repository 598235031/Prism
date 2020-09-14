using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using CommonServiceLocator;
using Wanghzh.Prism.Events;
using Wanghzh.Prism.Properties;
using Wanghzh.Prism.Regions.Behaviors;
 
namespace Wanghzh.Prism.Regions
{
    public class RegionManager : IRegionManager
    {
        private static readonly WeakDelegatesManager updatingRegionsListeners = new WeakDelegatesManager();
        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached(
            "RegionName",
            typeof(string),
            typeof(RegionManager),
            new PropertyMetadata(OnSetRegionNameCallback));
        public static void SetRegionName(DependencyObject regionTarget, string regionName)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            regionTarget.SetValue(RegionNameProperty, regionName);
        }
        public static string GetRegionName(DependencyObject regionTarget)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            return regionTarget.GetValue(RegionNameProperty) as string;
        }
        private static readonly DependencyProperty ObservableRegionProperty =
            DependencyProperty.RegisterAttached("ObservableRegion", typeof(ObservableObject<IRegion>), typeof(RegionManager), null);
        public static ObservableObject<IRegion> GetObservableRegion(DependencyObject view)
        {
            if (view == null) throw new ArgumentNullException("view");
            ObservableObject<IRegion> regionWrapper = view.GetValue(ObservableRegionProperty) as ObservableObject<IRegion>;
            if (regionWrapper == null)
            {
                regionWrapper = new ObservableObject<IRegion>();
                view.SetValue(ObservableRegionProperty, regionWrapper);
            }
            return regionWrapper;
        }
        private static void OnSetRegionNameCallback(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            if (!IsInDesignMode(element))
            {
                CreateRegion(element);
            }
        }
        private static void CreateRegion(DependencyObject element)
        {
            IServiceLocator locator = ServiceLocator.Current;
            DelayedRegionCreationBehavior regionCreationBehavior = locator.GetInstance<DelayedRegionCreationBehavior>();
            regionCreationBehavior.TargetElement = element;
            regionCreationBehavior.Attach();
        }
        public static readonly DependencyProperty RegionManagerProperty =
            DependencyProperty.RegisterAttached("RegionManager", typeof(IRegionManager), typeof(RegionManager), null);
        public static IRegionManager GetRegionManager(DependencyObject target)
        {
            if (target == null) throw new ArgumentNullException("target");
            return (IRegionManager)target.GetValue(RegionManagerProperty);
        }
        public static void SetRegionManager(DependencyObject target, IRegionManager value)
        {
            if (target == null) throw new ArgumentNullException("target");
            target.SetValue(RegionManagerProperty, value);
        }
        public static readonly DependencyProperty RegionContextProperty =
            DependencyProperty.RegisterAttached("RegionContext", typeof(object), typeof(RegionManager), new PropertyMetadata(OnRegionContextChanged));
        private static void OnRegionContextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (RegionContext.GetObservableContext(depObj).Value != e.NewValue)
            {
                RegionContext.GetObservableContext(depObj).Value = e.NewValue;
            }
        }
        public static object GetRegionContext(DependencyObject target)
        {
            if (target == null) throw new ArgumentNullException("target");
            return target.GetValue(RegionContextProperty);
        }
        public static void SetRegionContext(DependencyObject target, object value)
        {
            if (target == null) throw new ArgumentNullException("target");
            target.SetValue(RegionContextProperty, value);
        }
        public static event EventHandler UpdatingRegions
        {
            add { updatingRegionsListeners.AddListener(value); }
            remove { updatingRegionsListeners.RemoveListener(value); }
        }
        public static void UpdateRegions()
        {
            try
            {
                updatingRegionsListeners.Raise(null, EventArgs.Empty);
            }
            catch (TargetInvocationException ex)
            {
                Exception rootException = ex.GetRootException();
                throw new UpdateRegionsException(string.Format(CultureInfo.CurrentCulture,
                    Resources.UpdateRegionException, rootException), ex.InnerException);
            }
        }
        private static bool IsInDesignMode(DependencyObject element)
        {
            return DesignerProperties.GetIsInDesignMode(element);
        }
        private readonly RegionCollection regionCollection;
        public RegionManager()
        {
            regionCollection = new RegionCollection(this);
        }
        public IRegionCollection Regions
        {
            get { return regionCollection; }
        }
        public IRegionManager CreateRegionManager()
        {
            return new RegionManager();
        }
        private class RegionCollection : IRegionCollection
        {
            private readonly IRegionManager regionManager;
            private readonly List<IRegion> regions;
            public RegionCollection(IRegionManager regionManager)
            {
                this.regionManager = regionManager;
                this.regions = new List<IRegion>();
            }
            public event NotifyCollectionChangedEventHandler CollectionChanged;
            public IEnumerator<IRegion> GetEnumerator()
            {
                UpdateRegions();
                return this.regions.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            public IRegion this[string regionName]
            {
                get
                {
                    UpdateRegions();
                    IRegion region = GetRegionByName(regionName);
                    if (region == null)
                    {
                        throw new KeyNotFoundException(string.Format(CultureInfo.CurrentUICulture, Resources.RegionNotInRegionManagerException, regionName));
                    }
                    return region;
                }
            }
            public void Add(IRegion region)
            {
                if (region == null) throw new ArgumentNullException("region");
                UpdateRegions();
                if (region.Name == null)
                {
                    throw new InvalidOperationException(Resources.RegionNameCannotBeEmptyException);
                }
                if (this.GetRegionByName(region.Name) != null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                                                              Resources.RegionNameExistsException, region.Name));
                }
                this.regions.Add(region);
                region.RegionManager = this.regionManager;
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, region, 0));
            }
            public bool Remove(string regionName)
            {
                UpdateRegions();
                bool removed = false;
                IRegion region = GetRegionByName(regionName);
                if (region != null)
                {
                    removed = true;
                    this.regions.Remove(region);
                    region.RegionManager = null;
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, region, 0));
                }
                return removed;
            }
            public bool ContainsRegionWithName(string regionName)
            {
                UpdateRegions();
                return GetRegionByName(regionName) != null;
            }
            private IRegion GetRegionByName(string regionName)
            {
                return this.regions.FirstOrDefault(r => r.Name == regionName);
            }
            private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
            {
                var handler = this.CollectionChanged;
                if (handler != null)
                {
                    handler(this, notifyCollectionChangedEventArgs);
                }
            }
        }
    }
}
