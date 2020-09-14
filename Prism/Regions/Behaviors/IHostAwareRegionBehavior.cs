using System.Windows;
namespace Wanghzh.Prism.Regions.Behaviors
{
    public interface IHostAwareRegionBehavior : IRegionBehavior
    {
        DependencyObject HostControl { get; set; }
    }
}
