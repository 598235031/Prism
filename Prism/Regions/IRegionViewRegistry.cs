using System;
using System.Collections.Generic;
namespace Wanghzh.Prism.Regions
{
    public interface IRegionViewRegistry
    {
        event EventHandler<ViewRegisteredEventArgs> ContentRegistered;
        IEnumerable<object> GetContents(string regionName);
        void RegisterViewWithRegion(string regionName, Type viewType);
        void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate);
    }
}
