using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions
{
    public class ContentControlRegionAdapter : RegionAdapterBase<ContentControl>
    {
        public ContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }
        protected override void Adapt(IRegion region, ContentControl regionTarget)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            bool contentIsSet = regionTarget.Content != null;
            contentIsSet = contentIsSet || (BindingOperations.GetBinding(regionTarget, ContentControl.ContentProperty) != null);
            if (contentIsSet)
            {
                throw new InvalidOperationException(Resources.ContentControlHasContentException);
            }
            region.ActiveViews.CollectionChanged += delegate
            {
                regionTarget.Content = region.ActiveViews.FirstOrDefault();
            };
            region.Views.CollectionChanged +=
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0)
                    {
                        region.Activate(e.NewItems[0]);
                    }
                };
        }
        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
