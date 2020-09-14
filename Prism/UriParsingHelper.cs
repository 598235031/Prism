using System;
using Wanghzh.Prism.Regions;
namespace Wanghzh.Prism
{
    public static class UriParsingHelper
    {
        public static string GetQuery(Uri uri)
        {
            return EnsureAbsolute(uri).Query;
        }
        public static string GetAbsolutePath(Uri uri)
        {
            return EnsureAbsolute(uri).AbsolutePath;
        }
        public static NavigationParameters ParseQuery(Uri uri)
        {
            var query = GetQuery(uri);
            return new NavigationParameters(query);
        }
        private static Uri EnsureAbsolute(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri;
            }
            if ((uri != null) && !uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                return new Uri("http://localhost/" + uri, UriKind.Absolute);
            }
            return new Uri("http://localhost" + uri, UriKind.Absolute);
        }
    }
}
