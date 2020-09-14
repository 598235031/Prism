using System.Collections.Generic;
using System.Collections.Specialized;
namespace Wanghzh.Prism.Regions
{
    public interface IRegionCollection : IEnumerable<IRegion>, INotifyCollectionChanged
    {
        IRegion this[string regionName] { get; }
        void Add(IRegion region);
        bool Remove(string regionName);
        bool ContainsRegionWithName(string regionName);
    }
}
