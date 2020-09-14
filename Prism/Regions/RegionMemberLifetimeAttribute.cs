using System;
using Wanghzh.Prism.Regions.Behaviors;
namespace Wanghzh.Prism.Regions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true )]
    public sealed class RegionMemberLifetimeAttribute : Attribute
    {
        public RegionMemberLifetimeAttribute()
        {
            KeepAlive = true;
        }
        public bool KeepAlive { get; set; }
    }
}
