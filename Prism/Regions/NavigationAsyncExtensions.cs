using System;
namespace Wanghzh.Prism.Regions
{
    public static class NavigationAsyncExtensions
    {
        public static void RequestNavigate(this INavigateAsync navigation, string target)
        {
            RequestNavigate(navigation, target, nr => { });
        }
        public static void RequestNavigate(this INavigateAsync navigation, string target, Action<NavigationResult> navigationCallback)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (target == null) throw new ArgumentNullException("target");
            var targetUri = new Uri(target, UriKind.RelativeOrAbsolute);
            navigation.RequestNavigate(targetUri, navigationCallback);
        }
        public static void RequestNavigate(this INavigateAsync navigation, Uri target)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            navigation.RequestNavigate(target, nr => { });
        }
        public static void RequestNavigate(this INavigateAsync navigation, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (target == null) throw new ArgumentNullException("target");
            var targetUri = new Uri(target, UriKind.RelativeOrAbsolute);
            navigation.RequestNavigate(targetUri, navigationCallback, navigationParameters);
        }
        public static void RequestNavigate(this INavigateAsync navigation, Uri target, NavigationParameters navigationParameters)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            navigation.RequestNavigate(target, nr => { }, navigationParameters);
        }
        public static void RequestNavigate(this INavigateAsync navigation, string target, NavigationParameters navigationParameters)
        {
            if (navigation == null) throw new ArgumentNullException("navigation");
            if (target == null) throw new ArgumentNullException("target");
            navigation.RequestNavigate(new Uri(target, UriKind.RelativeOrAbsolute), nr => { }, navigationParameters);
        }
    }
}
