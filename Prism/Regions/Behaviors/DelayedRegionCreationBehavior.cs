using System;
using System.Globalization;
using System.Windows;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions.Behaviors
{
    public class DelayedRegionCreationBehavior
    {
        private readonly RegionAdapterMappings regionAdapterMappings;
        private WeakReference elementWeakReference;
        private bool regionCreated;
        public DelayedRegionCreationBehavior(RegionAdapterMappings regionAdapterMappings)
        {
            this.regionAdapterMappings = regionAdapterMappings;
            this.RegionManagerAccessor = new DefaultRegionManagerAccessor();
        }
        public IRegionManagerAccessor RegionManagerAccessor { get; set; }
        public DependencyObject TargetElement
        {
            get { return this.elementWeakReference != null ? this.elementWeakReference.Target as DependencyObject : null; }
            set { this.elementWeakReference = new WeakReference(value); }
        }
        public void Attach()
        {
            this.RegionManagerAccessor.UpdatingRegions += this.OnUpdatingRegions;
            this.WireUpTargetElement();
        }
        public void Detach()
        {
            this.RegionManagerAccessor.UpdatingRegions -= this.OnUpdatingRegions;
            this.UnWireTargetElement();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This has to be public in order to work with weak references in partial trust or Silverlight environments.")]
        public void OnUpdatingRegions(object sender, EventArgs e)
        {
            this.TryCreateRegion();
        }
        private void TryCreateRegion()
        {
            DependencyObject targetElement = this.TargetElement;
            if (targetElement == null)
            {
                this.Detach();
                return;
            }
            if (targetElement.CheckAccess())
            {
                this.Detach();
                if (!this.regionCreated)
                {
                    string regionName = this.RegionManagerAccessor.GetRegionName(targetElement);
                    CreateRegion(targetElement, regionName);
                    this.regionCreated = true;
                }
            }
        }
        protected virtual IRegion CreateRegion(DependencyObject targetElement, string regionName)
        {
            if (targetElement == null) throw new ArgumentNullException("targetElement");
            try
            {
                IRegionAdapter regionAdapter = this.regionAdapterMappings.GetMapping(targetElement.GetType());
                IRegion region = regionAdapter.Initialize(targetElement, regionName);
                return region;
            }
            catch (Exception ex)
            {
                throw new RegionCreationException(string.Format(CultureInfo.CurrentCulture, Resources.RegionCreationException, regionName, ex), ex);
            }
        }
        private void ElementLoaded(object sender, RoutedEventArgs e)
        {
            this.UnWireTargetElement();
            this.TryCreateRegion();
        }
        private void WireUpTargetElement()
        {
            FrameworkElement element = this.TargetElement as FrameworkElement;
            if (element != null)
            {
                element.Loaded += this.ElementLoaded;
            }
        }
        private void UnWireTargetElement()
        {
            FrameworkElement element = this.TargetElement as FrameworkElement;
            if (element != null)
            {
                element.Loaded -= this.ElementLoaded;
            }
        }
    }
}
