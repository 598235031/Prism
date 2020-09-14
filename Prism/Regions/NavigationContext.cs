using System;
using System.Collections.Generic;
namespace Wanghzh.Prism.Regions
{
    public class NavigationContext
    {
        public NavigationContext(IRegionNavigationService navigationService, Uri uri) : this(navigationService, uri, null)
        {
        }
        public NavigationContext(IRegionNavigationService navigationService, Uri uri, NavigationParameters navigationParameters)
        {
            this.NavigationService = navigationService;
            this.Uri = uri;
            this.Parameters = uri != null ? UriParsingHelper.ParseQuery(uri) : null;
            this.GetNavigationParameters(navigationParameters);
        }
        public IRegionNavigationService NavigationService { get; private set; }
        public Uri Uri { get; private set; }
        public NavigationParameters Parameters { get; private set; }
        private void GetNavigationParameters(NavigationParameters navigationParameters)
        {
            if (this.Parameters == null || this.NavigationService == null || this.NavigationService.Region == null)
            {
                this.Parameters = new NavigationParameters();
                return;
            }
            if (navigationParameters != null)
            { 
                foreach (KeyValuePair<string, object> navigationParameter in navigationParameters)
                {
                    this.Parameters.Add(navigationParameter.Key, navigationParameter.Value);
                }
            }
        }
    }
}
