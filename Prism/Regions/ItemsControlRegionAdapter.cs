using System;
using System.Windows.Controls;
using System.Windows.Data;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions
{
    public class ItemsControlRegionAdapter : RegionAdapterBase<ItemsControl>
    {
        public ItemsControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }
        protected override void Adapt(IRegion region, ItemsControl regionTarget)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            bool itemsSourceIsSet = regionTarget.ItemsSource != null;
            itemsSourceIsSet = itemsSourceIsSet || (BindingOperations.GetBinding(regionTarget, ItemsControl.ItemsSourceProperty) != null);
            if (itemsSourceIsSet)
            {
                throw new InvalidOperationException(Resources.ItemsControlHasItemsSourceException);
            }
            if (regionTarget.Items.Count > 0)
            {
                foreach (object childItem in regionTarget.Items)
                {
                    region.Add(childItem);
                }
                regionTarget.Items.Clear();
            }
            regionTarget.ItemsSource = region.Views;
        }
        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }
    }
}
