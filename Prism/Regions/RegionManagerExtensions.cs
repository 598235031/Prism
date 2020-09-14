using System;
using System.Globalization;
using System.Threading;
using CommonServiceLocator;
using Wanghzh.Prism.Properties;
 
namespace Wanghzh.Prism.Regions
{
    public static class RegionManagerExtensions
    {
        public static IRegionManager AddToRegion(this IRegionManager regionManager, string regionName, object view)
        {
            if (regionManager == null) throw new ArgumentNullException("regionManager");
            if (!regionManager.Regions.ContainsRegionWithName(regionName))
            {
                throw new ArgumentException(
                    string.Format(Thread.CurrentThread.CurrentCulture, Resources.RegionNotFound, regionName), "regionName");
            }
            IRegion region = regionManager.Regions[regionName];
            return region.Add(view);
        }
        public static IRegionManager RegisterViewWithRegion(this IRegionManager regionManager, string regionName, Type viewType)
        {
            var regionViewRegistry = ServiceLocator.Current.GetInstance<IRegionViewRegistry>();
            regionViewRegistry.RegisterViewWithRegion(regionName, viewType);
            return regionManager;
        }
        public static IRegionManager RegisterViewWithRegion(this IRegionManager regionManager, string regionName, Func<object> getContentDelegate)
        {
            var regionViewRegistry = ServiceLocator.Current.GetInstance<IRegionViewRegistry>();
            regionViewRegistry.RegisterViewWithRegion(regionName, getContentDelegate);
            return regionManager;
        }
        public static void Add(this IRegionCollection regionCollection, string regionName, IRegion region)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (regionCollection == null) throw new ArgumentNullException("regionCollection");
            if (region.Name != null && region.Name != regionName)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.RegionManagerWithDifferentNameException, region.Name, regionName), "regionName");
            }
            if (region.Name == null)
            {
                region.Name = regionName;
            }
            regionCollection.Add(region);
        }
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            if (regionManager == null) throw new ArgumentNullException("regionManager");
            if (navigationCallback == null) throw new ArgumentNullException("navigationCallback");
            if (regionManager.Regions.ContainsRegionWithName(regionName))
            {
                regionManager.Regions[regionName].RequestNavigate(source, navigationCallback);
            }
            else
            {
                navigationCallback(new NavigationResult(new NavigationContext(null, source), false));
            }
        }
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, Uri source)
        {
            RequestNavigate(regionManager, regionName, source, nr => { });
        }
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, string source, Action<NavigationResult> navigationCallback)
        {
            if (source == null) throw new ArgumentNullException("source");
            RequestNavigate(regionManager, regionName, new Uri(source, UriKind.RelativeOrAbsolute), navigationCallback);
        }
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, string source)
        {
            RequestNavigate(regionManager, regionName, source, nr => { });
        }
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (regionManager == null)
            {
                return;
            }
            if (regionManager.Regions.ContainsRegionWithName(regionName))
            {
                regionManager.Regions[regionName].RequestNavigate(target, navigationCallback, navigationParameters);
            }
        }
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            RequestNavigate(regionManager, regionName, new Uri(target, UriKind.RelativeOrAbsolute), navigationCallback, navigationParameters);
        }
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, Uri target, NavigationParameters navigationParameters)
        {
            RequestNavigate(regionManager, regionName, target, nr => { }, navigationParameters);
        }
        public static void RequestNavigate(this IRegionManager regionManager, string regionName, string target, NavigationParameters navigationParameters)
        {
            RequestNavigate(regionManager, regionName, new Uri(target, UriKind.RelativeOrAbsolute), nr => { }, navigationParameters);
        }
    }
}
