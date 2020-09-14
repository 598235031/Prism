using System.Collections.Generic;
using System.Collections.Specialized;
namespace Wanghzh.Prism.Regions
{
    public interface IViewsCollection : IEnumerable<object>, INotifyCollectionChanged
    {
        bool Contains(object value);
    }
}
