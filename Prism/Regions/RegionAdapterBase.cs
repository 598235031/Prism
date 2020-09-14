using System;
using System.Globalization;
using System.Windows;
using Wanghzh.Prism.Properties;
using Wanghzh.Prism.Regions.Behaviors;
namespace Wanghzh.Prism.Regions
{
    public abstract class RegionAdapterBase<T> : IRegionAdapter where T : class
    {
        protected RegionAdapterBase(IRegionBehaviorFactory regionBehaviorFactory)
        {
            this.RegionBehaviorFactory = regionBehaviorFactory;
        }
        protected IRegionBehaviorFactory RegionBehaviorFactory { get; set; }
        public IRegion Initialize(T regionTarget, string regionName)
        {
            if (regionName == null)
            {
                throw new ArgumentNullException("regionName");
            }
            IRegion region = this.CreateRegion();
            region.Name = regionName;
            SetObservableRegionOnHostingControl(region, regionTarget);
            this.Adapt(region, regionTarget);
            this.AttachBehaviors(region, regionTarget);
            this.AttachDefaultBehaviors(region, regionTarget);
            return region;
        }
        IRegion IRegionAdapter.Initialize(object regionTarget, string regionName)
        {
            return this.Initialize(GetCastedObject(regionTarget), regionName);
        }
        protected virtual void AttachDefaultBehaviors(IRegion region, T regionTarget)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            IRegionBehaviorFactory behaviorFactory = this.RegionBehaviorFactory;
            if (behaviorFactory != null)
            {
                DependencyObject dependencyObjectRegionTarget = regionTarget as DependencyObject;
                foreach (string behaviorKey in behaviorFactory)
                {
                    if (!region.Behaviors.ContainsKey(behaviorKey))
                    {
                        IRegionBehavior behavior = behaviorFactory.CreateFromKey(behaviorKey);
                        if (dependencyObjectRegionTarget != null)
                        {
                            IHostAwareRegionBehavior hostAwareRegionBehavior = behavior as IHostAwareRegionBehavior;
                            if (hostAwareRegionBehavior != null)
                            {
                                hostAwareRegionBehavior.HostControl = dependencyObjectRegionTarget;
                            }
                        }
                        region.Behaviors.Add(behaviorKey, behavior);
                    }
                }
            }
        }
        protected virtual void AttachBehaviors(IRegion region, T regionTarget)
        {
        }
        protected abstract void Adapt(IRegion region, T regionTarget);
        protected abstract IRegion CreateRegion();
        private static T GetCastedObject(object regionTarget)
        {
            if (regionTarget == null)
            {
                throw new ArgumentNullException("regionTarget");
            }
            T castedObject = regionTarget as T;
            if (castedObject == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Resources.AdapterInvalidTypeException, typeof(T).Name));
            }
            return castedObject;
        }
        private static void SetObservableRegionOnHostingControl(IRegion region, T regionTarget)
        {
            DependencyObject targetElement = regionTarget as DependencyObject;
            if (targetElement != null)
            {
                RegionManager.GetObservableRegion(targetElement).Value = region;
            }
        }
    }
}
