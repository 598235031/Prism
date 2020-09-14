using System;
using System.Collections;
using System.Collections.Generic;
namespace Wanghzh.Prism.Regions
{
    public class RegionBehaviorCollection : IRegionBehaviorCollection
    {
        private readonly IRegion region;
        private readonly Dictionary<string, IRegionBehavior> behaviors = new Dictionary<string, IRegionBehavior>();
        public RegionBehaviorCollection(IRegion region)
        {
            this.region = region;
        }
        public IRegionBehavior this[string key]
        {
            get { return this.behaviors[key]; }
        }
        public void Add(string key, IRegionBehavior regionBehavior)
        {
            if (key == null) 
                throw new ArgumentNullException("key");
            if (regionBehavior == null) 
                throw new ArgumentNullException("regionBehavior");
            if (this.behaviors.ContainsKey(key))
                throw new  ArgumentException("Could not add duplicate behavior with same key.", "key");
            this.behaviors.Add(key, regionBehavior);
            regionBehavior.Region = this.region;
            regionBehavior.Attach();
        }
        public bool ContainsKey(string key)
        {
            return this.behaviors.ContainsKey(key);
        }
        public IEnumerator<KeyValuePair<string, IRegionBehavior>> GetEnumerator()
        {
            return behaviors.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return behaviors.GetEnumerator();
        }
    }
}
