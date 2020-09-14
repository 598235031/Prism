using System;
using System.Windows;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions.Behaviors
{
    public class SyncRegionContextWithHostBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        private const string RegionContextPropertyName = "Context";
        private DependencyObject hostControl;
        public static readonly string BehaviorKey = "SyncRegionContextWithHost";
        private ObservableObject<object> HostControlRegionContext
        {
            get
            {
                return RegionContext.GetObservableContext(this.hostControl);
            }
        }
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
            if (this.HostControl != null)
            {
                SynchronizeRegionContext();
                this.HostControlRegionContext.PropertyChanged += this.RegionContextObservableObject_PropertyChanged;
                this.Region.PropertyChanged += this.Region_PropertyChanged;
            }
        }
        void Region_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RegionContextPropertyName)
            {
                if (RegionManager.GetRegionContext(this.HostControl) != this.Region.Context)
                {
                    RegionManager.SetRegionContext(this.hostControl, this.Region.Context);
                }
            }
        }
        void RegionContextObservableObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                SynchronizeRegionContext();
            }
        }
        private void SynchronizeRegionContext()
        {
            if (this.Region.Context != this.HostControlRegionContext.Value)
            {
                this.Region.Context = this.HostControlRegionContext.Value;
            }
            if (RegionManager.GetRegionContext(this.HostControl) != this.HostControlRegionContext.Value)
            {
                RegionManager.SetRegionContext(this.HostControl, this.HostControlRegionContext.Value);
            }
        }
    }
}
