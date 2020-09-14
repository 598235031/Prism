using System;
using System.Collections.Generic;
namespace Wanghzh.Prism.Regions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "It is more of a factory than a collection")]
    public interface IRegionBehaviorFactory : IEnumerable<string>
    {
        void AddIfMissing(string behaviorKey, Type behaviorType);
        bool ContainsKey(string behaviorKey);
        IRegionBehavior CreateFromKey(string key);
    }
}
