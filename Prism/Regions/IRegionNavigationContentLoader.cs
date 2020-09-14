using System;
namespace Wanghzh.Prism.Regions
{
    public interface IRegionNavigationContentLoader
    {
        object LoadContent(IRegion region, NavigationContext navigationContext);
    }
}
