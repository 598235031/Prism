namespace Wanghzh.Prism.Regions
{
    public interface IRegionAdapter
    {
        IRegion Initialize(object regionTarget, string regionName);
    }
}
