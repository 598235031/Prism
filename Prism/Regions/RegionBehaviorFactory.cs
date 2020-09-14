using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CommonServiceLocator;
using Wanghzh.Prism.Properties;
 
namespace Wanghzh.Prism.Regions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "It is more of a factory than a collection")]
    public class RegionBehaviorFactory : IRegionBehaviorFactory
    {
        private readonly IServiceLocator serviceLocator;
        private readonly Dictionary<string, Type> registeredBehaviors = new Dictionary<string, Type>();
        public RegionBehaviorFactory(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }
        public void AddIfMissing(string behaviorKey, Type behaviorType)
        {
            if (behaviorKey == null)
            {
                throw new ArgumentNullException("behaviorKey");
            }
            if (behaviorType == null)
            {
                throw new ArgumentNullException("behaviorType");
            }
            if (!typeof(IRegionBehavior).IsAssignableFrom(behaviorType))
            {
                throw new ArgumentException(
                    string.Format(Thread.CurrentThread.CurrentCulture, Resources.CanOnlyAddTypesThatInheritIFromRegionBehavior, behaviorType.Name), "behaviorType");
            }
            if (this.registeredBehaviors.ContainsKey(behaviorKey))
            {
                return;
            }
            this.registeredBehaviors.Add(behaviorKey, behaviorType);
        }
        public IRegionBehavior CreateFromKey(string key)
        {
            if (!this.ContainsKey(key))
            {
                throw new ArgumentException(
                    string.Format(Thread.CurrentThread.CurrentCulture, Resources.TypeWithKeyNotRegistered, key), "key");
            }
                
            return (IRegionBehavior)this.serviceLocator.GetInstance(this.registeredBehaviors[key]);
        }
        public IEnumerator<string> GetEnumerator()
        {
            return this.registeredBehaviors.Keys.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public bool ContainsKey(string behaviorKey)
        {
            return this.registeredBehaviors.ContainsKey(behaviorKey);
        }
    }
}
