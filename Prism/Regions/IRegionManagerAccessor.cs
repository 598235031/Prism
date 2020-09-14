using System;
using System.Windows;
namespace Wanghzh.Prism.Regions
{
    public interface IRegionManagerAccessor
    {
        event EventHandler UpdatingRegions;
        string GetRegionName(DependencyObject element);
        IRegionManager GetRegionManager(DependencyObject element);
    }
}
