using System;
namespace Wanghzh.Prism.Regions
{
    public class ViewRegisteredEventArgs : EventArgs
    {
        public ViewRegisteredEventArgs(string regionName, Func<object> getViewDelegate)
        {
            this.GetView = getViewDelegate;
            this.RegionName = regionName;
        }
        public string RegionName { get; private set; }
        public Func<object> GetView { get; private set; }
    }
}
