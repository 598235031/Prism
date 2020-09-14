using System;
using System.ComponentModel;
using System.Windows;
using Wanghzh.Prism.Properties;
using System.Globalization;
namespace Wanghzh.Prism.Regions.Behaviors
{
    public class RegionManagerRegistrationBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public static readonly string BehaviorKey = "RegionManagerRegistration";
        private WeakReference attachedRegionManagerWeakReference;
        private DependencyObject hostControl;
        public RegionManagerRegistrationBehavior()
        {
            this.RegionManagerAccessor = new DefaultRegionManagerAccessor();
        }
        public IRegionManagerAccessor RegionManagerAccessor { get; set; }
        public DependencyObject HostControl
        {
            get
            {
                return hostControl;
            }
            set
            {
                if (IsAttached)
                {
                    throw new InvalidOperationException(Resources.HostControlCannotBeSetAfterAttach);
                }
                this.hostControl = value;
            }
        }
        protected override void OnAttach()
        {
            if (string.IsNullOrEmpty(this.Region.Name))
            {
                this.Region.PropertyChanged += this.Region_PropertyChanged;
            }
            else
            {
                this.StartMonitoringRegionManager();
            }
        }
        private void Region_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && !string.IsNullOrEmpty(this.Region.Name))
            {
                this.Region.PropertyChanged -= this.Region_PropertyChanged;
                this.StartMonitoringRegionManager();
            }
        }
        private void StartMonitoringRegionManager()
        {
            this.RegionManagerAccessor.UpdatingRegions += this.OnUpdatingRegions;
            this.TryRegisterRegion();
        }
        private void TryRegisterRegion()
        {
            DependencyObject targetElement = this.HostControl;
            if (targetElement.CheckAccess())
            {
                IRegionManager regionManager = this.FindRegionManager(targetElement);
                IRegionManager attachedRegionManager = this.GetAttachedRegionManager();
                if (regionManager != attachedRegionManager)
                {
                    if (attachedRegionManager != null)
                    {
                        this.attachedRegionManagerWeakReference = null;
                        attachedRegionManager.Regions.Remove(this.Region.Name);
                    }
                    if (regionManager != null)
                    {
                        this.attachedRegionManagerWeakReference = new WeakReference(regionManager);
                        regionManager.Regions.Add(this.Region);
                    }
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This has to be public in order to work with weak references in partial trust or Silverlight environments.")]
        public void OnUpdatingRegions(object sender, EventArgs e)
        {
            this.TryRegisterRegion();
        }
        private IRegionManager FindRegionManager(DependencyObject dependencyObject)
        {
            var regionmanager = this.RegionManagerAccessor.GetRegionManager(dependencyObject);
            if (regionmanager != null)
            {
                return regionmanager;
            }
            DependencyObject parent = null;
            parent = LogicalTreeHelper.GetParent(dependencyObject);
            if (parent != null)
            {
                return this.FindRegionManager(parent);
            }
            return null;
        }
        private IRegionManager GetAttachedRegionManager()
        {
            if (this.attachedRegionManagerWeakReference != null)
            {
                return this.attachedRegionManagerWeakReference.Target as IRegionManager;
            }
            return null;
        }
    }
}
