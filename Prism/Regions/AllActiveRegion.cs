using System;
using Wanghzh.Prism.Properties;
namespace Wanghzh.Prism.Regions
{
    public class AllActiveRegion : Region
    {
        public override IViewsCollection ActiveViews
        {
            get { return Views; }
        }
        public override void Deactivate(object view)
        {
            throw new InvalidOperationException(Resources.DeactiveNotPossibleException);
        }
    }
}
