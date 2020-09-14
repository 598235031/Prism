using System.Windows.Controls;
namespace Wanghzh.Prism.Regions
{
    public interface IRegionManager
    {
        IRegionCollection Regions { get; }
        IRegionManager CreateRegionManager();
    }
}
