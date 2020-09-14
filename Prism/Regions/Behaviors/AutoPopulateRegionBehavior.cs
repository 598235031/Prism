using System.Collections.Generic;
namespace Wanghzh.Prism.Regions.Behaviors
{
    public class AutoPopulateRegionBehavior : RegionBehavior
    {
        public const string BehaviorKey = "AutoPopulate";
        private readonly IRegionViewRegistry regionViewRegistry;
        public AutoPopulateRegionBehavior(IRegionViewRegistry regionViewRegistry)
        {
            this.regionViewRegistry = regionViewRegistry;
        }
        protected override void OnAttach()
        {
            if (string.IsNullOrEmpty(this.Region.Name))
            {
                this.Region.PropertyChanged += this.Region_PropertyChanged;
            }
            else
            {
                this.StartPopulatingContent();
            }
        }
        private void StartPopulatingContent()
        {
            foreach (object view in this.CreateViewsToAutoPopulate())
            {
                AddViewIntoRegion(view);
            }
            this.regionViewRegistry.ContentRegistered += this.OnViewRegistered;
        }
        protected virtual IEnumerable<object> CreateViewsToAutoPopulate()
        {
            return this.regionViewRegistry.GetContents(this.Region.Name);
        }
        protected virtual void AddViewIntoRegion(object viewToAdd)
        {
            this.Region.Add(viewToAdd);
        }
        private void Region_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && !string.IsNullOrEmpty(this.Region.Name))
            {
                this.Region.PropertyChanged -= this.Region_PropertyChanged;
                this.StartPopulatingContent();
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers", Justification = "This has to be public in order to work with weak references in partial trust or Silverlight environments.")]
        public virtual void OnViewRegistered(object sender, ViewRegisteredEventArgs e)
        {
            if (e == null) throw new System.ArgumentNullException("e");
            if (e.RegionName == this.Region.Name)
            {
                AddViewIntoRegion(e.GetView());
            }
        }
    }
}
