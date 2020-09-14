using System;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions
{
    public abstract class RegionBehavior : IRegionBehavior
    {
        private IRegion region;
        public IRegion Region
        {
            get
            {
                return region;
            }
            set
            {
                if (this.IsAttached)
                {
                    throw new InvalidOperationException(Resources.RegionBehaviorRegionCannotBeSetAfterAttach);
                }
                this.region = value;
            }
        }
        public bool IsAttached { get; private set; }
        public void Attach()
        {
            if (this.region == null)
            {
                throw new InvalidOperationException(Resources.RegionBehaviorAttachCannotBeCallWithNullRegion);
            }
            IsAttached = true;
            OnAttach();
        }
        protected abstract void OnAttach();
    }
}
